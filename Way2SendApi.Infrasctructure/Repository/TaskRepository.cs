using Dapper;
using System.Data;
using Way2SendApi.Infrastructure.Models;
using Way2SendApi.Infrastructure.Repository.Interfaces;

namespace Way2SendApi.Infrastructure.Repository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IDbConnection _db;

        public TaskRepository(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            var sql = "SELECT * FROM Tasks WHERE Removed = @Removed";
            return await _db.QueryAsync<TaskItem>(sql, new { Removed = false });
        }

        public async Task<TaskItem> GetTaskByIdAsync(int id)
        {
            var sql = "SELECT * FROM Tasks WHERE Id = @Id";
            return await _db.QueryFirstOrDefaultAsync<TaskItem>(sql, new { Id = id });
        }

        public async Task AddTaskAsync(TaskItem task)
        {
            var sql = "INSERT INTO Tasks (Title, Description, DueDate, IsCompleted) VALUES (@Title, @Description, @DueDate, @IsCompleted)";
            await _db.ExecuteAsync(sql, task);
        }

        public async Task UpdateTaskAsync(TaskItem task)
        {
            var sql = "UPDATE Tasks SET Title = @Title, Description = @Description, DueDate = @DueDate, IsCompleted = @IsCompleted WHERE Id = @Id";
            await _db.ExecuteAsync(sql, task);
        }

        public async Task DeleteTaskAsync(int id)
        {
            var sql = "UPDATE Tasks SET Removed = @Removed WHERE Id = @Id";
            await _db.ExecuteAsync(sql, new { Removed = true, Id = id });
        }
    }
}
