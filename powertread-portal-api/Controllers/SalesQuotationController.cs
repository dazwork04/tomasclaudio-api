using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesQuotationController : ControllerBase
    {
        private readonly SalesQuotationService sqService;
        public SalesQuotationController() => sqService = new();

        // GET SALES QUOTATION
        [HttpPost("GetSalesQuotations/{userId}/{companyDB}/{status}/{cancelled}/{dateFrom}/{dateTo}")]
        public async Task<IActionResult> GetSalesOrders(int userId, string companyDB, char status, char cancelled, string dateFrom, string dateTo, Paginate paginate) =>
            Ok(await sqService.GetSalesQuotationsAsync(userId, companyDB, status, cancelled, dateFrom, dateTo, paginate));

        // CREATE SALES QUOTATION
        [HttpPost("CreateSalesQuotation/{userId}/{companyDB}/{forApproval}")]
        public async Task<IActionResult> CreateSalesQuotation(int userId, string companyDB, char forApproval, dynamic SalesOrder) => Ok(await sqService.CreateSalesQuotationAsync(userId, companyDB, forApproval, SalesOrder));

        // UPDATE SALES QUOTATION
        [HttpPost("UpdateSalesQuotation/{userId}/{companyDB}")]
        public async Task<IActionResult> UpdateSalesQuotation(int userId, string companyDB, dynamic SalesOrder) => Ok(await sqService.UpdateSalesQuotationAsync(userId, companyDB, SalesOrder));

        // CANCEL SALES QUOTATION
        [HttpPost("CancelSalesQuotation/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> CancelSalesQuotation(int userId, string companyDB, int docEntry) =>
            Ok(await sqService.CancelSalesQuotationAsync(userId, companyDB, docEntry));

        // CLOSE SALES QUOTATION
        [HttpPost("CloseSalesQuotation/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> CloseSalesQuotation(int userId, string companyDB, int docEntry) =>
            Ok(await sqService.CloseSalesQuotationAsync(userId, companyDB, docEntry));
    }
}
