using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryTransferRequestController : ControllerBase
    {
        private readonly InventoryTransferRequestService itrService;
        public InventoryTransferRequestController() => itrService = new();

        // GET INVENTORY TRANSFER REQUESTS
        [HttpPost("GetInventoryTransferRequests/{userId}/{companyDB}/{status}/{dateFrom}/{dateTo}")]
        public async Task<IActionResult> GetInventoryTransferRequests(int userId, string companyDB, char status, string dateFrom, string dateTo, Paginate paginate) =>
            Ok(await itrService.GetInventoryTransferRequestsAsync(userId, companyDB, status, dateFrom, dateTo, paginate));

        // CREATE INVENTORY TRANSFER REQUEST
        [HttpPost("CreateInventoryTransferRequest/{userId}/{companyDB}")]
        public async Task<IActionResult> CreateInventoryTransferRequest(int userId, string companyDB, dynamic inventoryRequestTransfer) => 
            Ok(await itrService.CreateInventoryTransferRequestAsync(userId, companyDB, inventoryRequestTransfer));

        // UPDATE INVENTORY TRANSFER REQUEST
        [HttpPost("UpdateInventoryTransferRequest/{userId}/{companyDB}")]
        public async Task<IActionResult> UpdateInventoryTransferRequest(int userId, string companyDB, dynamic inventoryRequestTransfer) => 
            Ok(await itrService.UpdateInventoryTransferRequestAsync(userId, companyDB, inventoryRequestTransfer));

        // CLOSE INVENTORY TRANSFER REQUEST
        [HttpPost("CloseInventoryTransferRequest/{userId}/{companyDB}/{docEntry}")]
        public async Task<IActionResult> CloseInventoryTransferRequest(int userId, string companyDB, int docEntry) =>
            Ok(await itrService.CloseInventoryTransferRequestAsync(userId, companyDB, docEntry));
    }
}
