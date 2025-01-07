using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Way2SendApi.Infrastructure.Models;
using Way2SendApi.Infrastructure.Repository.Interfaces;
using Way2SendApi.Infrastructure.Services.Interfaces;

namespace Way2SendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IEmailService _emailService;

        public TaskController(ITaskRepository taskRepository, IEmailService emailService)
        {
            _taskRepository = taskRepository;
            _emailService = emailService;
        }

        /// <summary>
        /// Pobiera listę wszystkich zadań.
        /// </summary>
        /// <returns>Lista wszystkich zadań.</returns>
        /// <response code="200">Zwraca listę zadań.</response>
        // GET: api/tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
        {
            var tasks = await _taskRepository.GetAllTasksAsync();
            return Ok(tasks);
        }

        /// <summary>
        /// Pobiera zadanie o określonym identyfikatorze.
        /// </summary>
        /// <param name="id">Identyfikator zadania.</param>
        /// <returns>Zadanie o podanym identyfikatorze.</returns>
        /// <response code="200">Zwraca zadanie o podanym identyfikatorze.</response>
        /// <response code="404">Nie znaleziono zadania o podanym identyfikatorze.</response>
        // GET: api/tasks/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTask(int id)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return Ok(task);
        }

        /// <summary>
        /// Tworzy nowe zadanie.
        /// </summary>
        /// <param name="task">Obiekt zadania do utworzenia.</param>
        /// <returns>Utworzone zadanie.</returns>
        /// <response code="201">Zadanie zostało pomyślnie utworzone.</response>
        /// <response code="400">Wystąpił błąd walidacji danych wejściowych.</response>
        // POST: api/tasks
        [HttpPost]
        public async Task<ActionResult<TaskItem>> CreateTask([FromBody] TaskItem task)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _taskRepository.AddTaskAsync(task);
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        /// <summary>
        /// Aktualizuje istniejące zadanie.
        /// </summary>
        /// <param name="id">Identyfikator zadania do zaktualizowania.</param>
        /// <param name="task">Obiekt zadania z zaktualizowanymi danymi.</param>
        /// <response code="204">Zadanie zostało pomyślnie zaktualizowane.</response>
        /// <response code="400">Wystąpił błąd walidacji lub niezgodność identyfikatorów.</response>
        /// <response code="404">Nie znaleziono zadania o podanym identyfikatorze.</response>
        // PUT: api/tasks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskItem task)
        {
            if (id != task.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _taskRepository.UpdateTaskAsync(task);
            return NoContent();
        }

        /// <summary>
        /// Usuwa zadanie o określonym identyfikatorze.
        /// </summary>
        /// <param name="id">Identyfikator zadania do usunięcia.</param>
        /// <response code="204">Zadanie zostało pomyślnie usunięte.</response>
        /// <response code="404">Nie znaleziono zadania o podanym identyfikatorze.</response>
        // DELETE: api/tasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            await _taskRepository.DeleteTaskAsync(id);
            return NoContent();
        }
        /// <summary>
        /// Wysyła przypomnienie e-mail o zadaniu.
        /// </summary>
        /// <param name="to">Adres e-mail odbiorcy.</param>
        /// <param name="taskTitle">Tytuł zadania, o którym przypominamy.</param>
        /// <returns>Task operacji wysyłania maila.</returns>
        /// <remarks>
        /// Przykład wywołania:
        ///
        ///     await SendReminderEmail("example@example.com", "Nazwa zadania");
        ///
        /// Metoda tworzy wiadomość e-mail z tematem "Przypomnienie o zadaniu" oraz treścią
        /// informującą o zbliżającym się terminie zadania. Wykorzystuje usługę e-mail do wysłania wiadomości.
        /// </remarks>
        [HttpPost("reminder")]
        public async Task SendReminderEmail(string to, string taskTitle)
        {
            string subject = "Przypomnienie o zadaniu";
            string body = $"Zadanie '{taskTitle}' jest już bliskie terminu!";
            await _emailService.SendEmailAsync(to, subject, body);
        }
    }
}
