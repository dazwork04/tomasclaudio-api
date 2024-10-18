using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Context;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {

        #region VARIABLES
        private readonly ReportService reportService;
        #endregion

        public ReportController(IHttpClientFactory httpClientFactory, AuthDbContext authDbContext) => reportService = new(httpClientFactory, authDbContext);

        [HttpPost("GenerateReport/{userId}/{companyDB}/{docCode}")]
        public async Task<IActionResult> GetReport(int userId, string companyDB, string docCode, dynamic parameters) =>
            Ok(await reportService.GenerateReportAsync(userId, companyDB, docCode, parameters));

    }
}
