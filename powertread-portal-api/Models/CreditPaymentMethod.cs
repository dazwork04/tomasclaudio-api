namespace SAPB1SLayerWebAPI.Models.SLayer
{
    public class CreditPaymentMethod
    {
        public int PaymentMethodCode { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PaymentCode { get; set; } = string.Empty;
    }
}
