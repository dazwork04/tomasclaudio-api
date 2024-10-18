using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APCreditMemoController : ControllerBase
    {
        private readonly APCreditMemoService apcmService;
        public APCreditMemoController() => apcmService = new();

        // GET AP CREDIT MEMOS
        [HttpPost("GetAPCreditMemos/{userId}/{companyDB}/{status}/{cancelled}/{dateFrom}/{dateTo}")]
        public async Task<IActionResult> GeAPCreditMemos(int userId, string companyDB, char status, char cancelled, string dateFrom, string dateTo, Paginate paginate) =>
            Ok(await apcmService.GetAPCreditMemosAsync(userId, companyDB, status, cancelled, dateFrom, dateTo, paginate));

        // CREATE AP CREDIT MEMO
        [HttpPost("CreateAPCreditMemo/{userId}/{companyDB}")]
        public async Task<IActionResult> CreateAPCreditMemo(int userId, string companyDB, dynamic apCreditMemo) => Ok(await apcmService.CreateAPCreditMemoAsync(userId, companyDB, apCreditMemo));

        // UPDATE AP CREDIT MEMO
        [HttpPost("UpdateAPCreditMemo/{userId}/{companyDB}")]
        public async Task<IActionResult> UpdateAPCreditMemo(int userId, string companyDB, dynamic apCreditMemo) => Ok(await apcmService.UpdateAPCreditMemoAsync(userId, companyDB, apCreditMemo));

        // CANCEL AP CREDIT MEMO
        [HttpPost("CancelAPCreditMemo/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> CancelPurchaseOrder(int userId, string companyDB, int docEntry) =>
            Ok(await apcmService.CancelAPCreditMemoAsync(userId, companyDB, docEntry));

        // CLOSE AP CREDIT MEMO
        [HttpPost("CloseAPCreditMemo/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> CloseGoodsReceiptPO(int userId, string companyDB, int docEntry) =>
            Ok(await apcmService.CloseAPCreditMemoAsync(userId, companyDB, docEntry));

        // GET PURCHASE ORDERS
        [HttpGet("GetPurchaseOrders/{userId}/{companyDB}/{cardCode}/{docType}/{priceMode}")]
        public async Task<IActionResult> GetPurchaseOrders(int userId, string companyDB, string cardCode, string docType, string priceMode) =>
            Ok(await apcmService.GetPurchaseOrdersAsync(userId, companyDB, cardCode, docType, priceMode));

        // GET GOODS RECEIPT POS
        [HttpGet("GetGoodsReceiptPOs/{userId}/{companyDB}/{cardCode}/{docType}/{priceMode}")]
        public async Task<IActionResult> GetGoodsReceiptPOs(int userId, string companyDB, string cardCode, string docType, string priceMode) =>
            Ok(await apcmService.GetGoodsReceiptPOsAsync(userId, companyDB, cardCode, docType, priceMode));

        // GET AP INVOICES
        [HttpGet("GetAPInvoices/{userId}/{companyDB}/{cardCode}/{docType}/{priceMode}")]
        public async Task<IActionResult> GetAPInvoices(int userId, string companyDB, string cardCode, string docType, string priceMode) =>
            Ok(await apcmService.GetAPInvoicesAsync(userId, companyDB, cardCode, docType, priceMode));
    }
}
