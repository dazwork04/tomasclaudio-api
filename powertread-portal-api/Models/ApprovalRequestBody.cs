using SAPB1SLayerWebAPI.Models;

namespace Models
{
    public class ApprovalRequestBody
    {
        public Paginate Paginate { get; set; } = new();
        public List<int> Approvals { get; set; } = [];
        public int? UserId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string IsDraft { get; set; } = string.Empty;
        public string ObjectType { get; set; } = string.Empty;
    }

    //public class Approval
    //{
    //    public int ApprovalTemplatesID { get; set; }
    //    public int CurrentStage { get; set; }
    //    public char IsOriginator { get; set; }
    //    public char IsApprover { get; set; }
    //}
}
