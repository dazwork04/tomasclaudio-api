using Microsoft.EntityFrameworkCore;

namespace SAPB1SLayerWebAPI.Models.EFCore
{
    public class SerialQty
    {
        public string ItemCode { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string SerialNum { get; set; } = string.Empty;
        public string WhsCode { get; set; } = string.Empty;
        [Precision(19, 6)]
        public decimal Quantity { get; set; }
        public string? ManufacturerSerialNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? ReceptionDate { get; set; }
    }
}
