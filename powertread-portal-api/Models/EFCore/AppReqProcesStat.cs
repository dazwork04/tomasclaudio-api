namespace SAPB1SLayerWebAPI.Models.EFCore
{
    public class AppReqProcesStat
    {
        public int Code { get; set; }
        public int DraftEntry { get; set; }
        public int? DocEntry { get; set; }
        public string ObjType { get; set; } = string.Empty;
        public char ProcesStat { get; set; }
    }
}
