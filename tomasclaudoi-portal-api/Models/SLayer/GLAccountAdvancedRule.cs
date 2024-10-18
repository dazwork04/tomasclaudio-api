namespace SAPB1SLayerWebAPI.Models.SLayer
{
    public class GLAccountAdvancedRule
    {
        public int AbsoluteEntry { get; set; }
        public int ItemGroup { get; set; }
        public string IsActive { get; set; } = string.Empty;
        public string InventoryAccount { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
