namespace SAPB1SLayerWebAPI.Models.SLayer
{
    public class EmployeeInfo
    {
        public int EmployeeID { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string Gender { get; set; } = string.Empty;
        public int? Branch { get; set; }
        public int? Department { get; set; }
        public int ApplicationUserID { get; set; }
        public string? Email {  get; set; }
    }
}
