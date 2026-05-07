using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PianoLearningTracker.Models
{
    // Predstavlja jednu samostalnu vježbu učenika (između satova)
    public class PracticeSession
    {
        // jedinstveni identifikator vježbe
        [Key]
        public int Id { get; set; }

        // datum i vrijeme kada je vježba održana
        public DateTime Date { get; set; }

        // trajanje vježbe u minutama
        public int DurationMinutes { get; set; }

        // bilješke učenika o tijeku vježbe
        public string? Notes { get; set; }

        // subjektivna ocjena kvalitete vježbe (1-5)
        public int QualityRating { get; set; }

        // ciljevi koje je učenik postavio za ovu vježbu
        public string? Goals { get; set; }

        // strani ključ — koji učenik je vježbao
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        // strani ključ — koja skladba je vježbana
        [ForeignKey("Piece")]
        public int PieceId { get; set; }

        // navigacijsko svojstvo — cijeli objekt učenika
        public virtual Student Student { get; set; } = null!;

        // navigacijsko svojstvo — cijeli objekt skladbe
        public virtual Piece Piece { get; set; } = null!;
    }
}
