using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAPB1SLayerWebAPI.Entities.Main
{
    [Table("OVPM")]
    public class OVPM
    {
        [Key]
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public int TransId { get; set; }
        public int? DraftKey { get; set; }
        [Precision(19, 6)]
        public decimal OpenBal { get; set; }
        [Precision(19, 6)]
        public decimal NoDocSum { get; set; }
        [Precision(19, 6)]
        public decimal DocTotal { get; set; }
    }
}
