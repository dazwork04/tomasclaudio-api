namespace SAPB1SLayerWebAPI.Models
{
    public class IssueForProduction
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public int Series { get; set; }
        public string SeriesName { get; set; } = string.Empty;
        public List<IssueForProductionLine> DocumentLines { get; set; } = [];

    }

    public class IssueForProductionLine
    {
        public int BaseDoc { get; set; }
        public int BaseLine { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Quantity { get; set; }
        public string WarehouseCode { get; set; } = string.Empty;
        public string UoMCode { get; set; } = string.Empty;

        public int BaseType { get; set; }
        public int BaseEntry { get; set; }
        public int UoMEntry { get; set; }
    }
}
