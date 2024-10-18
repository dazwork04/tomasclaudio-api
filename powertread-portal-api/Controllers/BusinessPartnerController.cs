using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessPartnerController : ControllerBase
    {
        private readonly BusinessPartnerService bpService;
        public BusinessPartnerController() => bpService = new();

        // CREATE PURCHASE ORDER
        [HttpPost("CreateBusinessPartner/{userId}/{companyDB}")]
        public async Task<IActionResult> CreateBusinessPartner(int userId, string companyDB, BusinessPartner businessPartner) => Ok(await bpService.CreateBusinessPartnerAsync(userId, companyDB, businessPartner));
        // UPDATE PURCHASE ORDER
        [HttpPost("UpdateBusinessPartner/{userId}/{companyDB}")]
        public async Task<IActionResult> UpdateBusinessPartner(int userId, string companyDB, BusinessPartner businessPartner) => Ok(await bpService.UpdateBusinessPartnerAsync(userId, companyDB, businessPartner));
    }
}
