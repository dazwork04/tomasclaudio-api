using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Context;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region VARIABLES
        private readonly UserService userService;
        #endregion

        public UserController(AuthDbContext authDbContext) => userService = new(authDbContext);

        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetUsers() => Ok(await userService.GetUsersAsync());

        [HttpGet("GetEmployees/{userId}/{companyDB}")]
        public async Task<IActionResult> GetEmployees(int userId, string companyDB) => Ok(await userService.GetEmployeesAsync(userId, companyDB));

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser(User user) => Ok(await userService.CreateUserAsync(user));

        [HttpPost("UpdateUser")]
        public async Task<IActionResult> UpdateUser(User user) => Ok(await userService.UpdateUserAsync(user));
    }
}
