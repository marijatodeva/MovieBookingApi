using Dapper;
using Microsoft.Extensions.Options;
using MovieApi.Models;
using MovieApi.Models.System;
using Npgsql;

namespace MovieAPI.Repositories
{
    public class AppUserRepository : IAppUserRepository
    {
        private readonly DBSettings _dbSettings;

        public AppUserRepository(IOptions<DBSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
        }

        // Return all users
        public async Task<IEnumerable<AppUser>> GetAllUsers()
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = @"SELECT id, name AS ""FullName"", phone, username, password, active, role
                           FROM ""AppUser""";
            return await conn.QueryAsync<AppUser>(sql);
        }

        // Return user by ID
        public async Task<AppUser> GetUserById(long id)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = @"SELECT id, name AS ""FullName"", phone, username, password, active, role
                           FROM ""AppUser"" WHERE id = @id";
            return await conn.QueryFirstOrDefaultAsync<AppUser>(sql, new { id });
        }

        // Create new user
        public async Task<int> CreateUser(RegisterRequest user)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = @"INSERT INTO ""AppUser"" (name, phone, username, password, active, role)
                           VALUES (@Name, @Phone, @Username, @Password, @Active, @Role)";
            var parameters = new
            {
                Name = user.FullName,
                Phone = user.Phone,
                Username = user.Username,
                Password = user.Password,
                Active = true,
                Role = "User" // default role
            };
            return await conn.ExecuteAsync(sql, parameters);
        }

        // Update existing user
        public async Task<int> UpdateUser(long id, AppUser user)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = @"UPDATE ""AppUser""
                           SET name = @Name,
                               phone = @Phone,
                               username = @Username,
                               password = @Password,
                               active = @Active,
                               role = @Role
                           WHERE id = @Id";
            return await conn.ExecuteAsync(sql, new
            {
                Id = id,
                user.FullName,
                user.Phone,
                user.Username,
                user.Password,
                user.Active,
                user.Role
            });
        }

        // Delete user by ID
        public async Task<int> DeleteUser(long id)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = @"DELETE FROM ""AppUser"" WHERE id = @id";
            return await conn.ExecuteAsync(sql, new { id });
        }

        // Get user by username and password (for login)
        public async Task<AppUser> GetUserByUsernameAndPassword(string username, string password)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = @"
                SELECT 
                    id, 
                    username, 
                    name AS ""FullName"", 
                    phone, 
                    password, 
                    active,
                    role
                FROM ""AppUser""
                WHERE username = @username AND password = @password";
            return await conn.QueryFirstOrDefaultAsync<AppUser>(sql, new { username, password });
        }

        // Get user by username
        public async Task<AppUser> GetUserByUsername(string username)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = @"
                SELECT 
                    id, 
                    username, 
                    name AS ""FullName"", 
                    phone, 
                    password, 
                    active,
                    role
                FROM ""AppUser""
                WHERE username = @username";
            return await conn.QueryFirstOrDefaultAsync<AppUser>(sql, new { username });
        }
    }
}
