namespace SAPB1SLayerWebAPI.Models.SLayer
{
    public class UomGroup
    {
        public int AbsEntry { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<UoMGroupDefinitionCollection> UoMGroupDefinitionCollection { get; set; } = new();

    }

    public class UoMGroupDefinitionCollection
    {
        public int AlternateUoM { get; set; }
    }
}
