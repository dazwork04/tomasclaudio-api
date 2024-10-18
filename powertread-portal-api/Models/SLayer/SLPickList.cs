namespace SAPB1SLayerWebAPI.Models.SLayer
{
    public class SLPickList
    {
        public int Absoluteentry { get; set; }
        public string Name { get; set; } = string.Empty;
        public int OwnerCode { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string PickDate { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ObjectType { get; set; } = string.Empty;
        public string UseBaseUnits { get; set; } = string.Empty;
        public List<PickListsLine> PickListsLines { get; set; } = [];
    }

    public class PickListsLine
    {
        public int AbsoluteEntry { get; set; }
        public int LineNumber { get; set; }
        public int OrderEntry { get; set; }
        public int OrderRowID { get; set; }
        public double PickedQuantity { get; set; }
        public string PickStatus { get; set; } = string.Empty;
        public double ReleasedQuantity { get; set; }
        public double PreviouslyReleasedQuantity { get; set; }
        public int BaseObjectType { get; set; }

    }
}
