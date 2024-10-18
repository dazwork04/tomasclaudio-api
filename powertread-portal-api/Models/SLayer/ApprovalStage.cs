namespace SAPB1SLayerWebAPI.Models.SLayer
{
    public class ApprovalStage
    {
        public int Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public int NoOfApproversRequired { get; set; }
        public List<ApprovalStageApprover> ApprovalStageApprovers { get; set; } = [];
    }

    public class ApprovalStageApprover
    {
        public int UserID { get; set; }
    }
}
