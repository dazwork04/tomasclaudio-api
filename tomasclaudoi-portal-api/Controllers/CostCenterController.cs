using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CostCenterController : ControllerBase
    {
        private readonly CostCenterService ccService;
        public CostCenterController(IConfiguration configuration) => ccService = new(configuration);

        // CREATE COST CENTER ORDER
        [HttpPost("CreateCostCenter/{userId}/{companyDB}/{userName}/{passWord}")]
        public async Task<IActionResult> CreateCostCenter(int userId, string companyDB, string userName, string passWord, CostCenter costCenter) => Ok(await ccService.CreateCostCenterAsync(userId, companyDB, userName, passWord, costCenter));
    }
}
