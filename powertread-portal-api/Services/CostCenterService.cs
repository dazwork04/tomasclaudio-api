using B1SLayer;
using Newtonsoft.Json;
using SAPB1SLayerWebAPI.Context;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Utils;
using SLayerConnectionLib;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SAPB1SLayerWebAPI.Services
{
    public class CostCenterService
    {
        #region VARIABLES
        private readonly IConfiguration configuration;
        #endregion
        public CostCenterService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // CREATE COST CENTER
        public async Task<Response> CreateCostCenterAsync(int userId, string companyDB, string userName, string passWord, CostCenter costCenter) => await Task.Run(async () =>
        {
            try
            {
                SLayerConnectionLib.Main.InitConnection(userId, new SLayerConnectionLib.Models.Credential
                {
                    Username = userName,
                    Password = passWord,
                    CompanyDB = companyDB,
                    SLUrl = configuration.GetValue<string>("SL_URL")!
                });

                var connection = SLayerConnectionLib.Main.GetConnection(userId, companyDB);
                await connection.LoginAsync();

                var result = await connection.Request(EntitiesKeys.ProfitCenters).PostAsync<dynamic>(costCenter);

                Logger.CreateLog(false, "CREATE COST CENTER", "SUCCESS", JsonConvert.SerializeObject(costCenter));
                return new Response
                {
                    Status = "success",
                    Message = $"COST CENTER CREATED successfully!",
                    Payload = result
                };
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CREATE COST CENTER", ex.Message, JsonConvert.SerializeObject(costCenter));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        public static async void AddCostCenterFromSO(int userId, string companyDB, string DocNum)
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                CostCenter costCenter = new()
                {
                    CenterCode = DocNum,
                    CenterName = DocNum,
                    InWhichDimension = "4",
                    EffectiveFrom = "20230101"
                };

                await connection.Request(EntitiesKeys.ProfitCenters).PostAsync<dynamic>(costCenter);

                Logger.CreateLog(false, "CREATE COST CENTER", "SUCCESS", JsonConvert.SerializeObject(costCenter));
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CREATE COST CENTER", ex.Message, DocNum);
            }

        }
    }
}
