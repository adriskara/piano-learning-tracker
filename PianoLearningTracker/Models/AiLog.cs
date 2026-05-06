namespace PianoLearningTracker.Models
{
    public class AiLog
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Role { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Model { get; set; }
        public int? InputTokens { get; set; }
        public int? OutputTokens { get; set; }
        public string? SessionId { get; set; }
        public Dictionary<string, string>? Metadata { get; set; }
    }
}
