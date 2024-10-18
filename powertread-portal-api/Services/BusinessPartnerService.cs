using B1SLayer;
using Newtonsoft.Json;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Utils;
using SLayerConnectionLib;

namespace SAPB1SLayerWebAPI.Services
{
    public class BusinessPartnerService
    {
        // CREATE BUSINESS PARTNER
        public async Task<Response> CreateBusinessPartnerAsync(int userId, string companyDB, BusinessPartner businessPartner) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                var result = await connection.Request(EntitiesKeys.BusinessPartners).PostAsync<dynamic>(businessPartner);

                Logger.CreateLog(false, "CREATE BUSINESS PARTNER", "SUCCESS", JsonConvert.SerializeObject(businessPartner));

                return new Response
                {
                    Status = "success",
                    Message = $"BP #{result.CardCode} CREATED successfully!",
                    Payload = result
                };

            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CREATE BUSINESS PARTNER", ex.Message, JsonConvert.SerializeObject(businessPartner));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        public async Task<Response> UpdateBusinessPartnerAsync(int userId, string companyDB, BusinessPartner businessPartner) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                await connection.Request(EntitiesKeys.BusinessPartners, businessPartner.CardCode).PutAsync(businessPartner);

                //await connection.Request(EntitiesKeys.PurchaseOrders, purchaseOrder.DocEntry).PutAsync(purchaseOrder);

                var result = await connection.Request(EntitiesKeys.BusinessPartners, businessPartner.CardCode).GetAsync();
                Logger.CreateLog(false, "UPDATE BUSINESS PARTNER", "SUCCESS", JsonConvert.SerializeObject(businessPartner));
                return new Response
                {
                    Status = "success",
                    Message = $"BP #{result.CardCode} UPDATED successfully!",
                    Payload = result
                };
            }
            catch (Exception ex)
            {

                Logger.CreateLog(true, "UPDATE BUSINESS PARTNER", ex.Message, JsonConvert.SerializeObject(businessPartner));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });
    }
}
