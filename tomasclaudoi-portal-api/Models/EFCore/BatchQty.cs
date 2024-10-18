using Microsoft.EntityFrameworkCore;

namespace SAPB1SLayerWebAPI.Models.EFCore
{
    public class BatchQty
    {
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string BatchNum { get; set; } = string.Empty;
        public string WhsCode { get; set; } = string.Empty;
        [Precision(19, 6)]
        public decimal Quantity { get; set; }
    }
}
