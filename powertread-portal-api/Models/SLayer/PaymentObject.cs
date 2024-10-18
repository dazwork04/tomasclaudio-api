namespace SAPB1SLayerWebAPI.Models.SLayer
{
    public class PaymentObject
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
        public string? U_Payee { get; set; }
        public List<PaymentCheck> PaymentChecks { get; set; } = [];
        public List<PaymentInvoice> PaymentInvoices { get; set; } = [];
        public List<PaymentCreditCard> PaymentCreditCards { get; set; } = [];
        public List<PaymentAccount> PaymentAccounts { get; set; } = [];
    }

    public class PaymentCheck
    {
        public int LineNum { get; set; }
        public string DueDate { get; set; } = string.Empty;
        public int CheckNumber { get; set; }
        public string BankCode { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
        public string AccounttNum { get; set; } = string.Empty;
        //public string? Details { get; set; }
        //public string Trnsfrable { get; set; } = string.Empty;
        public double CheckSum { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;
        public int? CheckAbsEntry { get; set; }
        public string CheckAccount { get; set; } = string.Empty;
        public string ManualCheck { get; set; } = string.Empty;
    }

    public class PaymentInvoice
    {
        public int LineNum { get; set; }
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public double SumApplied { get; set; }
        //public double AppliedFC { get; set; }
        //public double AppliedSys { get; set; }
        public int DocLine { get; set; }
        public string InvoiceType { get; set; } = string.Empty;
        public double DiscountPercent { get; set; }
        public double PaidSum { get; set; }
        //public int InstallmentId { get; set; }
        //public double WitholdingTaxApplied { get; set; }
        //public double WitholdingTaxAppliedFC { get; set; }
        //public double WitholdingTaxAppliedSC { get; set; }
        public double TotalDiscount { get; set; }
        public string? DistributionRule { get; set; }
        public string? DistributionRule2 { get; set; }
        public string? DistributionRule3 { get; set; }
        public string? DistributionRule4 { get; set; }
        public string? DistributionRule5 { get; set; }
    }

    public class PaymentAccount
    {
        public int LineNum { get; set; }
        public string AccountCode { get; set; } = string.Empty;
        public double SumPaid { get; set; }
        public string Decription { get; set; } = string.Empty;
        public string VatGroup { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public double GrossAmount { get; set; }
        public double? VatAmount { get; set; }
        public string? ProfitCenter { get; set; }
        public string? ProfitCenter2 { get; set; }
        public string? ProfitCenter3 { get; set; }
        public string? ProfitCenter4 { get; set; }
        public string? ProfitCenter5 { get; set; }

    }

    public class PaymentCreditCard
    {
        public double AdditionalPaymentSum { get; set; }
        public string CardValidUntil { get; set; } = string.Empty;
        public string CreditAcct { get; set; } = string.Empty;
        public int CreditCard { get; set; }
        public string CreditCardNumber { get; set; } = string.Empty;
        //public string CreditCur { get; set; } = string.Empty;
        //public double CreditRate { get; set; }
        public double CreditSum { get; set; }
        public string CreditType { get; set; } = string.Empty;
        //public string FirstPaymentDue { get; set; } = string.Empty;
        public double FirstPaymentSum { get; set; }
        public int LineNum { get; set; }
        //public int NumOfCreditPayments { get; set; }
        public int NumOfPayments { get; set; }
        public string OwnerIdNum { get; set; } = string.Empty;
        public string OwnerPhone { get; set; } = string.Empty;
        //public string SplitPayments { get; set; } = string.Empty;
        public string VoucherNum { get; set; } = string.Empty;
    }
}
