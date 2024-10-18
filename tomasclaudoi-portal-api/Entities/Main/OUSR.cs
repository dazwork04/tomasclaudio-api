using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAPB1SLayerWebAPI.Entities.Main
{
    [Table("OUSR")]
    public class OUSR
    {
        [Key]
        public int USERID { get; set; }
        public string USERCODE { get; set; } = string.Empty;
    }
}
