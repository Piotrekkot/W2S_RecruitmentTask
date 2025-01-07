using Microsoft.Extensions.Logging;
using Way2SendApi.Infrastructure.Repository.Interfaces;
using Way2SendApi.Infrastructure.Services.Interfaces;

namespace Way2SendApi.Infrastructure.Services
{
    public class RemindService : IRemindService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly ILogger<RemindService> _logger;
        private readonly IEmailService _emailService;

        public RemindService(ITaskRepository taskRepository, ILogger<RemindService> logger, IEmailService emailService)
        {
            _taskRepository = taskRepository;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task CheckForDueTasks()
        {
            var tasks = await _taskRepository.GetAllTasksAsync();
            var dueTasks = tasks.Where(task => task.DueDate > DateTime.Now && task.DueDate <= DateTime.Now.AddHours(1) && !task.IsCompleted).ToList();


            //dueTasks.ForEach(task => _logger.LogWarning($"Przypomnienie: Zadanie '{task.Title}' będzie aktywne do {task.DueDate}."));
            //dla widoczności kodu wraz z ustawieniamirobię w pętli foreach

            foreach (var task in dueTasks)
            {
                //log
                _logger.LogWarning($"Przypomnienie: Zadanie '{task.Title}' będzie aktywne do {task.DueDate}.");
                //mail
                var subject = $"Przypomnienie o zadaniu: {task.Title}";
                var body = $"Zadanie '{task.Title}' jest aktywne do {task.DueDate}. Opis: {task.Description}";

                //odkomentowac w przypadku testow mailservice
                //await _emailService.SendEmailAsync("odbiorca@example.com", subject, body);
            }

        }
    }
}
