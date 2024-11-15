using Dapper;
using System.Collections.Generic;
using System.Data;
using Kartverket.Models;

namespace Kartverket.Data
{
    public class GeoChangeService
    {
        private readonly IDbConnection _dbConnection;

        public GeoChangeService(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // Inserts a new record into the GeoChanges table
        public void AddGeoChange(string description, string geoJson, string userId)
        {
            string query = @"INSERT INTO GeoChanges (Description, GeoJson, UserId) 
                             VALUES (@Description, @GeoJson, @UserId)";
            _dbConnection.Execute(query, new { Description = description, GeoJson = geoJson, UserId = userId });
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
        public void UpdateGeoChange(int id, string description, string geoJsonData, string userId)
        {
            string query = @"UPDATE GeoChanges 
                             SET Description = @Description, GeoJson = @GeoJson 
                             WHERE Id = @Id AND UserId = @UserId";
            Console.WriteLine(query);
            _dbConnection.Execute(query, new { Id = id, Description = description, GeoJson = geoJsonData, UserId = userId });
        }

        // Deletes an existing GeoChange record based on its Id and UserId
        public void DeleteGeoChange(int id, string userId)
        {
            string query = "DELETE FROM GeoChanges WHERE Id = @Id AND UserId = @UserId";
            _dbConnection.Execute(query, new { Id = id, UserId = userId });
        }
    }
}
