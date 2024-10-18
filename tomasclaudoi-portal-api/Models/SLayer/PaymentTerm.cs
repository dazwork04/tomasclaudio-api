namespace SAPB1SLayerWebAPI.Models.SLayer
{
    public class SLPaymentTermType
    {
        public int GroupNumber { get; set; }
        public string PaymentTermsGroupName { get; set; } = string.Empty;
        public int NumberOfAdditionalDays { get; set; }
    }
}
