using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoodsReceiptController : ControllerBase
    {
        private readonly GoodsReceiptService grService;
        public GoodsReceiptController() => grService = new();

        // GET GOODS RECEIPTS
        [HttpPost("GetGoodsReceipts/{userId}/{companyDB}/{dateFrom}/{dateTo}")]
        public async Task<IActionResult> GetGoodsReceipts(int userId, string companyDB, string dateFrom, string dateTo, Paginate paginate) =>
            Ok(await grService.GetGoodsReceiptsAsync(userId, companyDB, dateFrom, dateTo, paginate));

        // CREATE GOODS RECEIPT
        [HttpPost("CreateGoodsReceipt/{userId}/{companyDB}")]
        public async Task<IActionResult> CreateGoodsReceipt(int userId, string companyDB, dynamic goodsReceipt) =>
            Ok(await grService.CreateGoodsReceiptAsync(userId, companyDB, goodsReceipt));

        // UPDATE GOODS RECEIPT
        [HttpPost("UpdateGoodsReceipt/{userId}/{companyDB}")]
        public async Task<IActionResult> UpdateGoodsReceipt(int userId, string companyDB, dynamic goodsReceipt) =>
            Ok(await grService.UpdateGoodsReceiptAsync(userId, companyDB, goodsReceipt));
    }
}
