using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Context;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        #region VARIABLES
        private readonly AuthService authService;
        #endregion

        public AuthController(AuthDbContext authDbContext, SboDbContext sboDbContext, IConfiguration configuration) => authService = new(authDbContext, sboDbContext, configuration);

        //
        [HttpGet]
        public IActionResult Get() => Ok("Welcome to SAP SLayer Web API");

        // LOGIN
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginCredential loginCred) => Ok(await authService.LoginAsync(loginCred));

        // LOGOUT - userId
        [HttpPost("Logout/{userId}")]
        public async Task<IActionResult> Logout(int userId) => Ok(await authService.LogoutAsync(userId));

        // GET COMPANIES
        [HttpGet("GetCompanies")]
        public async Task<IActionResult> GetCompanies() => Ok(await authService.GetCompanies());

        // GET CONNECTIOM
        [HttpGet("CheckConnection/{userId}/{companyDB}")]
        public async Task<IActionResult> CheckConnection(int userId, string companyDB) => Ok(await authService.CheckConnection(userId, companyDB));

    }
}
