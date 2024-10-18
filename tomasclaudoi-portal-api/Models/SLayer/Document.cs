namespace SAPB1SLayerWebAPI.Models.SLayer
{
    public class Document
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string DocType { get; set; } = string.Empty;
        public string DocDate { get; set; } = string.Empty;
        public string DocDueDate { get; set; } = string.Empty;
        public string CardCode { get; set; } = string.Empty;
        public string CardName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string NumAtCard { get; set; } = string.Empty;
        public double DocRate { get; set; }
        public string DocCurrency { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public int PaymentGroupCode { get; set; }
        public int SalesPersonCode { get; set; }
        public int TransportationCode { get; set; }
        public int? ContactPersonCode { get; set; }
        public int Series { get; set; }
        public string TaxDate { get; set; } = string.Empty;
        public double DiscountPercent { get; set; } = 0;
        public double VatSum { get; set; }
        public double DocTotal { get; set; }
        public double PaidToDate { get; set; }
        public int? DocumentsOwner { get; set; }
        public string Address2 { get; set; } = string.Empty;
        public string? PayToCode { get; set; }
        public string PriceMode { get; set; } = string.Empty;
        public int? UserSign { get; set; }
        public List<DownPaymentsToDraw> DownPaymentsToDraw { get; set; } = [];
        public List<DocumentLines> DocumentLines { get; set; } = [];

    }

    public class DocumentLines
    {
        public int LineNum { get; set; }
        public string? ItemCode { get; set; }
        public string ItemDescription { get; set; } = string.Empty;
        public double Quantity { get; set; }
        public double RemainingOpenQuantity { get; set; }
        public decimal? DiscountPercent { get; set; }
        public string? WarehouseCode { get; set; }
        public string? AccountCode { get; set; }
        public string VatGroup { get; set; } = string.Empty;
        public double TaxTotal { get; set; }
        public int BaseType { get; set; }
        public int? BaseEntry { get; set; }
        public int? BaseLine { get; set; }
        public double UnitPrice { get; set; }
        public double GrossPrice { get; set; }
        public double LineTotal { get; set; }
        public double GrossTotal { get; set; }
        public string? Text { get; set; }
        public string? ItemDetails { get; set; }
        public int UoMEntry { get; set; }
        public string? UoMCode { get; set; }
        public int TargetType { get; set; }
        public int? TargetEntry { get; set; }
        public string? VendorNum { get; set; }
        public string? ShipDate { get; set; }
        public string? CostingCode { get; set; }
        public string? CostingCode2 { get; set; }
        public string? CostingCode3 { get; set; }
        public string? CostingCode4 { get; set; }
        public string? CostingCode5 { get; set; }
    }

    public class DownPaymentsToDraw
    {
        public int DocEntry { get; set; }
        public double AmountToDraw { get; set; }
        public double? AmountToDrawFC { get; set; }
        public double Tax { get; set; }
        public double? TaxFC { get; set; }
        public double GrossAmountToDraw { get; set; }
        public double GrossAmountToDrawFC { get; set; }
        public List<DownPaymentsToDrawDetail> DownPaymentsToDrawDetails { get; set; } = [];
    }

    public class DownPaymentsToDrawDetail
    {
        public int DocInternalID { get; set; }
        public int DocEntry { get; set; }
        public string? VatGroupCode { get; set; }
        public double VatPercent { get; set; }
        public double AmountToDraw { get; set; }
        public double? AmountToDrawFC { get; set; }
        public double Tax { get; set; }
        public double? TaxFC { get; set; }
        public double GrossAmountToDraw { get; set; }
        public double GrossAmountToDrawFC { get; set; }
        public string LineType { get; set; } = string.Empty;
    }
}
