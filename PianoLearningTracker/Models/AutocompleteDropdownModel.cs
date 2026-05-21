namespace PianoLearningTracker.Models
{
    public class AutocompleteDropdownModel
    {
        public string FieldId { get; set; } = null!;
        public string FieldName { get; set; } = null!;
        public string DisplayFieldId { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Endpoint { get; set; } = null!;
        public int? SelectedId { get; set; }
        public string? SelectedText { get; set; }
        public bool Required { get; set; } = true;
    }
}
