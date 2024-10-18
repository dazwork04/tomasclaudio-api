namespace SAPB1SLayerWebAPI.Models.SLayer
{
    public class VatGroup
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<VatGroups_Line> VatGroups_Lines { get; set; } = new();

    }

    public class VatGroups_Line
    {
        public double Rate { get; set; }
        public string Effectivefrom { get; set; } = string.Empty;
    }
}
