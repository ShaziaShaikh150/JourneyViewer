using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JourneyViewer.Models
{
    [Table("FLOW")]
    public class Flow
    {
        [Key]
        [Column("FLOWID")]
        public long FlowId { get; set; }

        [Column("FLOWUNIQUENAME")]
        public string? FlowUniqueName { get; set; }

        [Column("FLOWTYPE")]
        public int FlowType { get; set; }
    }
}