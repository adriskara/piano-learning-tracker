using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace PianoLearningTracker.Models
{
    public class ApplicationUser : IdentityUser
    {
        public int? StudentId { get; set; }
        public Student? Student { get; set; }

        public int? TeacherId { get; set; }
        public Teacher? Teacher { get; set; }

        [Required]
        [StringLength(11, MinimumLength = 11)]
        [RegularExpression("^[0-9]*$")]
        public string OIB { get; set; } = string.Empty;
    }
}
