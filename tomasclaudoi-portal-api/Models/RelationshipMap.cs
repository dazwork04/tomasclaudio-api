namespace SAPB1SLayerWebAPI.Models
{
    public class RelationshipMap
    {
        public List<BusinessPartnerMap> BusinessPartners = new();
        //
        public List<DocumentMap> Bases { get; set; } = new();
        public DocumentMap Main { get; set; } = new();
        public List<DocumentMap> Targets { get; set; } = new();
    }

    public class BusinessPartnerMap
    {
        public string CardCode { get; set; } = string.Empty;
        public string CardName { get; set; } = string.Empty;
    }

    public class DocumentMap
    {
        public string DocumentStatus { get; set; } = string.Empty;
        public int DocNum { get; set; }
        public string DocDate { get; set; } = string.Empty;
        public string NumAtCard { get; set; } = string.Empty;
        public string DocCurrency { get; set; } = string.Empty;
        public double DocTotal { get; set; }
        //
        public int DocEntry { get; set; }
        public string DocObjectCode { get; set; } = string.Empty;

        public string CardCode = string.Empty;
        public string CardName = string.Empty;
    }

}
