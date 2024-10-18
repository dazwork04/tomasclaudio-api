namespace SAPB1SLayerWebAPI.Models.SLayer
{
    public class AdminInfo
    {
        public int Code { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string LocalCurrency { get; set; } = string.Empty;
        public string SystemCurrency { get; set; } = string.Empty;
        //
        public int TotalsAccuracy { get; set; }
        public int AccuracyofQuantities { get; set; }
        public int PriceAccuracy { get; set; }
        public int RateAccuracy { get; set; }
        public int PercentageAccuracy { get; set; }
        public int MeasuringAccuracy { get; set; }
        //
        public string EnableSeparatePriceMode { get; set; } = string.Empty;
        //
        public string DefaultWarehouse { get; set; } = string.Empty;

    }
}