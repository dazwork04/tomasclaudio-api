using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAPB1SLayerWebAPI.Entities.Main
{
    [Table("OWDD")]
    public class OWDD
    {
        [Key]
        public int WddCode { get; set; }
        public int DraftEntry { get; set; }
        public int? DocEntry { get; set; }
        public string ObjType { get; set; } = string.Empty;
        public char ProcesStat { get; set; }
    }
}
