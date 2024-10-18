using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SAPB1SLayerWebAPI.Entities.Sbo
{
    [Table("SRGC")]
    public class SRGC
    {
        [Key]
        public string DbName { get; set; } = string.Empty;
        public string CmpName { get; set; } = string.Empty;
    }
}
