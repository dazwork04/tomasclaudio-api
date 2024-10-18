namespace SAPB1SLayerWebAPI.Models
{
    public class DocumentList
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string DocDate { get; set; } = string.Empty;
        public string CardCode { get; set; } = string.Empty;
        public string CardName { get; set; } = string.Empty;
        public string NumAtCard { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public string U_PlateNo { get; set; } = string.Empty;

        // PR
        public string Requester { get; set; } = string.Empty;
        public string RequesterName { get; set; } = string.Empty;
        public string DocDueDate { get; set; } = string.Empty;
        public string PriceMode { get; set; } = string.Empty;

        // ITR, IT
        public string FromWarehouse { get; set; } = string.Empty;
        public string FromWarehouseName { get; set; } = string.Empty;
        public string ToWarehouse { get; set; } = string.Empty;
        public string ToWarehouseName { get; set; } = string.Empty;

    }
}
