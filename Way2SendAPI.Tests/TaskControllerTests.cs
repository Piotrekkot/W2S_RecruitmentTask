using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Way2SendApi.Infrastructure.Models;
using Way2SendApi.Infrastructure.Repository.Interfaces;
using Way2SendApi.Infrastructure.Services.Interfaces;
using Way2SendAPI.Controllers;

namespace Way2SendAPI.Tests
{
    public class TaskControllerTests
    {
        private readonly TaskController _taskController;
        private readonly Mock<ITaskRepository> _taskRepositoryMock;
        private readonly Mock<IEmailService> _emailServiceMock;

        public TaskControllerTests()
        {
            _taskRepositoryMock = new Mock<ITaskRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _taskController = new TaskController(_taskRepositoryMock.Object, _emailServiceMock.Object);
        }

        [Fact]
        public async Task GetTasks_ReturnsOkResult()
        {
            //Arrange
            var taskList = new List<TaskItem>
            {
                new TaskItem { Id = 1, Title = "Test Task 1" },
                new TaskItem { Id = 2, Title = "Test Task 2" }
            };
            _taskRepositoryMock.Setup(repo => repo.GetAllTasksAsync()).ReturnsAsync(taskList);
            
            //Act
            var result = await _taskController.GetTasks();
            
            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedTasks = Assert.IsType<List<TaskItem>>(okResult.Value);
            Assert.Equal(2, returnedTasks.Count);
        }

        [Fact]
        public async Task CreateTask_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var task = new TaskItem { Title = "Nowe zadanie w testach" };
            _taskRepositoryMock.Setup(repo => repo.AddTaskAsync(task)).Returns(Task.CompletedTask);

            // Act
            var result = await _taskController.CreateTask(task);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdTask = Assert.IsType<TaskItem>(createdResult.Value);
            Assert.Equal(task.Title, createdTask.Title);
        }
    }
}
