using SAPB1SLayerWebAPI.Models.SLayer;

namespace SAPB1SLayerWebAPI.Models
{
    public class PostPaymentObject
    {
        public int DocNum { get; set; }
        public string DocType { get; set; } = string.Empty;
        public string DocDate { get; set; } = string.Empty;
        public string CardCode { get; set; } = string.Empty;
        public string CardName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? CashAccount { get; set; }
        public string DocCurrency { get; set; } = string.Empty;
        public double CashSum { get; set; }
        public string? CheckAccount { get; set; }
        public string? TransferAccount { get; set; }
        public double TransferSum { get; set; }
        public string? TransferDate { get; set; }
        public string? TransferReference { get; set; }
        public string LocalCurrency { get; set; } = string.Empty;
        public double DocRate { get; set; }
        public string Reference1 { get; set; } = string.Empty;
        public string? Reference2 { get; set; }
        public string CounterReference { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public string? JournalRemarks { get; set; }
        public string SplitTransaction { get; set; } = string.Empty;
        public int? ContactPersonCode { get; set; }
        public string ApplyVAT { get; set; } = string.Empty;
        public string? TaxDate { get; set; }
        public int Series { get; set; }
        public string? BankCode { get; set; }
        public string? BankAccount { get; set; }
        public double DiscountPercent { get; set; }
        public string? ProjectCode { get; set; }
        public string CurrencyIsLocal { get; set; } = string.Empty;
        public string? WTCode { get; set; }
        public double WTAmount { get; set; }
        public string? WTAccount { get; set; }
        public double WTTaxableAmount { get; set; }
        public string Proforma { get; set; } = string.Empty;
        public string? PayToBankCode { get; set; }
        public string? PayToBankBranch { get; set; }
        public string? PayToBankAccountNo { get; set; }
        public string? PayToCode { get; set; }
        public string? PayToBankCountry { get; set; }
        public string IsPayToBank { get; set; } = string.Empty;
        public int DocEntry { get; set; }
        public string? TaxGroup { get; set; }
        public string? VatDate { get; set; }
        public string TransactionCode { get; set; } = string.Empty;
        public string DocObjectCode { get; set; } = string.Empty;
        public string? DueDate { get; set; }
        public string Cancelled { get; set; } = string.Empty;
        public string ControlAccount { get; set; } = string.Empty;
        public string? BPLID { get; set; }
        public string? BPLName { get; set; }
        public string? VATRegNum { get; set; }
        public List<PaymentCheck> PaymentChecks { get; set; } = [];
        public List<PaymentInvoice> PaymentInvoices { get; set; } = [];
        public List<PaymentCreditCard> PaymentCreditCards { get; set; } = [];
        public List<PostPaymentAccount> PaymentAccounts { get; set; } = [];
    }

    public class PostPaymentAccount
    {
        public int LineNum { get; set; }
        public string AccountCode { get; set; } = string.Empty;
        public double SumPaid { get; set; }
        public string Decription { get; set; } = string.Empty;
        public string VatGroup { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public double GrossAmount { get; set; }

    }
}
