using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReturnController : ControllerBase
    {
        private readonly ReturnService arReturnService;
        public ReturnController() => arReturnService = new();

        // GET DELIVERY RETURN
        [HttpPost("GetReturns/{userId}/{companyDB}/{status}/{cancelled}/{dateFrom}/{dateTo}")]
        public async Task<IActionResult> GetReturns(int userId, string companyDB, char status, char cancelled, string dateFrom, string dateTo, Paginate paginate) =>
            Ok(await arReturnService.GetReturnsAsync(userId, companyDB, status, cancelled, dateFrom, dateTo, paginate));

        // CREATE DELIVERY RETURN
        [HttpPost("CreateReturn/{userId}/{companyDB}/{forApproval}")]
        public async Task<IActionResult> CreateReturn(int userId, string companyDB, char forApproval, dynamic arReturn) => Ok(await arReturnService.CreateReturnAsync(userId, companyDB, forApproval, arReturn));

        // UPDATE DELIVERY RETURN
        [HttpPost("UpdateReturn/{userId}/{companyDB}")]
        public async Task<IActionResult> UpdateReturn(int userId, string companyDB, dynamic arReturn) => Ok(await arReturnService.UpdateReturnAsync(userId, companyDB, arReturn));

        // CANCEL DELIVERY RETURN
        [HttpPost("CancelReturn/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> CancelPurchaseOrder(int userId, string companyDB, int docEntry) =>
            Ok(await arReturnService.CancelReturnAsync(userId, companyDB, docEntry));

        // CLOSE DELIVERY RETURN
        [HttpPost("CloseReturn/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> CloseReturn(int userId, string companyDB, int docEntry) =>
            Ok(await arReturnService.CloseReturnAsync(userId, companyDB, docEntry));

        // GET DELIVERIES
        [HttpGet("GetDeliveries/{userId}/{companyDB}/{cardCode}/{docType}/{priceMode}")]
        public async Task<IActionResult> GetPurchaseOrders(int userId, string companyDB, string cardCode, string docType, string priceMode) =>
            Ok(await arReturnService.GetDeliveriesAsync(userId, companyDB, cardCode, docType, priceMode));
    }
}
