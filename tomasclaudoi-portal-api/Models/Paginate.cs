namespace SAPB1SLayerWebAPI.Models
{
    public class Paginate
    {
        public int Size { get; set; }
        public int Page { get; set; }
        public string OrderBy { get; set; } = string.Empty;
        public string Direction { get; set; } = string.Empty;
        public string Filter { get; set; } = string.Empty;
    }
}