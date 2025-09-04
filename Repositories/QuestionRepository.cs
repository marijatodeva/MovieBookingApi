using Dapper;
using Microsoft.Extensions.Options;
using MovieApi.Models.System;
using MovieApi.Repositories;
using MovieAPI.Models.MovieWeb.Models;

using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieWeb.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly DBSettings _dbSettings;

        public QuestionRepository(IOptions<DBSettings> dbSettings)
        {
            _dbSettings = dbSettings.Value;
        }

        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = @"SELECT id, questiontext AS QuestionText, answertext AS AnswerText, 
                                  createdat AS CreatedAt, category AS Category
                           FROM questions
                           ORDER BY createdat DESC";
            return await conn.QueryAsync<Question>(sql);
        }

        public async Task<Question?> GetByIdAsync(int id)
        {
            using var conn = new NpgsqlConnection(_dbSettings.MoviesDb);
            string sql = @"SELECT id, questiontext AS QuestionText, answertext AS AnswerText, 
                                  createdat AS CreatedAt, category AS Category
                           FROM questions
                           WHERE id = @id";
            return await conn.QueryFirstOrDefaultAsync<Question>(sql, new { id });
        }
    }
}
