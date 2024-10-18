using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SAPB1SLayerWebAPI.Entities.Main
{
    [Table("OPDF")]
    public class OPDF
    {
        [Key]
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public int? TransId { get; set; }
        public int ObjType { get; set; }
        [Precision(19, 6)]
        public decimal OpenBal { get; set; }
        [Precision(19, 6)]
        public decimal NoDocSum { get; set; }
        [Precision(19, 6)]
        public decimal DocTotal { get; set; }
        public DateTime DocDate { get; set; }
    }
}
