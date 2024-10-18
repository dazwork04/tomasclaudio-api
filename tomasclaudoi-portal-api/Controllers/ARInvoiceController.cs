using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Models;
using SARB1SLayerWebARI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ARInvoiceController : ControllerBase
    {
        private readonly ARInvoiceService ariService;
        public ARInvoiceController() => ariService = new();

        // GET AR INVOICES
        [HttpPost("GetARInvoices/{userId}/{companyDB}/{status}/{dateFrom}/{dateTo}")]
        public async Task<IActionResult> GetARInvoices(int userId, string companyDB, char status, string dateFrom, string dateTo, Paginate paginate) =>
            Ok(await ariService.GetARInvoicesAsync(userId, companyDB, status, dateFrom, dateTo, paginate));

        // CREATE AR INVOICE
        [HttpPost("CreateARInvoice/{userId}/{companyDB}/{forApproval}")]
        public async Task<IActionResult> CreateARInvoice(int userId, string companyDB, char forApproval, dynamic apInvoice) => Ok(await ariService.CreateARInvoiceAsync(userId, companyDB, forApproval, apInvoice));

        // UPDATE AR INVOICE
        [HttpPost("UpdateARInvoice/{userId}/{companyDB}")]
        public async Task<IActionResult> UpdateARInvoice(int userId, string companyDB, dynamic apInvoice) => Ok(await ariService.UpdateARInvoiceAsync(userId, companyDB, apInvoice));

        // CANCEL AR INVOICE
        [HttpPost("CancelARInvoice/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> CancelPurchaseOrder(int userId, string companyDB, int docEntry) =>
            Ok(await ariService.CancelARInvoiceAsync(userId, companyDB, docEntry));

        // CLOSE AR INVOICE
        [HttpPost("CloseARInvoice/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> CloseGoodsReceiptPO(int userId, string companyDB, int docEntry) =>
            Ok(await ariService.CloseARInvoiceAsync(userId, companyDB, docEntry));

        // CANCELLATION AR INVOICE
        [HttpPost("CancellationARInvoice/{userId}/{companyDB}")]
        public async Task<IActionResult> CancellationARInvoice(int userId, string companyDB, dynamic arInvoice) =>
            Ok(await ariService.CancellationARInvoiceAsync(userId, companyDB, arInvoice));

        // GET SALES ORDERS
        [HttpGet("GetSalesOrders/{userId}/{companyDB}/{cardCode}/{docType}/{priceMode}")]
        public async Task<IActionResult> GetSalesOrders(int userId, string companyDB, string cardCode, string docType, string priceMode) =>
            Ok(await ariService.GetSalesOrdersAsync(userId, companyDB, cardCode, docType, priceMode));

        // GET DELIVERIES
        [HttpGet("GetDeliveries/{userId}/{companyDB}/{cardCode}/{docType}/{priceMode}")]
        public async Task<IActionResult> GetDeliveries(int userId, string companyDB, string cardCode, string docType, string priceMode) =>
            Ok(await ariService.GetDeliveriesAsync(userId, companyDB, cardCode, docType, priceMode));

        // GET DOWNPAYMENTS
        [HttpGet("GetDownPayments/{userId}/{companyDB}/{cardCode}")]
        public async Task<IActionResult> GetDownPayments(int userId, string companyDB, string cardCode) => Ok(await ariService.GetDownPaymentsAsync(userId, companyDB, cardCode));
    }
}
