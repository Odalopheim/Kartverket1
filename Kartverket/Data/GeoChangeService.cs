using Dapper;
using System.Collections.Generic;
using System.Data;
using Kartverket.Models;
using System;
using Microsoft.Extensions.Logging;
using Kartverket.Controllers;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using System.Collections;

namespace Kartverket.Data
{
    public class GeoChangeService
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<GeoChangeService> _logger;

        public GeoChangeService(IDbConnection dbConnection, ILogger<GeoChangeService> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }

        // Inserts a new record into the GeoChanges table
        public void AddGeoChange(string description, string geoJson, string userId, GeoChangeStatus status, GeoChangeCategory category)
        {
            string query = @"INSERT INTO GeoChanges (Description, GeoJson, UserId, Status, CreatedDate, Category) 
                             VALUES (@Description, @GeoJson, @UserId, @Status, @CreatedDate, @Category)";
            _dbConnection.Execute(query, new { Description = description, GeoJson = geoJson, UserId = userId, Status = status, CreatedDate = DateTime.Now, Category = category });
        }

        // Retrieves all records from the GeoChanges table for a specific user
        public IEnumerable<GeoChange> GetAllGeoChanges(string userId)
        {
            string query = @"SELECT * FROM GeoChanges WHERE UserId = @UserId";
            return _dbConnection.Query<GeoChange>(query, new { UserId = userId });
        }

        // Retrieves a single GeoChange by its unique Id for a specific user
        public GeoChange GetGeoChangeById(int id, string userId)
        {
            string query = "SELECT * FROM GeoChanges WHERE Id = @Id AND UserId = @UserId";
            return _dbConnection.QuerySingleOrDefault<GeoChange>(query, new { Id = id, UserId = userId });
        }

        // Updates an existing GeoChange record in the database based on Id and UserId
        public void UpdateGeoChange(int id, string description, string geoJson, string userId, GeoChangeStatus status, GeoChangeCategory category)
        {
            string query = @"UPDATE GeoChanges 
                     SET Description = @Description, GeoJson = @GeoJson, Status = @Status, Category = @Category 
                     WHERE Id = @Id AND UserId = @UserId";

            _logger.LogInformation("Executing query: " + query);
            _logger.LogInformation($"Parameters - Id: {id}, Description: {description}, GeoJson: {geoJson}, UserId: {userId}, Status: {status}, Category: {category}");

            _dbConnection.Execute(query, new { Id = id, Description = description, GeoJson = geoJson, UserId = userId, Status = status, Category = category });
        }

        public void UpdateGeoChangeAdmin(int id, GeoChangeStatus status, GeoChangeCategory category)
        {
            string query = @"UPDATE GeoChanges 
                     SET   Status = @Status, Category = @Category 
                     WHERE Id = @Id";

            _logger.LogInformation("Executing query: " + query);
            _logger.LogInformation($"Parameters - Id: {id}, Status: {status}, Category: {category}");

            _dbConnection.Execute(query, new { Id = id, Status = status, Category = category });
        }



        // Deletes an existing GeoChange record based on its Id and UserId
        public void DeleteGeoChange(int id, string userId)
        {
            string query = "DELETE FROM GeoChanges WHERE Id = @Id AND UserId = @UserId";
            _dbConnection.Execute(query, new { Id = id, UserId = userId });
        }

        //Søkefunskjon for saksbehandlere
        public IEnumerable SearchGeoChanges(DateTime? fromDate, DateTime? toDate, GeoChangeCategory? category)
        {
            string query = @"SELECT * FROM GeoChanges WHERE 1=1";

            if (fromDate.HasValue)
            {
                query += " AND CreatedDate >= @FromDate";
            }

            if (toDate.HasValue)
            {
                query += " AND CreatedDate <= @ToDate";
            }

            if (category.HasValue)
            {
                query += " AND Category = @Category";
            }

            return _dbConnection.Query(query, new { FromDate = fromDate, ToDate = toDate, Category = category });
        }

    }
}
