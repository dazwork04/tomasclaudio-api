using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly HomeService homeService;
        public HomeController() => homeService = new();

        // GET DOCUMENTS COUNT - userId, companyDB
        [HttpGet("GetDocumentsCount/{userId}/{companyDB}")]
        public async Task<IActionResult> GetDocumentsCount(int userId, string companyDB) =>
            Ok(await homeService.GetDocumentsCount(userId, companyDB));
    }
}
