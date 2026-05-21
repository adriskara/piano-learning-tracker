namespace PianoLearningTracker.Models
{
    public class DateTimePickerModel
    {
        public string FieldId { get; set; } = null!;
        public string FieldName { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public DateTime? Value { get; set; }
        public bool IncludeTime { get; set; } = false;
        public bool Required { get; set; } = true;
    }
}
