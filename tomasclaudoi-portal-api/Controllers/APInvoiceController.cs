using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class APInvoiceController : ControllerBase
    {
        private readonly APinvoiceService apiService;
        public APInvoiceController() => apiService = new();

        // GET AP INVOICES
        [HttpPost("GetAPInvoices/{userId}/{companyDB}/{status}/{cancelled}/{dateFrom}/{dateTo}")]
        public async Task<IActionResult> GeAPInvoices(int userId, string companyDB, char status, char cancelled, string dateFrom, string dateTo, Paginate paginate) =>
            Ok(await apiService.GetAPInvoicesAsync(userId, companyDB, status, cancelled, dateFrom, dateTo, paginate));

        // CREATE AP INVOICE
        [HttpPost("CreateAPInvoice/{userId}/{companyDB}")]
        public async Task<IActionResult> CreateAPInvoice(int userId, string companyDB, dynamic apInvoice) => Ok(await apiService.CreateAPInvoiceAsync(userId, companyDB, apInvoice));

        // UPDATE AP INVOICE
        [HttpPost("UpdateAPInvoice/{userId}/{companyDB}")]
        public async Task<IActionResult> UpdateAPInvoice(int userId, string companyDB, dynamic apInvoice) => Ok(await apiService.UpdateAPInvoiceAsync(userId, companyDB, apInvoice));

        // CANCEL AP INVOICE
        [HttpPost("CancelAPInvoice/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> CancelPurchaseOrder(int userId, string companyDB, int docEntry) =>
            Ok(await apiService.CancelAPInvoiceAsync(userId, companyDB, docEntry));

        // CLOSE AP INVOICE
        [HttpPost("CloseAPInvoice/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> CloseGoodsReceiptPO(int userId, string companyDB, int docEntry) =>
            Ok(await apiService.CloseAPInvoiceAsync(userId, companyDB, docEntry));

        // GET PURCHASE ORDERS
        [HttpGet("GetPurchaseOrders/{userId}/{companyDB}/{cardCode}/{docType}/{priceMode}")]
        public async Task<IActionResult> GetPurchaseOrders(int userId, string companyDB, string cardCode, string docType, string priceMode) =>
            Ok(await apiService.GetPurchaseOrdersAsync(userId, companyDB, cardCode, docType, priceMode));

        // GET GOODS RECEIPT POS
        [HttpGet("GetGoodsReceiptPOs/{userId}/{companyDB}/{cardCode}/{docType}/{priceMode}")]
        public async Task<IActionResult> GetGoodsReceiptPOs(int userId, string companyDB, string cardCode, string docType, string priceMode) =>
            Ok(await apiService.GetGoodsReceiptPOsAsync(userId, companyDB, cardCode, docType, priceMode));
    }
}
