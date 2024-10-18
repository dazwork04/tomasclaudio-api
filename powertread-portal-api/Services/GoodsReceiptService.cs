using Newtonsoft.Json;
using SAPB1SLayerWebAPI.Models;
using SLayerConnectionLib;
using B1SLayer;
using SAPB1SLayerWebAPI.Utils;

namespace SAPB1SLayerWebAPI.Services
{
    public class GoodsReceiptService
    {
        // GET GOODS RECEIPTS
        public async Task<Response> GetGoodsReceiptsAsync(int userId, string companyDB, string dateFrom, string dateTo, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                string queryFilter = $"DocDate ge '{dateFrom}' and DocDate le '{dateTo}'" + paginate.Filter;

                var count = await connection.Request(EntitiesKeys.InventoryGenEntries)
                    .Filter(queryFilter)
                    .GetCountAsync();

                var result = await connection.Request(EntitiesKeys.InventoryGenEntries)
                    .Filter(queryFilter)
                    .Skip(paginate.Page * paginate.Size)
                    .Top(paginate.Size)
                    .OrderBy($"{orderBy} {paginate.Direction}")
                    .GetAsync<List<dynamic>>();

                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        Count = count,
                        Data = result
                    }
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message,
                    Payload = new List<dynamic>()
                };
            }
        });

        // CREATE GOODS RECEIPT
        public async Task<Response> CreateGoodsReceiptAsync(int userId, string companyDB, dynamic GoodsReceipt) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                await connection.Request(EntitiesKeys.InventoryGenEntries).PostAsync(GoodsReceipt);

                var result = await connection.Request(EntitiesKeys.InventoryGenEntries).OrderBy("DocEntry desc").Top(1).GetAsync<List<dynamic>>();
                var newDR = result.First();

                Logger.CreateLog(false, "CREATE GOODS RECEIPT", "SUCCESS", JsonConvert.SerializeObject(GoodsReceipt));
                return new Response
                {
                    Status = "success",
                    Message = $"GR #{newDR.DocNum} CREATED successfully!",
                    Payload = newDR
                };
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CREATE GOODS RECEIPT", ex.Message, JsonConvert.SerializeObject(GoodsReceipt));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // UPDATE GOODS RECEIPT
        public async Task<Response> UpdateGoodsReceiptAsync(int userId, string companyDB, dynamic GoodsReceipt) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                await connection.Request(EntitiesKeys.InventoryGenEntries, GoodsReceipt.DocEntry).PatchAsync(GoodsReceipt);
                var result = await connection.Request(EntitiesKeys.InventoryGenEntries, GoodsReceipt.DocEntry).GetAsync();
                Logger.CreateLog(false, "UPDATE GOODS RECEIPT", "SUCCESS", JsonConvert.SerializeObject(GoodsReceipt));
                return new Response
                {
                    Status = "success",
                    Message = $"GR #{result.DocNum} UPDATED successfully!",
                    Payload = result
                };
            }
            catch (Exception ex)
            {

                Logger.CreateLog(true, "UPDATE GOODS RECEIPT", ex.Message, JsonConvert.SerializeObject(GoodsReceipt));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });
    }
}
