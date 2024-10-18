using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAPB1SLayerWebAPI.Entities.Main
{
    [Table("OACT")]
    public class OACT
    {
        [Key]
        public string AcctCode { get; set; } = string.Empty;
        public string AcctName { get; set; } = string.Empty;
        public char LocManTran { get; set; }
    }
}
