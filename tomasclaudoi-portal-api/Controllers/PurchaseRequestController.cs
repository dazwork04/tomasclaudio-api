using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseRequestController : ControllerBase
    {
        private readonly PurchaseRequestService prService;
        public PurchaseRequestController() => prService = new();

        // GET PURCHASE REQUESTS
        [HttpPost("GetPurchaseRequests/{userId}/{companyDB}/{status}/{cancelled}/{dateFrom}/{dateTo}")]
        public async Task<IActionResult> GetPurchaseRequests(int userId, string companyDB, char status, char cancelled, string dateFrom, string dateTo, Paginate paginate) =>
            Ok(await prService.GetPurchaseRequestsAsync(userId, companyDB, status, cancelled, dateFrom, dateTo, paginate));

        // CREATE PURCHASE REQUEST
        [HttpPost("CreatePurchaseRequest/{userId}/{companyDB}/{forApproval}")]
        public async Task<IActionResult> CreatePurchaseRequest(int userId, string companyDB, char forApproval, dynamic purchaseRequest) => Ok(await prService.CreatePurchaseRequestAsync(userId, companyDB, forApproval, purchaseRequest));

        // UPDATE PURCHASE REQUEST
        [HttpPost("UpdatePurchaseRequest/{userId}/{companyDB}")]
        public async Task<IActionResult> UpdatePurchaseRequest(int userId, string companyDB, dynamic purchaseRequest) => Ok(await prService.UpdatePurchaseRequestAsync(userId, companyDB, purchaseRequest));

        // CANCEL PURCHASE REQUEST
        [HttpPost("CancelPurchaseRequest/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> CancelPurchaseRequest(int userId, string companyDB, int docEntry) =>
            Ok(await prService.CancelPurchaseRequestAsync(userId, companyDB, docEntry));

        // CLOSE PURCHASE REQUEST
        [HttpPost("ClosePurchaseRequest/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> ClosePurchaseRequest(int userId, string companyDB, int docEntry) =>
            Ok(await prService.ClosePurchaseRequestAsync(userId, companyDB, docEntry));
    }
}
