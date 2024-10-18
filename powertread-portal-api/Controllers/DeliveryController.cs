using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly DeliveryService drService;
        public DeliveryController() => drService = new();

        // GET DELIVERIES
        [HttpPost("GetDeliveries/{userId}/{companyDB}/{status}/{cancelled}/{dateFrom}/{dateTo}")]
        public async Task<IActionResult> GetDeliveries(int userId, string companyDB, char status, char cancelled, string dateFrom, string dateTo, Paginate paginate) =>
            Ok(await drService.GetDeliveriesAsync(userId, companyDB, status, cancelled, dateFrom, dateTo, paginate));

        // GET DELIVERY
        [HttpGet("GetDelivery/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> GetDelivery(int userId, string companyDB, int docEntry) => Ok(await drService.GetDeiveryAsync(userId, companyDB, docEntry));

        // CREATE DELIVERY
        [HttpPost("CreateDelivery/{userId}/{companyDB}/{forApproval}")]
        public async Task<IActionResult> CreateDelivery(int userId, string companyDB, char forApproval, Document delivery) => Ok(await drService.CreateDeliveryAsync(userId, companyDB, forApproval, delivery));

        // UPDATE DELIVERY
        [HttpPost("UpdateDelivery/{userId}/{companyDB}")]
        public async Task<IActionResult> UpdateDelivery(int userId, string companyDB, Document delivery) => Ok(await drService.UpdateDeliveryAsync(userId, companyDB, delivery));

        // CANCEL DELIVERY
        [HttpPost("CancelDelivery/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> CancelDelivery(int userId, string companyDB, int docEntry) =>
            Ok(await drService.CancelDeliveryAsync(userId, companyDB, docEntry));

        // CLOSE DELIVERY
        [HttpPost("CloseDelivery/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> CloseDelivery(int userId, string companyDB, int docEntry) =>
            Ok(await drService.CloseDeliveryAsync(userId, companyDB, docEntry));

        // GET SALES ORDERS
        [HttpGet("GetSalesOrders/{userId}/{companyDB}/{cardCode}/{docType}/{priceMode}")]
        public async Task<IActionResult> GetSalesOrders(int userId, string companyDB, string cardCode, string docType, string priceMode) =>
            Ok(await drService.GetSalesOrdersAsync(userId, companyDB, cardCode, docType, priceMode));
    }
}
