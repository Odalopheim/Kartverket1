using System.Data;
using Dapper;
using Kartverket.Data;
using Microsoft.Extensions.Logging;

public class UserService
{
    private readonly IDbConnection _dbConnection;
    private readonly ILogger<UserService> _logger;

    public UserService(IDbConnection dbConnection, ILogger<UserService> logger)
    {
        _dbConnection = dbConnection;
        _logger = logger;
    }

    //Oppdaterer brukerinformasjon
    public void UpdateUserDetails(string userId, string name, string address, string postNumber)
    {
        string query = @"UPDATE UserDetails 
                         SET Name = @Name, Address = @Address, PostNumber = @PostNumber 
                         WHERE UserId = @UserId";

        _logger.LogInformation("Executing query: " + query);
        _logger.LogInformation($"Parameters - UserId: {userId}, Name: {name}, Address: {address}, PostNumber: {postNumber}");

        _dbConnection.Execute(query, new { UserId = userId, Name = name, Address = address, PostNumber = postNumber });
    }
}
