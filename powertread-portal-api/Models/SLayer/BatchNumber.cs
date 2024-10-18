namespace SAPB1SLayerWebAPI.Models.SLayer
{
    public class BatchNumber
    {
        public int DocEntry { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string ItemDescription { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Batch { get; set; } = string.Empty;
        public string BatchAttribute1 { get; set; } = string.Empty;
        public string BatchAttribute12 { get; set; } = string.Empty;
        public string AdmissionDate { get; set; } = string.Empty;
        public string ManufacturingDate { get; set; } = string.Empty;
        public string ExpirationDate { get; set; } = string.Empty;
        public int SystemNumber { get; set; }
    }
}
