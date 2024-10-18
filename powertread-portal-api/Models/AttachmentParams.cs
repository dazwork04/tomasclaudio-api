namespace SAPB1SLayerWebAPI.Models
{
    public class AttachmentParams
    {
        public int Key { get; set; }
        public string FileName { get; set; } = string.Empty;
        public required dynamic File { get; set; }
    }   
}
