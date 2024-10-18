namespace SAPB1SLayerWebAPI.Models.SLayer
{
    public class Item
    {
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public double QuantityOnStock { get; set; }
        public string ItemType { get; set; } = string.Empty;
        public int ItemsGroupCode { get; set; }
        public int UoMGroupEntry { get; set; }
        public string InventoryItem { get; set; } = string.Empty;
        public string SalesItem { get; set; } = string.Empty;
        public string PurchaseItem { get; set; } = string.Empty;
        public List<ItemUnitOfMeasurementCollection> ItemUnitOfMeasurementCollection { get; set; } = [];
        public string SalesUnit { get; set; } = string.Empty;
        public string PurchaseUnit { get; set; } = string.Empty;
        public string InventoryUOM { get; set; } = string.Empty;
        public string ManageSerialNumbers { get; set; } = string.Empty;
        public string ManageBatchNumbers { get; set; } = string.Empty;
        public string? DefaultWarehouse { get; set; }
        public int InventoryUoMEntry { get; set; }
        public int? DefaultSalesUoMEntry { get; set; }
        public int? DefaultPurchasingUoMEntry { get; set; }
        public string SalesVATGroup { get; set; } = string.Empty;
        public string PurchaseVATGroup { get; set; } = string.Empty;
        public string? SupplierCatalogNo { get; set; }
        public string CostAccountingMethod { get; set; } = string.Empty;
        
        public List<ItemWarehouseInfoCollection> ItemWarehouseInfoCollection { get; set; } = [];
    }

    public class ItemUnitOfMeasurementCollection
    {
        public string UoMType { get; set; } = string.Empty;
        public int UoMEntry { get; set; }
    }

    public class ItemWarehouseInfoCollection
    {
        public double MinimalStock { get; set; }
        public double MaximalStock { get; set; }
        public double MinimalOrder { get; set; }
        public double StandardAveragePrice { get; set; }
        public string Locked { get; set; } = string.Empty;
        public string WarehouseCode { get; set; } = string.Empty;
        public double InStock { get; set; }
        public double Committed { get; set; }
        public double Ordered { get; set; }
    }
}