using Kartverket.API_Models;
using Kartverket.Data;
using Kartverket.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Binder API settings fra appsettings.json
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));

// Registrerer services og interface
builder.Services.AddHttpClient<IKommuneInfoService, KommuneInfoService>();
builder.Services.AddHttpClient<IStedsnavnService, StedsnavnService>();

// Legger til services til containeren
builder.Services.AddControllersWithViews();

// Konfigurerer Identity med ApplicationDbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(10, 5, 9))
    )
);
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Konfigurerer HTTP request-pipelinen
var app = builder.Build();
app.UseDeveloperExceptionPage();

// Database migrasjon
/*using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.Migrate();
}*/

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

// Legger til autentisering og autorisasjon
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
