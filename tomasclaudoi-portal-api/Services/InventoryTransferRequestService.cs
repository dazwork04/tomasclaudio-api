using B1SLayer;
using Newtonsoft.Json;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Utils;
using SLayerConnectionLib;

namespace SAPB1SLayerWebAPI.Services
{
    public class InventoryTransferRequestService
    {
        // GET INVENTORY TRANSFER REQUESTS
        public async Task<Response> GetInventoryTransferRequestsAsync(int userId, string companyDB, char status, string dateFrom, string dateTo, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                string queryFilter = $"DocumentStatus eq '{status}' and DocDate ge '{dateFrom}' and DocDate le '{dateTo}'" + paginate.Filter;


                var count = await connection.Request(EntitiesKeys.InventoryTransferRequests)
                    .Filter(queryFilter)
                    .GetCountAsync();

                var result = await connection.Request(EntitiesKeys.InventoryTransferRequests)
                    .Filter(queryFilter)
                    .Skip(paginate.Page * paginate.Size)
                    .Top(paginate.Size)
                    .OrderBy($"{orderBy} {paginate.Direction}")
                    .GetAsync<List<DocumentList>>();

                for (int i = 0; i < result.Count; i++)
                {
                    var fromWarehouse = await connection.Request(EntitiesKeys.Warehouses, result[i].FromWarehouse).GetAsync<Warehouse>();
                    result[i].FromWarehouseName = fromWarehouse.WarehouseName;
                    var toWarehouse = await connection.Request(EntitiesKeys.Warehouses, result[i].ToWarehouse).GetAsync<Warehouse>();
                    result[i].ToWarehouseName = toWarehouse.WarehouseName;
                }

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

        // CREATE INVENTORY TRANSFER REQUEST
        public async Task<Response> CreateInventoryTransferRequestAsync(int userId, string companyDB, dynamic inventoryTransferRequests) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                await connection.Request(EntitiesKeys.InventoryTransferRequests).PostAsync(inventoryTransferRequests);

                var result = await connection.Request(EntitiesKeys.InventoryTransferRequests).OrderBy("DocEntry desc").Top(1).GetAsync<List<dynamic>>();
                var newDR = result.First();

                Logger.CreateLog(false, "CREATE INVENTORY TRANSFER REQUEST", "SUCCESS", JsonConvert.SerializeObject(inventoryTransferRequests));
                return new Response
                {
                    Status = "success",
                    Message = $"ITR #{newDR.DocNum} CREATED successfully!",
                    Payload = newDR
                };
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CREATE INVENTORY TRANSFER REQUEST", ex.Message, JsonConvert.SerializeObject(inventoryTransferRequests));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // UPDATE INVENTORY TRANSFER REQUEST
        public async Task<Response> UpdateInventoryTransferRequestAsync(int userId, string companyDB, dynamic inventoryTransferRequests) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                await connection.Request(EntitiesKeys.InventoryTransferRequests, inventoryTransferRequests.DocEntry).PatchAsync(inventoryTransferRequests);
                var result = await connection.Request(EntitiesKeys.InventoryTransferRequests, inventoryTransferRequests.DocEntry).GetAsync();
                Logger.CreateLog(false, "UPDATE INVENTORY TRANSFER REQUEST", "SUCCESS", JsonConvert.SerializeObject(inventoryTransferRequests));
                return new Response
                {
                    Status = "success",
                    Message = $"ITR #{result.DocNum} UPDATED successfully!",
                    Payload = result
                };
            }
            catch (Exception ex)
            {

                Logger.CreateLog(true, "UPDATE INVENTORY TRANSFER REQUEST", ex.Message, JsonConvert.SerializeObject(inventoryTransferRequests));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // CANCEL INVENTORY TRANSFER REQUEST
      /*  public async Task<Response> CancelinventoryTransferRequestsAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.inventoryTransferRequests}({docEntry})/Cancel";

                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.inventoryTransferRequests, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"ITR #{result.DocNum} CANCELED successfully!",
                    Payload = result,
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
        });*/

        // CLOSE INVENTORY TRANSFER REQUEST
        public async Task<Response> CloseInventoryTransferRequestAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.InventoryTransferRequests}({docEntry})/Close";
                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.InventoryTransferRequests, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"ITR #{result.DocNum} CLOSED successfully!",
                    Payload = result
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
