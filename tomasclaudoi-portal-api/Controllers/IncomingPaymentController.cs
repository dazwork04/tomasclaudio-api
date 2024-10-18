using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Context;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IncomingPaymentController : ControllerBase
    {
        private readonly IncomingPaymentService ipService;
        private readonly MainDbContext mainDbContext;
        public IncomingPaymentController(MainDbContext mainDbContext)
        {
            ipService = new(mainDbContext);
            this.mainDbContext = mainDbContext;
        }

        // GET INCOMING PAYMENTS
        [HttpPost("GetIncomingPayments/{userId}/{companyDB}/{cancelled}/{dateFrom}/{dateTo}")]
        public async Task<IActionResult> GetIncomingPayments(int userId, string companyDB, string cancelled, string dateFrom, string dateTo, Paginate paginate) =>
            Ok(await ipService.GetIncomingPaymentsAsync(userId, companyDB, cancelled, dateFrom, dateTo, paginate));

        // GET INCOMING PAYMENT LINES
        [HttpGet("GetIncomingPaymentLines/{userId}/{companyDB}/{docEntry}/{posted}")]
        public async Task<IActionResult> GetIncomingPaymentLines(int userId, string companyDB, int docEntry, char posted) =>
            Ok(await ipService.GetIncomingPaymentLinesAsync(userId, companyDB, docEntry, posted));

        // GET BP INCOMING PAYMENTS
        [HttpGet("GetBPInvoices/{userId}/{companyDB}/{cardCode}")]
        public async Task<IActionResult> GetBPInvoices(int userId, string companyDB, string cardCode) => Ok(await ipService.GetBPInvoicesAsync(userId, companyDB, cardCode, mainDbContext));

        // GET INCOMING PAYMENT OTHER DATA
        [HttpGet("GetIncomingPaymentOtherData/{companyDB}/{docEntry}/{posted}")]
        public async Task<IActionResult> GetIncomingPaymentOtherData(string companyDB, char posted, int docEntry) => Ok(await ipService.GetIncomingPaymentOtherDataAsync(companyDB, docEntry, posted, mainDbContext));

        // CREATE INCOMING PAYMENT
        [HttpPost("CreateIncomingPayment/{userId}/{companyDB}/{forPosted}")]
        public async Task<IActionResult> CreateIncomingPayment(int userId, string companyDB, char forPosted, PostPaymentObject payment) => Ok(await ipService.CreateIncomingPaymentAsync(userId, companyDB, forPosted, payment));

        // UPDATE INCOMING PAYMENT
        [HttpPost("UpdateIncomingPayment/{userId}/{companyDB}/{forPosted}")]
        public async Task<IActionResult> UpdateIncomingPayment(int userId, string companyDB, char forPosted, dynamic payment) => Ok(await ipService.UpdateIncomingPaymentAsync(userId, companyDB, forPosted, payment));

        // CANCEL INCOMING PAYMENT
        [HttpPost("CancelIncomingPayment/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> CancelIncomingPayment(int userId, string companyDB, int docEntry) => Ok(await ipService.CancelIncomingPaymentAsync(userId, companyDB, docEntry));
    }
}
