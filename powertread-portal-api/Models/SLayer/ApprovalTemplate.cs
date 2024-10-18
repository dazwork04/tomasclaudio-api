namespace SAPB1SLayerWebAPI.Models.SLayer
{
    public class ApprovalTemplate
    {
        public int Code { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public string IsActive { get; set; } = string.Empty;
        public List<ApprovalTemplateUser> ApprovalTemplateUsers { get; set; } = [];
        public List<ApprovalTemplateStage> ApprovalTemplateStages { get; set; } = [];
        public List<ApprovalTemplateDocument> ApprovalTemplateDocuments { get; set; } = [];

    }

    public class ApprovalTemplateUser
    {
        public int UserID { get; set; }
    }

    public class ApprovalTemplateStage
    {
        public int SortID { get; set; }
        public int ApprovalStageCode { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }

    public class ApprovalTemplateDocument
    {
        public string DocumentType { get; set; } = string.Empty;
    }

}
