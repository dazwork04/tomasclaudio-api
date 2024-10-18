using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseReturnController : ControllerBase
    {
        private readonly PurchaseReturnService purchaseReturnService;
        public PurchaseReturnController() => purchaseReturnService = new();

        // GET GOODS RETURN
        [HttpPost("GetPurchaseReturns/{userId}/{companyDB}/{status}/{cancelled}/{dateFrom}/{dateTo}")]
        public async Task<IActionResult> GetPurchaseReturns(int userId, string companyDB, char status, char cancelled, string dateFrom, string dateTo, Paginate paginate) =>
            Ok(await purchaseReturnService.GetPurchaseReturnsAsync(userId, companyDB, status, cancelled, dateFrom, dateTo, paginate));

        // CREATE GOODS RETURN
        [HttpPost("CreatePurchaseReturn/{userId}/{companyDB}/{forApproval}")]
        public async Task<IActionResult> CreatePurchaseReturn(int userId, string companyDB, char forApproval, dynamic purchaseReturn) => Ok(await purchaseReturnService.CreatePurchaseReturnAsync(userId, companyDB, forApproval, purchaseReturn));

        // UPDATE GOODS RETURN
        [HttpPost("UpdatePurchaseReturn/{userId}/{companyDB}")]
        public async Task<IActionResult> UpdatePurchaseReturn(int userId, string companyDB, dynamic purchaseReturn) => Ok(await purchaseReturnService.UpdatePurchaseReturnAsync(userId, companyDB, purchaseReturn));

        // CANCEL GOODS RETURN
        [HttpPost("CancelPurchaseReturn/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> CancelPurchaseOrder(int userId, string companyDB, int docEntry) =>
            Ok(await purchaseReturnService.CancelPurchaseReturnAsync(userId, companyDB, docEntry));

        // CLOSE GOODS RETURN
        [HttpPost("ClosePurchaseReturn/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> ClosePurchaseReturn(int userId, string companyDB, int docEntry) =>
            Ok(await purchaseReturnService.ClosePurchaseReturnAsync(userId, companyDB, docEntry));

        // GET GOODS RETURN
        [HttpGet("GetGoodsReceiptPOs/{userId}/{companyDB}/{cardCode}/{docType}/{priceMode}")]
        public async Task<IActionResult> GetPurchaseOrders(int userId, string companyDB, string cardCode, string docType, string priceMode) =>
            Ok(await purchaseReturnService.GetGoodsReceiptPOsAsync(userId, companyDB, cardCode, docType, priceMode));

    }
}
