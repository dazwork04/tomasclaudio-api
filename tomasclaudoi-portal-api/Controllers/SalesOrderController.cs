using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesOrderController : ControllerBase
    {
        private readonly SalesOrderService soService;
        public SalesOrderController() => soService = new();

        // GET SALES ORDERS
        [HttpPost("GetSalesOrders/{userId}/{companyDB}/{status}/{cancelled}/{dateFrom}/{dateTo}")]
        public async Task<IActionResult> GetSalesOrders(int userId, string companyDB, char status, char cancelled, string dateFrom, string dateTo, Paginate paginate) =>
            Ok(await soService.GetSalesOrdersAsync(userId, companyDB, status, cancelled, dateFrom, dateTo, paginate));

        // CREATE SALES ORDER
        [HttpPost("CreateSalesOrder/{userId}/{companyDB}/{forApproval}")]
        public async Task<IActionResult> CreateSalesOrder(int userId, string companyDB, char forApproval, dynamic SalesOrder) => Ok(await soService.CreateSalesOrderAsync(userId, companyDB, forApproval, SalesOrder));

        // UPDATE SALES ORDER
        [HttpPost("UpdateSalesOrder/{userId}/{companyDB}")]
        public async Task<IActionResult> UpdateSalesOrder(int userId, string companyDB, dynamic SalesOrder) => Ok(await soService.UpdateSalesOrderAsync(userId, companyDB, SalesOrder));

        // CANCEL SALES ORDER
        [HttpPost("CancelSalesOrder/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> CancelSalesOrder(int userId, string companyDB, int docEntry) =>
            Ok(await soService.CancelSalesOrderAsync(userId, companyDB, docEntry));

        // CLOSE SALES ORDER
        [HttpPost("CloseSalesOrder/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> CloseSalesOrder(int userId, string companyDB, int docEntry) =>
            Ok(await soService.CloseSalesOrderAsync(userId, companyDB, docEntry));

        // GET SALES QUOTATIONS
        [HttpGet("GetSalesQuotations/{userId}/{companyDB}/{cardCode}/{docType}/{priceMode}")]
        public async Task<IActionResult> GetSalesQuotations(int userId, string companyDB, string cardCode, string docType, string priceMode) =>
            Ok(await soService.GetSalesQuotationsAsync(userId, companyDB, cardCode, docType, priceMode));
    }
}
