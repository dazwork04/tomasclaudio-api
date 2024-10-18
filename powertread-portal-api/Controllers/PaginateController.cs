using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Context;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaginateController : ControllerBase
    {
        private readonly PaginateService paginateService;
        public PaginateController(MainDbContext mainDbContext) => paginateService = new(mainDbContext);

        // GET BUSINESS PARTNERS PAGINATE
        [HttpPost("GetBusinessPartners/{userId}/{companyDB}/{cardType}")]
        public async Task<IActionResult> GetBusinessPartners(int userId, string companyDB, string cardType, Paginate paginate) =>
             Ok(await paginateService.GetBusinessPartnersAsync(userId, companyDB, cardType, paginate));

        // GET ITEMS PAGINATE
        [HttpPost("GetItems/{userId}/{companyDB}/{itemsGroupCode}/{type}")]
        public async Task<IActionResult> GetItems(int userId, string companyDB, int itemsGroupCode, char type, Paginate paginate) =>
             Ok(await paginateService.GetItemsAsync(userId, companyDB, itemsGroupCode, type, paginate));

        // GET CHART OF ACCOUNTS
        [HttpPost("GetChartOfAccounts/{userId}/{companyDB}")]
        public async Task<IActionResult> GetChartOfAccounts(int userId, string companyDB, Paginate paginate) =>
             Ok(await paginateService.GetChartOfAccountsAsync(userId, companyDB, paginate));

        // GET WAREHOUSES
        [HttpPost("GetWarehouses/{userId}/{companyDB}")]
        public async Task<IActionResult> GetWarehouses(int userId, string companyDB, Paginate paginate) =>
             Ok(await paginateService.GetWarehousesAsync(userId, companyDB, paginate));

        //GET COSTING CODE
        [HttpPost("GetCostingCodes/{userId}/{companyDB}/{dimCode}")]
        public async Task<IActionResult> GetCostingCodes(int userId, string companyDB, int dimCode, Paginate paginate) =>
             Ok(await paginateService.GetCostingCodesAsync(userId, companyDB, dimCode, paginate));

        // GET BUSINESS PARTNERS PAGINATE
        [HttpPost("GetBusinessPartners1/{userId}/{companyDB}")]
        public async Task<IActionResult> GetBusinessPartners1(int userId, string companyDB, Paginate paginate) =>
             Ok(await paginateService.GetBusinessPartnersAsync1(userId, companyDB, paginate));

        // GET PAYMENT DRAFTS
        [HttpPost("GetPaymentDrafts/{userId}/{companyDB}/{dateFrom}/{dateTo}/{objectType}")]
        public async Task<IActionResult> GetPaymentDrafts(int userId, string companyDB, string dateFrom, string dateTo, int objectType, Paginate paginate) =>
             Ok(await paginateService.GetPaymentDraftsAsync(userId, companyDB, dateFrom, dateTo, objectType, paginate));

        // GET JO REFERENCES PAGINATE
        [HttpPost("GetJOReferences/{userId}/{companyDB}/{objType}")]
        public async Task<IActionResult> GetJOReferences(int userId, string companyDB, int objType, Paginate paginate) =>
             Ok(await paginateService.GetJOReferencesAsync(userId, companyDB, objType, paginate));

    }
}
