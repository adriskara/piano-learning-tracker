namespace PianoLearningTracker.Models.DTOs
{
    public class PracticeSessionDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int DurationMinutes { get; set; }
        public string? Notes { get; set; }
        public int QualityRating { get; set; }
        public string? Goals { get; set; }
        public int StudentId { get; set; }
        public StudentBriefDTO Student { get; set; } = null!;
        public int PieceId { get; set; }
        public PieceBriefDTO Piece { get; set; } = null!;
    }
}
