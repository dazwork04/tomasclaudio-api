namespace SAPB1SLayerWebAPI.Models.SLayer
{
    public class Attachment
    {
        public int AbsoluteEntry { get; set; }
        public List<Attachments2_Line> Attachments2_Lines = [];
    }

    public class Attachments2_Line
    {
        public int AbsoluteEntry { get; set; }
        public int LineNum { get; set; }
        public string SourcePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;
        public string AttachmentDate { get; set; } = string.Empty;
        public string Override { get; set; } = string.Empty;
        public string? FreeText { get; set; }
        public string CopyToTargetDoc { get; set; } = string.Empty;
        public string CopyToProductionOrder { get; set; } = string.Empty;
    }
}
