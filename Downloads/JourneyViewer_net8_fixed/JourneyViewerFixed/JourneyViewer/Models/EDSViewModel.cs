namespace JourneyViewer.Models
{
    public class EDSViewModel
    {
        public long EDSId { get; set; }
        public string EDSName { get; set; }
        public string DocumentName { get; set; }

        public string? displayName { get; set; }

        public string? type { get; set; }

        public long? CrmEDS { get; set; }

        public string? CrmEDSName { get; set; }
    }
}
