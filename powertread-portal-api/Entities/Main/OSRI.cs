using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace SAPB1SLayerWebAPI.Entities.Main
{
    [Table("OSRI")]
    public class OSRI
    {
        [Key]
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string IntrSerial { get; set; } = string.Empty;
        public string WhsCode { get; set; } = string.Empty;
        public int Status { get; set; }
        //[Precision(19, 6)]
        //public decimal Quantity { get; set; }
        public string? SuppSerial { get; set; }
        public DateTime? ExpDate { get; set; }
        public DateTime? PrdDate { get; set; }
        public DateTime? InDate { get; set; }

    }
}
