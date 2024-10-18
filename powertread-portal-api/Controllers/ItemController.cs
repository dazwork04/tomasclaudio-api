using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly ItemService itmService;
        public ItemController() => itmService = new();

        // CREATE ITEM
        [HttpPost("CreateItem/{userId}/{companyDB}")]
        public async Task<IActionResult> CreateItem(int userId, string companyDB, Item item) => Ok(await itmService.CreateItemAsync(userId, companyDB, item));
        // UPDATE ITEM
        [HttpPost("UpdateItem/{userId}/{companyDB}")]
        public async Task<IActionResult> UpdateItem(int userId, string companyDB, Item item) => Ok(await itmService.UpdateItemAsync(userId, companyDB, item));
    }
}
