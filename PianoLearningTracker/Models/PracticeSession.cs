namespace PianoLearningTracker.Models
{
    // Predstavlja jednu samostalnu vježbu učenika (između satova)
    public class PracticeSession
    {
        // jedinstveni identifikator vježbe
        public int Id { get; set; }

        // datum i vrijeme kada je vježba održana
        public DateTime Date { get; set; }

        // trajanje vježbe u minutama
        public int DurationMinutes { get; set; }

        // bilješke učenika o tijeku vježbe
        public string Notes { get; set; }

        // subjektivna ocjena kvalitete vježbe (1-5)
        public int QualityRating { get; set; }

        // ciljevi koje je učenik postavio za ovu vježbu
        public string Goals { get; set; }

        // strani ključ — koji učenik je vježbao
        public int StudentId { get; set; }

        // strani ključ — koja skladba je vježbana
        public int PieceId { get; set; }

        // navigacijsko svojstvo — cijeli objekt učenika
        public Student Student { get; set; }

        // navigacijsko svojstvo — cijeli objekt skladbe
        public Piece Piece { get; set; }
    }
}
