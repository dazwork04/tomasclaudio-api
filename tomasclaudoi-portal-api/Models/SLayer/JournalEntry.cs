namespace SAPB1SLayerWebAPI.Models.SLayer
{
    public class JournalEntry
    {
        public string ReferenceDate { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
        public string Reference2 { get; set; } = string.Empty;
        //public string? TransactionCode { get; set; }
        //public string? ProjectCode { get; set; }
        public string TaxDate { get; set; } = string.Empty;
        public long JdtNum { get; set; }
        // Indicator ?
        // UseAutoStorno string
        // StornoDate string
        //public string VatDate { get; set; } = string.Empty;

        public int Series { get; set; }
        // StampTax string
        public string DueDate { get; set; } = string.Empty;
        // AutoVAT string
        public long Number { get; set; }
        // FolioNumber ?
        // FolioPrefixString ?
        // ReportEU string
        // Report347 string
        // Printed  string
        // LocationCode ? 
        public string OriginalJournal { get; set; } = string.Empty;
        public long Original { get; set; }
        public string BaseReference { get; set; } = string.Empty;
        // BlockDunningLetter string
        // AutomaticWT string
        // WTSum ?double
        // WTSumSC ?double
        // WTSumFC ?double
        // SignatureInputMessage ?
        // SignatureDigest ?
        // CertificationNumber ? 
        // PrivateKeyVersion ?
        // Corsptivi string
        public string? Reference3 { get; set; }
        // DocumentType ?
        // DeferredTax string
        // BlanketAgreementNumber ?
        // OperationCode ?
        // ResidenceNumberType string
        // ECDPostingType string
        // ExposedTransNumber ?
        // PointOfIssueCode ?
        // Letter ?
        // FolioNumberFrom ?
        // FolioNumberTo ?
        // IsCostCenterTransfer string
        // ReportingSectionControlStatementVAT ?
        // ExcludeFromTaxReportControlStatementVAT string
        // SAPPassport ?
        // Cig ?
        // Cup ?
        // AdjustTransaction string
        // AttachmentEntry ?
        public List<JournalEntryLine> JournalEntryLines { get; set; } = [];

    }

    public class JournalEntryLine
    {
        public int Line_ID { get; set; }
        public string AccountCode { get; set; } = string.Empty;
        public double Debit { get; set; }
        public double Credit { get; set; }
        //public double FCDebit { get; set; }
        //public double FCCredit { get; set; }
        // FCCurrency ?
        public string ShortName { get; set; } = string.Empty;
        public string ContraAccount { get; set; } = string.Empty;
        public string LineMemo { get; set; } = string.Empty;
        public string Reference1 { get; set; } = string.Empty;
        public string Reference2 { get; set; } = string.Empty;
        // ProjectCode ?
        // CostingCode ?
        // BaseSum double
        public string? TaxGroup { get; set; }
        // DebitSys double
        // CreditSys double
        // VatDate string
        // VatLine ?
        // SystemBaseAmount ?
        // GrossValue ?
        //public string AdditionalReference { get; set; } = string.Empty;
        // CheckAbs ?
        public string? CostingCode { get; set; }
        public string? CostingCode2 { get; set; }
        public string? CostingCode3 { get; set; }
        public string? CostingCode4 { get; set; }
        // TaxCode ?
        // TaxPostAccount string
        // CostingCode5 string
        // LocationCode ?
        public string ControlAccount { get; set; } = string.Empty;
        // EqualizationTaxAmount ?
        // SystemEqualizationTaxAmount ?
        // TotalTax double
        // SystemTotalTsx double
        // WTLiable string
        // WTRow string
        // PaymentBlock string
        // BlockReason ?
        //public string FederalTaxID { get; set; } = string.Empty;
        // BPLID ?
        // BPLName string
        // VATRegNum ?
        // PaymentOrdered string
        // ExposedTransNumber ?
        // DocumentArray int
        // DocumentLine int
        // CostElementCode ?
        // Cig
        // Cup
        // IncomeClassificationCategory ?
        // IncomeClassificationType ?
        // ExpenseClassificationCategory ?
        // ExpenseClassificationType ?
        // VATClassificationCategory
        // VATClassificationType
        // VATExemptionCause

    }

}
