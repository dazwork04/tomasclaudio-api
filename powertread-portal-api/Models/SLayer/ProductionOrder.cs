namespace SAPB1SLayerWebAPI.Models.SLayer
{
    public class ProductionOrder
    {
        public int AbsoluteEntry { get; set; }
        public int DocumentNumber { get; set; }
        public string ItemNo { get; set; } = string.Empty;
        public List<ProductionOrderLine> ProductionOrderLines { get; set; } = [];

    }

    public class ProductionOrderLine
    {
        public int DocumentAbsoluteEntry { get; set; }
        public int LineNumber { get; set; }
        public string ItemNo { get; set; } = string.Empty;
        public double BaseQuantity { get; set; }
        public double PlannedQuantity { get; set; }
        public double IssuedQuantity { get; set; }
        public string Warehouse { get; set; } = string.Empty;
        public int VisualOrder { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int UoMCode { get; set; }
        public int UomEntry { get; set; }

    }
}
