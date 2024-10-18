namespace SAPB1SLayerWebAPI.Models.SLayer
{
    public class WitholdingTaxCode
    {
        public string WTCode { get; set; } = string.Empty;
        public string WTName { get; set; } = string.Empty;
        public double Rate { get; set; }
        public string EffectiveFrom { get; set; } = string.Empty;
        public string? Currency { get; set; }
        public string Account { get; set; } = string.Empty;
    }
}
