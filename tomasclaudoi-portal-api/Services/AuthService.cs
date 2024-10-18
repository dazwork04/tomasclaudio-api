using Microsoft.EntityFrameworkCore;
using SAPB1SLayerWebAPI.Context;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;

namespace SAPB1SLayerWebAPI.Services
{
    public class AuthService
    {
        #region VARIABLES
        private readonly AuthDbContext authDbContext;
        private readonly SboDbContext sboDbContext;
        private readonly MainDbContext mainDbContext;
        private readonly IConfiguration configuration;
        #endregion
        public AuthService(AuthDbContext authDbContext, SboDbContext sboDbContext, IConfiguration configuration, MainDbContext mainDbContext)
        {
            this.authDbContext = authDbContext;
            this.sboDbContext = sboDbContext;
            this.configuration = configuration;
            this.mainDbContext = mainDbContext;
        }

        // LOGIN
        //public Task<Response> LoginAsync(LoginCredential loginCred) => Task.Run(async () =>
        public Task<Response> LoginAsync() => Task.Run(async () =>
        {
            try
            {
                int sap_id = Int32.Parse(configuration.GetValue<string>("SAP_CRED:ID")!);
                var sap_code = configuration.GetValue<string>("SAP_CRED:CODE")!;
                var sap_pass = configuration.GetValue<string>("SAP_CRED:PASS")!;
                var sap_db = configuration.GetValue<string>("SAP_CRED:DB")!;

                SLayerConnectionLib.Main.InitConnection(sap_id, new SLayerConnectionLib.Models.Credential
                {
                    Username = sap_code,
                    Password = sap_pass,
                    CompanyDB = sap_db,
                    SLUrl = configuration.GetValue<string>("SL_URL")!
                });

                var connection = SLayerConnectionLib.Main.GetConnection(sap_id, sap_db);
                await connection.LoginAsync();

                return new Response
                {
                    Status = "success",
                    Payload = connection
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });
        //public Task<Response> LoginAsync(LoginCredential loginCred) => Task.Run(async () =>
        //{
        //    try
        //    {
        //        User? user = await authDbContext.OUSR
        //        .Where(
        //            u => u.UserCode == loginCred.Username
        //            && u.UserPass == loginCred.Password
        //            && u.Status == "active")
        //        .Select(u => new User
        //        {
        //            Id = u.Id,
        //            UserId = u.UserId,
        //            EmpId = u.EmpId,
        //            Name = u.Name,
        //            UserCode = u.UserCode,
        //            UserPass = u.UserPass,
        //            EmpCode = u.EmpCode,
        //            SapUser = u.SapUser,
        //            SapPass = u.SapPass,
        //            Modules = u.Modules,
        //            Status = u.Status,
        //            WhseCode = u.WhseCode,
        //            Branch = u.Branch,
        //        })
        //        .FirstOrDefaultAsync();

        //        if (user != null)
        //        {
        //            SLayerConnectionLib.Main.InitConnection(user.Id, new SLayerConnectionLib.Models.Credential
        //            {
        //                Username = user.SapUser,
        //                Password = user.SapPass,
        //                CompanyDB = loginCred.CompanyDB,
        //                SLUrl = configuration.GetValue<string>("SL_URL")!
        //            });

        //            var connection = SLayerConnectionLib.Main.GetConnection(user.Id, loginCred.CompanyDB);
        //            await connection.LoginAsync();

        //            return new Response
        //            {
        //                Status = "success",
        //                Payload = user
        //            };
        //        }
        //        return new Response
        //        {
        //            Status = "failed",
        //            Message = "Login failed: Invalid credentials."
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new Response
        //        {
        //            Status = "failed",
        //            Message = ex.Message
        //        };
        //    }
        //});

        // LOGOUT
        public Task<Response> LogoutAsync(int userId) => Task.Run(() =>
        {
            try
            {
                SLayerConnectionLib.Main.RemoveConnetion(userId);

                return new Response
                {
                    Status = "success",
                    Message = "Logout successful."
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }

        });

        // GET COMPANIES
        public Task<List<Company>> GetCompanies() => Task.Run(() => sboDbContext.SRGC.Select(s => new Company
        {
            Database = s.DbName,
            Name = s.CmpName
        }).ToList()); //.Where(s => s.DbName == "GSI_TEST")

        // CHECK CONNECTION
        public Task<Response> CheckConnection(int userId, string companyDB) => Task.Run(async () =>
        {
            try
            {
                var connection = SLayerConnectionLib.Main.GetConnection(userId, companyDB);
                await connection.LoginAsync();

                User? user = await authDbContext.OUSR
               .Where(u => u.Id == userId)
               .Select(u => new User
               {
                   Id = u.Id,
                   UserId = u.UserId,
                   EmpId = u.EmpId,
                   Name = u.Name,
                   UserCode = u.UserCode,
                   UserPass = u.UserPass,
                   EmpCode = u.EmpCode,
                   SapUser = u.SapUser,
                   SapPass = u.SapPass,
                   Modules = u.Modules,
                   Status = u.Status,
                   WhseCode = u.WhseCode,
                   Branch = u.Branch,
               })
               .FirstOrDefaultAsync();

                return new Response
                {
                    Status = "success",
                    Payload = user
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });
    }
}
