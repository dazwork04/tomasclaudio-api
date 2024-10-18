using Microsoft.EntityFrameworkCore;

namespace SAPB1SLayerWebAPI.Models.SLayer
{
    public class DocumentArchived
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
        public double DocTotal { get; set; }
        public double DocTotalFC { get; set; }
        public string DocCurrency { get; set; } = string.Empty;
        public double DocRate { get; set; }
        public string Comments { get; set; } = string.Empty;
        public int PaymentGroupCode { get; set; }
        public int SalesPersonCode { get; set; }
        public int TransportationCode { get; set; }
        public int? ContactPersonCode { get; set; }
        public int Series { get; set; }
        public string TaxDate { get; set; } = string.Empty;
        public decimal? DiscountPercent { get; set; }
        public double VatSum { get; set; }
        public double VatSumFC { get; set; }
        public int? DocumentsOwner { get; set; }
        public string Address2 { get; set; } = string.Empty;
        public string DocumentStatus { get; set; } = string.Empty;
        public string? PayToCode { get; set; }
        public double TotalDiscount { get; set; }
        public string Cancelled { get; set; } = string.Empty;
        public string? PriceMode { get; set; }
        //
        //public string ControlAccount { get; set; } = string.Empty;
        //public string DocumentDelivery { get; set; } = string.Empty;
        //public string ApplyCurrentVATRatesForDownPaymentsToDraw { get; set; } = string.Empty;
        //public string DownPaymentStatus { get; set; } = string.Empty;
        //public string StartFrom { get; set; } = string.Empty;
        //public int ExtraMonth { get; set; }
        //public int ExtraDays { get; set; }
        public List<DocumentLinesArchived> DocumentLines { get; set; } = new();
        //public object TaxExtension { get; set; } = new { };
    }

    public class DocumentLinesArchived
    {
        public int LineNum { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string ItemDescription { get; set; } = string.Empty;
        public double Quantity { get; set; }
        public decimal? DiscountPercent { get; set; }
        public string WarehouseCode { get; set; } = string.Empty;
        public string AccountCode { get; set; } = string.Empty;
        public string VatGroup { get; set; } = string.Empty;
        public int BaseType { get; set; }
        public int? BaseEntry { get; set; }
        public int? BaseLine { get; set; }
        public double NetTaxAmount { get; set; }
        public double NetTaxAmountFC { get; set; }
        public double TaxTotal { get; set; }
        public int VisualOrder { get; set; }
        public double UnitPrice { get; set; }
        public string LineStatus { get; set; } = string.Empty;
        public string? Text { get; set; }
        public string? ItemDetails { get; set; }
        public int DocEntry { get; set; }
        public int UoMEntry { get; set; }
        public string UoMCode { get; set; } = string.Empty;
        public string ItemType { get; set; } = string.Empty;
        public double PriceAfterVat { get; set; }
        public double LineTotal { get; set; }
        //public double GrossBuyPrice { get; set; }
        //public double GrossProfitTotalBasePrice { get; set; }

    }
}
