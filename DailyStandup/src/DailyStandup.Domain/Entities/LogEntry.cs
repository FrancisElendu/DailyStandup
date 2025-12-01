namespace DailyStandup.Domain.Entities
{
    public class LogEntry
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Level { get; set; } = "Information";
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
    }
}
