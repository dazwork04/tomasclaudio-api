using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrderController : ControllerBase
    {
        private readonly PurchaseOrderService poService;
        public PurchaseOrderController() => poService = new();

        // GET PURCHASE ORDERS
        [HttpPost("GetPurchaseOrders/{userId}/{companyDB}/{status}/{cancelled}/{dateFrom}/{dateTo}")]
        public async Task<IActionResult> GetPurchaseOrders(int userId, string companyDB, char status, char cancelled, string dateFrom, string dateTo, Paginate paginate) =>
            Ok(await poService.GetPurchaseOrdersAsync(userId, companyDB, status, cancelled, dateFrom, dateTo, paginate));

        // CREATE PURCHASE ORDER
        [HttpPost("CreatePurchaseOrder/{userId}/{companyDB}/{forApproval}")]
        public async Task<IActionResult> CreatePurchaseOrder(int userId, string companyDB, char forApproval, Document purchaseOrder) => Ok(await poService.CreatePurchaseOrderAsync(userId, companyDB, forApproval, purchaseOrder));

        // UPDATE PURCHASE ORDER
        [HttpPost("UpdatePurchaseOrder/{userId}/{companyDB}")]
        public async Task<IActionResult> UpdatePurchaseOrder(int userId, string companyDB, dynamic purchaseOrder) => Ok(await poService.UpdatePurchaseOrderAsync(userId, companyDB, purchaseOrder));

        // CANCEL PURCHASE ORDER
        [HttpPost("CancelPurchaseOrder/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> CancelPurchaseOrder(int userId, string companyDB, int docEntry) =>
            Ok(await poService.CancelPurchaseOrderAsync(userId, companyDB, docEntry));

        // CLOSE PURCHASE ORDER
        [HttpPost("ClosePurchaseOrder/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> ClosePurchaseOrder(int userId, string companyDB, int docEntry) =>
            Ok(await poService.ClosePurchaseOrderAsync(userId, companyDB, docEntry));

        // GET PURCHASE REQUESTS
        [HttpGet("GetPurchaseRequests/{userId}/{companyDB}/{docType}/{priceMode}")]
        public async Task<IActionResult> GetPurchaseOrders(int userId, string companyDB, string docType, string priceMode) =>
            Ok(await poService.GetPurchaseRequestsAsync(userId, companyDB, docType, priceMode));
    }
}
