namespace SAPB1SLayerWebAPI.Models.SLayer
{
    public class GLAccountDetermination
    {
        public int AbsoluteEntry { get; set; }
        public string BeginningofFinancialYear { get; set; } = string.Empty;
        public string PeriodCategory { get; set; } = string.Empty;
        public string PeriodName { get; set; } = string.Empty;
        public string? DebitorsFollowUpAccount { get; set; }
        public string? AccountforOutgoingChecks { get; set; }
        public string? AccountforCashReceipt { get; set; }
        public string? ForeignAccountsReceivables { get; set; }
        public string? CreditorsFollowUpAccount { get; set; }
        public string? OutgoingChecksAccount { get; set; }
        public string? DefaultSaleAccount { get; set; }
    }
}
