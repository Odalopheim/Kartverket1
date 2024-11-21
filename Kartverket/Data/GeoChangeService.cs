using Dapper;
using System.Collections.Generic;
using System.Data;
using Kartverket.Models;
using System;

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
        public void AddGeoChange(string description, string geoJson, string userId, GeoChangeStatus status, GeoChangeCategory category, string saksbehandler)
        {
            string query = @"INSERT INTO GeoChanges (Description, GeoJson, UserId, Status, CreatedDate, Category, Saksbehandler) 
                             VALUES (@Description, @GeoJson, @UserId, @Status, @CreatedDate, @Category, @Saksbehandler)";
            _dbConnection.Execute(query, new { Description = description, GeoJson = geoJson, UserId = userId, Status = status, CreatedDate = DateTime.Now, Category = category, Saksbehandler = saksbehandler });
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
        public void UpdateGeoChange(int id, string description, string geoJson, string userId, GeoChangeStatus status, GeoChangeCategory category, string saksbehandler)
        {
            string query = @"UPDATE GeoChanges 
                     SET Description = @Description, GeoJson = @GeoJson, Status = @Status, Category = @Category, Saksbehandler = @Saksbehandler 
                     WHERE Id = @Id AND UserId = @UserId";
            _dbConnection.Execute(query, new { Id = id, Description = description, GeoJson = geoJson, UserId = userId, Status = status, Category = category, Saksbehandler = saksbehandler });
        }


        // Deletes an existing GeoChange record based on its Id and UserId
        public void DeleteGeoChange(int id, string userId)
        {
            string query = "DELETE FROM GeoChanges WHERE Id = @Id AND UserId = @UserId";
            _dbConnection.Execute(query, new { Id = id, UserId = userId });
        }

        //Søkefunskjon for saksbehandlere
        public IEnumerable<GeoChange> SearchGeoChanges(DateTime? fromDate, DateTime? toDate, GeoChangeCategory? category)
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

            return _dbConnection.Query<GeoChange>(query, new { FromDate = fromDate, ToDate = toDate, Category = category });
        }

    }
}
