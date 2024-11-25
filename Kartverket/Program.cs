using Microsoft.AspNetCore.Identity;
using Kartverket.API_Models;
using Kartverket.Data;
using Kartverket.Services;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using MySqlConnector;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

//Konfigurer Logger
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

//Legg til Konfigurasjonsfiler
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
Console.WriteLine(builder.Environment);

//Legg til Tjenester til Container
builder.Services.AddControllersWithViews();

//Konfigurer MySQL/MariaDB med Transient Feilresiliens
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(10, 5, 8)),
        mysqlOptions => mysqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null
        )
    )
);

//Legg til Identity-tjenester
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

//Konfigurer Autentisering
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login/";
    options.AccessDeniedPath = "/Account/AccessDenied/";
});

//Registrer IDbConnection for Dapper
builder.Services.AddTransient<IDbConnection>((sp) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    return new MySqlConnection(connectionString);
});

//Registrer GeoChangeService
builder.Services.AddScoped<GeoChangeService>();

//Registrerer UserServices
builder.Services.AddScoped<UserService>();

//Bind API-innstillinger fra appsettings.json
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

//Registrer Tjenester og Interface
builder.Services.AddHttpClient<IKommuneInfoService, KommuneInfoService>();
builder.Services.AddHttpClient<IStedsnavnService, StedsnavnService>();

var app = builder.Build();

//Apply Migrations ved Oppstart med Retry Logikk
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    int retryCount = 0;
    int maxRetryAttempts = 10;
    TimeSpan pauseBetweenFailures = TimeSpan.FromSeconds(5);

    while (retryCount < maxRetryAttempts)
    {
        try
        {
            context.Database.Migrate();
            break; // Success!
        }
        catch (MySqlException ex)
        {
            retryCount++;
            logger.LogError(ex, "An error occurred while migrating the database. Attempt {RetryCount}/{MaxRetryAttempts}", retryCount, maxRetryAttempts);
            if (retryCount >= maxRetryAttempts)
            {
                throw; // Give up after max retries
            }
            System.Threading.Thread.Sleep(pauseBetweenFailures);
        }
    }
}

//Initialiser Roller og Brukere etter at App er Opprettet
using (var serviceScope = app.Services.CreateScope())
{
    var serviceProvider = serviceScope.ServiceProvider;
    await RoleInitializer.SeedRolesAsync(serviceProvider);
}

//Hent og Logg MariaDB-versjon
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var connection = dbContext.Database.GetDbConnection();

    try
    {
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT VERSION()";
        var version = command.ExecuteScalar()?.ToString();

        Console.WriteLine($"Connected to MariaDB version: {version}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error fetching MariaDB version: {ex.Message}");
    }
    finally
    {
        connection.Close();
    }
}

//Bruk av Middleware for Feilhåndtering og Sikkerhet
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "admin",
    pattern: "{controller=Admin}/{action=AdminHjemmeside}/{id?}");

app.Run();
