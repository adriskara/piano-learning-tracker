namespace PianoLearningTracker.Models.DTOs
{
    public class PieceBriefDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Composer { get; set; } = null!;
        public string Difficulty { get; set; } = null!;
    }

    public class PieceDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Composer { get; set; } = null!;
        public string Difficulty { get; set; } = null!;
        public string? Genre { get; set; }
        public int DurationMinutes { get; set; }
        public int YearComposed { get; set; }
        public string? Description { get; set; }
    }
}
