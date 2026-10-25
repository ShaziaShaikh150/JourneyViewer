using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JourneyViewer.Models
{
    public class EDSCrmMapping_cs
    {
        public long EDSId { get; set; }

        public long? CrmEDS { get; set; }

        public string? CrmEDSName { get; set; }
    }

    [Table("EDS_CRM_MAPPING")]
    public class EDSCrmMapping
    {
        [Key]
        [Column("EDS_ID")]
        public long EDSId { get; set; }

        [Column("CRM_EDS")]
        public long? CrmEDS { get; set; }

        [Column("CRM_EDS_NAME")]
        public string? CrmEDSName { get; set; }
    }
}
