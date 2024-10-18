using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using SAPB1SLayerWebAPI.Context;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalRequestController : ControllerBase
    {
        #region VARIABLES
        private readonly ApprovalRequestService approvalRequestService;
        #endregion
        public ApprovalRequestController(MainDbContext mainDbContext) => approvalRequestService = new(mainDbContext);

        [HttpPost("GetApprovalRequests/{userId}/{companyDB}")]
        public async Task<IActionResult> GetApprovalRequests(int userId, string companyDB, ApprovalRequestBody body) => Ok(await approvalRequestService.GetApprovalRequestsAsync(userId, companyDB, body));

        //[HttpGet("GetApprovedApprovalRequests/{userId}/{companyDB}")]
        //public async Task<IActionResult> GetApprovedApprovalRequests(int userId, string companyDB, ApprovalRequestBody body) => Ok(await approvalRequestService.GetApprovedApprovalRequests(userId, companyDB, body));

        [HttpPost("ApproveApprovalRequest/{userId}/{companyDB}")]
        public async Task<IActionResult> ApproveApprovalRequest(int userId, string companyDB, dynamic body) => Ok(await approvalRequestService.ApproveApprovalRequestAsync(userId, companyDB, body));

        [HttpPost("RejectApprovalRequest/{userId}/{companyDB}")]
        public async Task<IActionResult> RejectApprovalRequest(int userId, string companyDB, dynamic body) => Ok(await approvalRequestService.RejectApprovalRequestAsync(userId, companyDB, body));

        [HttpPost("AddToDocumentApprovalRequest/{userId}/{companyDB}")]
        public async Task<IActionResult> AddToDocumentApprovalRequest(int userId, string companyDB, dynamic body) => Ok(await approvalRequestService.AddToDocumentApprovalRequestAsync(userId, companyDB, body));

        [HttpPost("UpdateDraft/{userId}/{companyDB}")]
        public async Task<IActionResult> UpdateDraft(int userId, string companyDB, dynamic draft) => Ok(await approvalRequestService.UpdateDraftAsync(userId, companyDB, draft));

        [HttpPost("CreateDraft/{userId}/{companyDB}")]
        public async Task<IActionResult> CreateDraft(int userId, string companyDB, dynamic draft) => Ok(await approvalRequestService.CreateDraftAsync(userId, companyDB, draft));

        [HttpPost("UpdateDraftPayment/{userId}/{companyDB}")]
        public async Task<IActionResult> UpdateDraftPayment(int userId, string companyDB, dynamic draft) => Ok(await approvalRequestService.UpdateDraftPaymentAsync(userId, companyDB, draft));

        [HttpPost("GetApprovalRequestsBilling/{userId}/{companyDB}")]
        public async Task<IActionResult> GetApprovalRequestsBilling(int userId, string companyDB, ApprovalRequestBody body) => Ok(await approvalRequestService.GetApprovalRequestsBillingAsync(userId, companyDB, body));

        [HttpPost("GetApprovalRequestsSalesOrder/{userId}/{companyDB}")]
        public async Task<IActionResult> GetApprovalRequestsSalesOrder(int userId, string companyDB, ApprovalRequestBody body) => Ok(await approvalRequestService.GetApprovalRequestsSalesOrderAsync(userId, companyDB, body));
        [HttpPost("GetApprovalRequestsDelivert/{userId}/{companyDB}")]
        public async Task<IActionResult> GetApprovalRequestsDelivert(int userId, string companyDB, ApprovalRequestBody body) => Ok(await approvalRequestService.GetApprovalRequestsDeliveryAsync(userId, companyDB, body));

        [HttpPost("GetApprovalStatus/{userId}/{companyDB}")]
        public async Task<IActionResult> GetApprovalStatus(int userId, string companyDB, ApprovalRequestBody body) => Ok(await approvalRequestService.GetApprovalStatusAsync(userId, companyDB, body));

        [HttpPost("UpdateApprovalRequest/{userId}/{companyDB}")]
        public async Task<IActionResult> UpdateApprovalRequest(int userId, string companyDB, dynamic body) => Ok(await approvalRequestService.UpdateApprovalRequestAsync(userId, companyDB, body));

        [HttpPost("AddToDocumentPaymentDraft/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> AddToDocumentPaymentDraft(int userId, string companyDB, int docEntry) => Ok(await approvalRequestService.AddToDocumentPaymentDraftAsync(userId, companyDB, docEntry));

        [HttpPost("RemovePaymentDraft/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> RemovePaymentDraft(int userId, string companyDB, int docEntry) => Ok(await approvalRequestService.RemovePaymentDraftAsync(userId, companyDB, docEntry));
    }
}
