﻿namespace Models
{
    public class PickList
    {
        public int Absoluteentry { get; set; }
        public string Name { get; set; } = string.Empty;
        public int OwnerCode { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
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

        //public int TransType { get; set; }
        public int BaseDocNum { get; set; }
        public string CardCode { get; set; } = string.Empty;
        public string CardName { get; set; } = string.Empty;
        public string DueDate { get; set; } = string.Empty;
        public string OrderDate { get; set; } = string.Empty;
        public string ItemCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string UomCode { get; set; } = string.Empty;
        public string UomName { get; set; } = string.Empty;
        //public double ItemsPerUnit { get; set; } 
        public string WarehouseCode { get; set; } = string.Empty;
        public string WarehouseName { get; set; } = string.Empty;
        //public double Ordered { get; set; }
        //public double Released { get; set; }
        //public double Picked { get; set; }
        public double AvailToPick { get; set; }
    }
}