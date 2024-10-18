namespace SAPB1SLayerWebAPI.Models
{
    public class PaymentOtherData
    {
        public int TransId { get; set; }
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public decimal TotalAmountDue { get; set; } // DocTotal
        public decimal OpenBalance { get; set; } // OpenBal
        public decimal PaymentOnAccount { get; set; } // NoDocSum
    }
}
