namespace SAPB1SLayerWebAPI.Models.SLayer
{
    public class PriceList
    {
        public int PriceListNo { get; set; }
        public string PriceListName { get; set; } = string.Empty;
        public string IsGrossPrice { get; set; } = string.Empty;
    }
}
