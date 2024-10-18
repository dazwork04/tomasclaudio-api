using B1SLayer;
using Newtonsoft.Json;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Utils;
using SLayerConnectionLib;

namespace SAPB1SLayerWebAPI.Services
{
    public class ItemService
    {
        // CREATE ITEM
        public async Task<Response> CreateItemAsync(int userId, string companyDB, Item item) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                var result = await connection.Request(EntitiesKeys.Items).PostAsync<dynamic>(item);

                Logger.CreateLog(false, "CREATE ITEM", "SUCCESS", JsonConvert.SerializeObject(item));

                return new Response
                {
                    Status = "success",
                    Message = $"ITEM #{result.ItemCode} CREATED successfully!",
                    Payload = result
                };

            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CREATE ITEM", ex.Message, JsonConvert.SerializeObject(item));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        public async Task<Response> UpdateItemAsync(int userId, string companyDB, Item item) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                await connection.Request(EntitiesKeys.Items, item.ItemCode).PatchAsync(item);

                var result = await connection.Request(EntitiesKeys.Items, item.ItemCode).GetAsync<Item>();

                Logger.CreateLog(false, "UPDATE ITEM", "SUCCESS", JsonConvert.SerializeObject(item));
                return new Response
                {
                    Status = "success",
                    Message = $"ITEM #{result.ItemCode} UPDATED successfully!",
                    Payload = result
                };
            }
            catch (Exception ex)
            {

                Logger.CreateLog(true, "UPDATE ITEM", ex.Message, JsonConvert.SerializeObject(item));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });
    }
}
