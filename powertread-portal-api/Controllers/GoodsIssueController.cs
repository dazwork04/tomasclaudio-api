using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoodsIssueController : ControllerBase
    {
        private readonly GoodsIssueService giService;
        public GoodsIssueController() => giService = new();

        // GET GOODS ISSUE
        [HttpPost("GetGoodsIssues/{userId}/{companyDB}/{dateFrom}/{dateTo}")]
        public async Task<IActionResult> GetGoodsIssues(int userId, string companyDB, string dateFrom, string dateTo, Paginate paginate) =>
            Ok(await giService.GetGoodsIssuesAsync(userId, companyDB, dateFrom, dateTo, paginate));

        // CREATE GOODS ISSUE
        [HttpPost("CreateGoodsIssue/{userId}/{companyDB}/{forApproval}")]
        public async Task<IActionResult> CreateGoodIssue(int userId, string companyDB, char forApproval, dynamic goodsReceipt) =>
            Ok(await giService.CreateGoodsIssueAsync(userId, companyDB, forApproval, goodsReceipt));

        // UPDATE GOODS ISSUE
        [HttpPost("UpdateGoodsIssue/{userId}/{companyDB}")]
        public async Task<IActionResult> UpdateGoodsIssue(int userId, string companyDB, dynamic goodsReceipt) =>
            Ok(await giService.UpdateGoodsIssueAsync(userId, companyDB, goodsReceipt));
    }
}
