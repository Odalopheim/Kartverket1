using Microsoft.AspNetCore.Identity;
using Kartverket.API_Models;
using Kartverket.Data;
using Kartverket.Services;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using System.Diagnostics;
using MySqlConnector;
using System.Data;
using Microsoft.CodeAnalysis;


var builder = WebApplication.CreateBuilder(args);

builder.ConfigureAppConfiguration(config =>
{
    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    config.AddEnvironmentVariables();
});

// Legger til services til containeren
builder.Services.AddControllersWithViews();

// Configure MySQL/MariaDB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(10, 5, 8))));

// Add Identity services
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

// Configure authentication
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login/";
    options.AccessDeniedPath = "/Account/AccessDenied/";
});

// Register IDbConnection for Dapper
builder.Services.AddTransient<IDbConnection>((sp) =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection");
    return new MySqlConnection(connectionString);
});

// Register GeoChangeService
builder.Services.AddScoped<GeoChangeService>();

var app = builder.Build();

// Apply migrations at startup with retry logic
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

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


// Binder API settings fra appsettings.json
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// Registrerer services og interface
builder.Services.AddHttpClient<IKommuneInfoService, KommuneInfoService>();
builder.Services.AddHttpClient<IStedsnavnService, StedsnavnService>();

app.UseDeveloperExceptionPage();

// Henter og logger MariaDB-versjon
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

// Konfigurerer HTTP request-pipelinen
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

app.Run();

