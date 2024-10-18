namespace SAPB1SLayerWebAPI.Models
{
    public class DownPayment
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        //public string Remarks { get; set; } = string.Empty;
        public string TaxCode { get; set; } = string.Empty;
        public double NetAmountToDraw { get; set; }
        public double TaxAmountToDraw { get; set; }
        public double GrossAmountToDraw { get; set; }
        public double OpenNetAmount { get; set; }
        public double OpenTaxAmount { get; set; }
        public double OpenGrossAmount { get; set; }
        public string DocumentDate { get; set; } = string.Empty;
        public List<DownPayment> Lines { get; set; } = [];
    }
}
