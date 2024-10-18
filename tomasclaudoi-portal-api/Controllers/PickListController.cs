using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PickListController : ControllerBase
    {
        private readonly PickListService pickListService;
        public PickListController() => pickListService = new();

        // GET PICK LISTS - userId, companyDB
        [HttpPost("GetPickLists/{userId}/{companyDB}/{baseType}/{status}")] ///{dateFrom}/{dateTo}
        public async Task<IActionResult> GetSeries(int userId, string companyDB, int baseType, string status, Paginate paginate) => //string dateFrom, string dateTo,
            Ok(await pickListService.GetPickListsAsync(userId, companyDB, baseType, status, paginate)); //dateFrom, dateTo,

        // GET PICK LIST - userId, companyDB
        [HttpGet("GetPickList/{userId}/{companyDB}/{actionType}/{absEntry}")]
        public async Task<IActionResult> GetPickList(int userId, string companyDB, char actionType, int absEntry) => Ok(await pickListService.GetPickListAsync(userId, companyDB, actionType, absEntry));

        // UPDATE PICK LIST
        [HttpPost("UpdatePickList/{userId}/{companyDB}")]
        public async Task<IActionResult> UpdatePickList(int userId, string companyDB, SLPickList pickList) => Ok(await pickListService.UpdatePickListAsync(userId, companyDB, pickList));

        // CREATE AUTOMATIC DELIVERY
        [HttpPost("CreateAutomaticDelivery/{userId}/{companyDB}")]
        public async Task<IActionResult> CreateAutomaticDelivery(int userId, string companyDB, SLPickList pickLists) => Ok(await pickListService.CreateAutomaticDeliveryAsync(userId, companyDB, pickLists));

        // GET NEW ISSUE FOR PRODUCTION
        [HttpPost("GetNewIssueForProduction/{userId}/{companyDB}")]
        public async Task<IActionResult> GetNewIssueForProduction(int userId, string companyDB, SLPickList pickList) => Ok(await pickListService.GetNewIssueForProductionAsync(userId, companyDB, pickList));

        // CREATE ISSUE FOR PRODUCTION
        [HttpPost("CreateIssueForProduction/{userId}/{companyDB}")]
        public async Task<IActionResult> CreateIssueForProduction(int userId, string companyDB, dynamic issueForProduction) => Ok(await pickListService.CreateIssueForProductionAsync(userId, companyDB, issueForProduction));

        // GET PICK LIST DELIVERY
        [HttpGet("GetPickListDelivery/{userId}/{companyDB}/{absEntry}/{orderEntries}")]
        public async Task<IActionResult> GetPickListDelivery(int userId, string companyDB, int absEntry, string orderEntries) => Ok(await pickListService.GetPickListDeliveryAsync(userId, companyDB, absEntry, orderEntries));
    }
}
