using Newtonsoft.Json;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Models;
using SLayerConnectionLib;
using B1SLayer;
using SAPB1SLayerWebAPI.Utils;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SAPB1SLayerWebAPI.Services
{
    public class InventoryTransferService
    {
        // GET INVENTORY TRANSFERS
        public async Task<Response> GetInventoryTransfersAsync(int userId, string companyDB, char status, string dateFrom, string dateTo, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                string queryFilter = $"DocumentStatus eq '{status}' and DocDate ge '{dateFrom}' and DocDate le '{dateTo}'" + paginate.Filter;


                var count = await connection.Request(EntitiesKeys.StockTransfers)
                    .Filter(queryFilter)
                    .GetCountAsync();

                var result = await connection.Request(EntitiesKeys.StockTransfers)
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

        // CREATE INVENTORY TRANSFER
        public async Task<Response> CreateInventoryTransfersAsync(int userId, int userSign, string companyDB, char forApproval, dynamic inventoryTransfer) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                int? prevCode = null;
                try
                {
                    // GET THE APPROVAL CODE -- BEFORE
                    var approvals = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectType eq '67' and IsDraft eq 'Y' and OriginatorID eq {userSign} and ObjectEntry eq null").Top(1).OrderBy("Code desc").GetAsync<List<SLApprovalRequest>>();
                    if (approvals.Count > 0) prevCode = approvals[0].Code;

                    var result = await connection.Request(EntitiesKeys.StockTransfers).PostAsync<dynamic>(inventoryTransfer);
                    Logger.CreateLog(false, "CREATE INVENTORY TRANSFER", "SUCCESS", JsonConvert.SerializeObject(inventoryTransfer));

                    return new Response
                    {
                        Status = "success",
                        Message = $"IT #{result.DocNum} CREATED successfully!",
                        Payload = result
                    };

                }
                catch (Exception ex)
                {
                    Logger.CreateLog(false, "CREATE INVENTORY TRANSFER - CATCH", ex.Message, "");
                    if (ex.Message.Contains("no matching records found", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (forApproval == 'Y')
                        {
                            int? newCode = null;
                            var approvals = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectType eq '67' and IsDraft eq 'Y' and OriginatorID eq {inventoryTransfer.UserSign} and ObjectEntry eq null").Top(1).OrderBy("Code desc").GetAsync<List<SLApprovalRequest>>();
                            if (approvals.Count > 0) newCode = approvals[0].Code;
                            if (prevCode != newCode)
                            {
                                Logger.CreateLog(false, "CREATE INVENTORY TRANSFER", "SUCCESS", JsonConvert.SerializeObject(inventoryTransfer));
                                return new Response
                                {
                                    Status = "success",
                                    Message = $"IT For Approval CREATED successfully!",
                                };
                            }
                            else
                            {
                                Logger.CreateLog(true, "CREATE INVENTORY TRANSFER ERROR", ex.Message, JsonConvert.SerializeObject(inventoryTransfer));
                                return new Response
                                {
                                    Status = "failed",
                                    Message = ex.Message
                                };
                            }
                        }
                        else
                        {
                            Logger.CreateLog(true, "CREATE INVENTORY TRANSFER", ex.Message, JsonConvert.SerializeObject(inventoryTransfer));
                            return new Response
                            {
                                Status = "failed",
                                Message = ex.Message
                            };
                        }
                    }
                    Logger.CreateLog(true, "CREATE INVENTORY TRANSFER", ex.Message, JsonConvert.SerializeObject(inventoryTransfer));
                    return new Response
                    {
                        Status = "failed",
                        Message = ex.Message
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CREATE INVENTORY TRANSFER", ex.Message, JsonConvert.SerializeObject(inventoryTransfer));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }

        });

        // UPDATE INVENTORY TRANSFER
        public async Task<Response> UpdateInventoryTransferAsync(int userId, string companyDB, dynamic inventoryTransfer) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                await connection.Request(EntitiesKeys.StockTransfers, inventoryTransfer.DocEntry).PatchAsync(inventoryTransfer);
                var result = await connection.Request(EntitiesKeys.StockTransfers, inventoryTransfer.DocEntry).GetAsync();
                var count = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectEntry eq {result.DocEntry} and IsDraft eq 'Y' and ObjectType eq '67'").GetCountAsync();
                Logger.CreateLog(false, "UPDATE INVENTORY TRANSFER - COUNT APPROVAL", "SUCCESS", count.ToString());


                string message = "";
                if (count > 0)
                {

                    message = $"IT For Approval UPDATED successfully!";
                    Logger.CreateLog(false, "UPDATE INVENTORY TRANSFER REROUTING APPROVAL", "SUCCESS", JsonConvert.SerializeObject(inventoryTransfer));
                }
                else
                {
                    Logger.CreateLog(false, "UPDATE INVENTORY TRANSFER", "SUCCESS", JsonConvert.SerializeObject(inventoryTransfer));
                    message = $"IT #{result.DocNum} UPDATED successfully!";
                }

                return new Response
                {
                    Status = "success",
                    Message = message,
                    Payload = result
                };
            }
            catch (Exception ex)
            {

                Logger.CreateLog(true, "UPDATE INVENTORY TRANSFER", ex.Message, JsonConvert.SerializeObject(inventoryTransfer));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // CANCEL INVENTORY TRANSFER REQUEST
        public async Task<Response> CancelInventoryTransferAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.StockTransfers}({docEntry})/Cancel";

                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.StockTransfers, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"IT #{result.DocNum} CANCELED successfully!",
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
        });

        // GET INVENTORY TRANSFER REQUESTS
        public async Task<Response> GetInventoryTransferRequestsAsync(int userId, string companyDB, List<string> warehouses) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string queryFilter = $"";
                for (int i = 0; i < warehouses.Count; i++)
                {
                    queryFilter += i == 0 ? $" and FromWarehouse eq '{warehouses[i].ToString()}'" : $" or FromWarehouse eq '{warehouses[i].ToString()}'";
                }

                //
                var inventoryTransferRequests = await connection.Request(EntitiesKeys.InventoryTransferRequests)
                    .Filter($"DocumentStatus eq 'O' {queryFilter}")
                    .GetAllAsync<dynamic>();

                for (int i = 0; i < inventoryTransferRequests.Count; i++)
                {
                    var fromWarehouse = await connection.Request(EntitiesKeys.Warehouses, inventoryTransferRequests[i].FromWarehouse.ToString()).GetAsync<Warehouse>();
                    inventoryTransferRequests[i].FromWarehouseName = fromWarehouse.WarehouseName;
                    var toWarehouse = await connection.Request(EntitiesKeys.Warehouses, inventoryTransferRequests[i].ToWarehouse.ToString()).GetAsync<Warehouse>();
                    inventoryTransferRequests[i].ToWarehouseName = toWarehouse.WarehouseName;
                }


                return new Response
                {
                    Status = "success",
                    Payload = inventoryTransferRequests
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

        // GET INVENTORY TRANSFERS COPY FROM
        public async Task<Response> GetInventoryTransfersCopyFromAsync(int userId, string companyDB, List<string> warehouses) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string queryFilter = $"";
                for (int i = 0; i < warehouses.Count; i++)
                {
                    queryFilter += i == 0 ? $" and U_ToWarehouse eq '{warehouses[i].ToString()}'" : $" or U_ToWarehouse eq '{warehouses[i].ToString()}'";
                }

                var inventoryTransfers = await connection.Request(EntitiesKeys.StockTransfers)
                    .Filter($"DocumentStatus eq 'O' and ToWarehouse eq '03' {queryFilter}") // IN TRANSIT
                    .GetAllAsync<dynamic>();

                for (int i = 0; i < inventoryTransfers.Count; i++)
                {
                    var fromWarehouse = await connection.Request(EntitiesKeys.Warehouses, inventoryTransfers[i].FromWarehouse.ToString()).GetAsync<Warehouse>();
                    inventoryTransfers[i].FromWarehouseName = fromWarehouse.WarehouseName;
                    var toWarehouse = await connection.Request(EntitiesKeys.Warehouses, inventoryTransfers[i].ToWarehouse.ToString()).GetAsync<Warehouse>();
                    inventoryTransfers[i].ToWarehouseName = toWarehouse.WarehouseName;
                }


                return new Response
                {
                    Status = "success",
                    Payload = inventoryTransfers
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
