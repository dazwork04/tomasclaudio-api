namespace SAPB1SLayerWebAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EmpId { get; set; }
        public string UserCode { get; set; } = string.Empty;
        public string UserPass { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SapUser { get; set; } = string.Empty;
        public string SapPass { get; set; } = string.Empty;
        public string EmpCode { get; set; } = string.Empty;
        public string Modules { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string WhseCode { get; set; } = string.Empty;
        public string Branch { get; set; } = string.Empty;
    }
}
