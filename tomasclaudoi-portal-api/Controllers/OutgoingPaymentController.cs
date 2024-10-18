using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Context;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OutgoingPaymentController : ControllerBase
    {
        private readonly OutgoingPaymentService opService;
        private readonly MainDbContext mainDbContext;
        public OutgoingPaymentController(MainDbContext mainDbContext)
        {
            opService = new(mainDbContext);
            this.mainDbContext = mainDbContext;
        }

        // GET OUTGOING PAYMENTS
        [HttpPost("GetOutgoingPayments/{userId}/{companyDB}/{cancelled}/{dateFrom}/{dateTo}")]
        public async Task<IActionResult> GetOutgoingPayments(int userId, string companyDB, string cancelled, string dateFrom, string dateTo, Paginate paginate) =>
            Ok(await opService.GetOutgoingPaymentsAsync(userId, companyDB, cancelled, dateFrom, dateTo, paginate));

        // GET OUTGOING PAYMENT LINES
        [HttpGet("GetOutgoingPaymentLines/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> GetOutgoingPaymentLines(int userId, string companyDB, int docEntry) =>
            Ok(await opService.GetOutgoingPaymentLinesAsync(userId, companyDB, docEntry));

        // GET BP OUTGOING PAYMENTS
        [HttpGet("GetBPInvoices/{userId}/{companyDB}/{cardCode}")]
        public async Task<IActionResult> GetBPInvoices(int userId, string companyDB, string cardCode) => Ok(await opService.GetBPInvoicesAsync(userId, companyDB, cardCode, mainDbContext));

        // GET OUTGOING PAYMENT OTHER DATA
        [HttpGet("GetOutgoingPaymentOtherData/{companyDB}/{docEntry}")]
        public async Task<IActionResult> GetOutgoingPaymentOtherData(string companyDB, int docEntry) => Ok(await opService.GetOutgoingPaymentOtherDataAsync(companyDB, docEntry, mainDbContext));

        // CREATE OUTGOING PAYMENT
        [HttpPost("CreateOutgoingPayment/{userId}/{companyDB}")]
        public async Task<IActionResult> CreateIncomingPayment(int userId, string companyDB, PostPaymentObject payment) => Ok(await opService.CreateOutgoingPaymentAsync(userId, companyDB, payment));
    }
}
