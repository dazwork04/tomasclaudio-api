using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SAPB1SLayerWebAPI.Entities.Main
{
    [Table("JDT1")]
    public class JDT1
    {
        [Key]
        public int TransId { get; set; }
        public int Line_ID { get; set; }
        public int TransType { get; set; }
        public int BaseRef { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime RefDate { get; set; }
        public int Ref1 { get; set; }
        public char DebCred { get; set; }
        public double Debit { get; set; }
        public double Credit { get; set; }
        public double BalDueDeb { get; set; }
        public double BalDueCred { get; set; }
        public string ShortName { get; set; } = string.Empty;
        public string Account { get; set; } = string.Empty;
    }
}
