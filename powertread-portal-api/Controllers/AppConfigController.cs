using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppConfigController : ControllerBase
    {
        #region VARIABLES
        private readonly AppConfigService appConfigService;
        #endregion
        public AppConfigController() => appConfigService = new();

        [HttpGet("GetAppConfig/{userId}/{companyDB}")]
        public async Task<IActionResult> GetAppConfig(int userId, string companyDB) => Ok(await appConfigService.GetAppConfigAsync(userId, companyDB));

    }
}
