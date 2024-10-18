namespace SAPB1SLayerWebAPI.Models.SLayer
{
    public class UserFieldsMD
    {
        public string Name { get; set; } = string.Empty;
        public int Size { get; set; }
        public string Description { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public int FieldID { get; set; }
        public string Mandatory { get; set; } = string.Empty;
        public string? DefaultValue { get; set; } = string.Empty;
        public List<ValidValuesMD>? ValidValuesMD { get; set; } = new();
    }

    public class ValidValuesMD
    {
        public string? Value { get; set; }
        public string? Description { get; set; }
    }
}
