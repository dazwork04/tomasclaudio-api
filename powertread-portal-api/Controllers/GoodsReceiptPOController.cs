using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoodsReceiptPOController : ControllerBase
    {
        private readonly GoodsReceiptPOService grpoService;
        public GoodsReceiptPOController() => grpoService = new();

        // GET PURCHASE ORDERS
        [HttpPost("GetGoodsReceiptPOs/{userId}/{companyDB}/{status}/{cancelled}/{dateFrom}/{dateTo}")]
        public async Task<IActionResult> GetGoodsReceiptPOs(int userId, string companyDB, char status, char cancelled, string dateFrom, string dateTo, Paginate paginate) =>
            Ok(await grpoService.GetGoodsReceiptPOsAsync(userId, companyDB, status, cancelled, dateFrom, dateTo, paginate));

        // CREATE PURCHASE ORDER
        [HttpPost("CreateGoodsReceiptPO/{userId}/{companyDB}/{forApproval}")]
        public async Task<IActionResult> CreateGoodsReceiptPO(int userId, string companyDB, char forApproval, dynamic goodsReceiptPO) => Ok(await grpoService.CreateGoodsReceiptPOAsync(userId, companyDB, forApproval, goodsReceiptPO));

        // UPDATE PURCHASE ORDER
        [HttpPost("UpdateGoodsReceiptPO/{userId}/{companyDB}")]
        public async Task<IActionResult> UpdateGoodsReceiptPO(int userId, string companyDB, dynamic purchaseOrder) => Ok(await grpoService.UpdateGoodsReceiptPOAsync(userId, companyDB, purchaseOrder));

        // CANCEL PURCHASE ORDER
        [HttpPost("CancelGoodsReceiptPO/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> CancelPurchaseOrder(int userId, string companyDB, int docEntry) =>
            Ok(await grpoService.CancelGoodsReceiptPOAsync(userId, companyDB, docEntry));

        // CLOSE PURCHASE ORDER
        [HttpPost("CloseGoodsReceiptPO/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> CloseGoodsReceiptPO(int userId, string companyDB, int docEntry) =>
            Ok(await grpoService.CloseGoodsReceiptPOAsync(userId, companyDB, docEntry));

        // GET PURCHASE ORDERS
        [HttpGet("GetPurchaseOrders/{userId}/{companyDB}/{cardCode}/{docType}/{priceMode}")]
        public async Task<IActionResult> GetPurchaseOrders(int userId, string companyDB, string cardCode, string docType, string priceMode) =>
            Ok(await grpoService.GetPurchaseOrdersAsync(userId, companyDB, cardCode, docType, priceMode));
    }
}
