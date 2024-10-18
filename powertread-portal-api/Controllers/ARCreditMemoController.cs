using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Models;
using SARB1SLayerWebARI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ARCreditMemoController : ControllerBase
    {
        private readonly ARCreditMemoService arcmService;
        public ARCreditMemoController() => arcmService = new();

        // GET AR CREDIT MEMO
        [HttpPost("GetARCreditMemos/{userId}/{companyDB}/{status}/{cancelled}/{dateFrom}/{dateTo}")]
        public async Task<IActionResult> GetARCreditMemos(int userId, string companyDB, char status, char cancelled, string dateFrom, string dateTo, Paginate paginate) =>
            Ok(await arcmService.GetARCreditMemosAsync(userId, companyDB, status, cancelled, dateFrom, dateTo, paginate));

        // CREATE AR CREDIT MEMO
        [HttpPost("CreateARCreditMemo/{userId}/{companyDB}/{forApproval}")]
        public async Task<IActionResult> CreateARCreditMemo(int userId, string companyDB, char forApproval, dynamic apInvoice) => Ok(await arcmService.CreateARCreditMemoAsync(userId, companyDB, forApproval, apInvoice));

        // UPDATE AR CREDIT MEMO
        [HttpPost("UpdateARCreditMemo/{userId}/{companyDB}")]
        public async Task<IActionResult> UpdateARCreditMemo(int userId, string companyDB, dynamic apInvoice) => Ok(await arcmService.UpdateARCreditMemoAsync(userId, companyDB, apInvoice));

        // CANCEL AR CREDIT MEMO
        [HttpPost("CancelARCreditMemo/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> CancelPurchaseOrder(int userId, string companyDB, int docEntry) =>
            Ok(await arcmService.CancelARCreditMemoAsync(userId, companyDB, docEntry));

        // CLOSE AR CREDIT MEMO
        [HttpPost("CloseARCreditMemo/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> CloseGoodsReceiptPO(int userId, string companyDB, int docEntry) =>
            Ok(await arcmService.CloseARCreditMemoAsync(userId, companyDB, docEntry));

        // GET SALES ORDERS
        [HttpGet("GetSalesOrders/{userId}/{companyDB}/{cardCode}/{docType}/{priceMode}")]
        public async Task<IActionResult> GetSalesOrders(int userId, string companyDB, string cardCode, string docType, string priceMode) =>
            Ok(await arcmService.GetSalesOrdersAsync(userId, companyDB, cardCode, docType, priceMode));

        // GET DELIVERIES
        [HttpGet("GetDeliveries/{userId}/{companyDB}/{cardCode}/{docType}/{priceMode}")]
        public async Task<IActionResult> GetDeliveries(int userId, string companyDB, string cardCode, string docType, string priceMode) =>
            Ok(await arcmService.GetDeliveriesAsync(userId, companyDB, cardCode, docType, priceMode));

        // GET INVOICES
        [HttpGet("GetARInvoices/{userId}/{companyDB}/{cardCode}/{docType}/{priceMode}")]
        public async Task<IActionResult> GetARInvoices(int userId, string companyDB, string cardCode, string docType, string priceMode) =>
            Ok(await arcmService.GetARInvoicesAsync(userId, companyDB, cardCode, docType, priceMode));

        // GET DOWNPAYMENTS
        [HttpGet("GetDownPayments/{userId}/{companyDB}/{cardCode}")]
        public async Task<IActionResult> GetDownPayments(int userId, string companyDB, string cardCode) => Ok(await arcmService.GetDownPaymentsAsync(userId, companyDB, cardCode));
    }
}
