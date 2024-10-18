namespace SAPB1SLayerWebAPI.Models
{
    public class PaymentInvoiceData
    {
        public bool Checked { get; set; }
        public int LineNum { get; set; }
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string NumAtCard { get; set; } = string.Empty;
        public string DocType { get; set; } = string.Empty;
        public string DocDate { get; set; } = string.Empty;
        public int OverdueDays { get; set; }
        public double Total { get; set; }
        public double BalanceDue { get; set; }
        public double TotalPayment { get; set; }
    }
}
