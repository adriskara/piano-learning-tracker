using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PianoLearningTracker.Models
{
    public class PieceAttachment
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Piece")]
        public int PieceId { get; set; }
        public virtual Piece Piece { get; set; } = null!;

        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public long FileSize { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
