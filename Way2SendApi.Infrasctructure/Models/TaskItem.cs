using System.ComponentModel.DataAnnotations;

namespace Way2SendApi.Infrastructure.Models
{
    public sealed class TaskItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title required")]
        [StringLength(100, ErrorMessage = "longer than 100 characters is not valid")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "DueDate required")]
        public DateTime DueDate { get; set; }

        public bool IsCompleted { get; set; }
        public bool Removed { get; set; } = false; // pole dodane na potrzeby trzymania rekordów w bazie

        public TaskItem()
        {
            
        }
    }
}
