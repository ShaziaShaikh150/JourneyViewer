namespace JourneyViewer.Models
{
    public class LayoutViewModel
    {
        public long LayoutId { get; set; }

        public string? LayoutName { get; set; }

        public string? LayoutJson { get; set; }

        public string? LayoutVersion { get; set; }

        public string? UniqueLayoutName { get; set; }

        public long? MasterId { get; set; }

        public string? MasterName { get; set; }
    }
}