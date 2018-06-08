using System;
using System.ComponentModel.DataAnnotations;

namespace LogServer.API
{
    public class Log
    {
        [Key]
        public int LogId { get; set; }
        [Required, StringLength(50)]
        public string LogLevel { get; set; }
        [Required]
        public string Message { get; set; }
        public Guid ClientId { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
