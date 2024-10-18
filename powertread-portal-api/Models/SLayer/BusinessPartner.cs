namespace SAPB1SLayerWebAPI.Models.SLayer
{
    public class BusinessPartner
    {
        public string CardCode { get; set; } = string.Empty;
        public string CardName { get; set; } = string.Empty;
        public string CardType { get; set; } = string.Empty;
        public double CurrentAccountBalance { get; set; }
        public string ContactPerson { get; set; } = string.Empty;
        public int PayTermsGrpCode { get; set; }
        public string Currency { get; set; } = string.Empty;
        public int SalesPersonCode { get; set; }
        public string VatGroup { get; set; } = string.Empty;
        public string PriceMode { get; set; } = string.Empty;
        public string DefaultCurrency { get; set; } = string.Empty;
        public string BilltoDefault { get; set; } = string.Empty;
        public string ShipToDefault { get; set; } = string.Empty;
        public string VatLiable { get; set; } = string.Empty;
        public string SubjectToWithholdingTax { get; set; } = string.Empty;
        public string FederalTaxID { get; set; } = string.Empty;
        public string Phone1 { get; set; } = string.Empty;
        public int PriceListNum { get; set; }
        public string Valid { get; set; } = string.Empty;
        public List<BPAddress> BPAddresses { get; set; } = [];
        public List<ContactEmployee> ContactEmployees { get; set; } = [];
        public List<BPWithholdingTaxCollection> BPWithholdingTaxCollection { get; set; } = [];
    }

    public class BPAddress
    {
        public string AddressName { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string AddressType { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
    }

    public class ContactEmployee
    {
        public string Name { get; set; } = string.Empty;
        public int InternalCode { get; set; }
        public string Active { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public class BPWithholdingTaxCollection {
        public string WTCode { get; set; } = string.Empty;
        public string BPCode { get; set; } = string.Empty;
    }
}
