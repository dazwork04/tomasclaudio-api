using Newtonsoft.Json;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Models;
using SLayerConnectionLib;
using B1SLayer;
using SAPB1SLayerWebAPI.Utils;

namespace SAPB1SLayerWebAPI.Services
{
    public class DeliveryService
    {
        // GET DELIVERIES
        public async Task<Response> GetDeliveriesAsync(int userId, string companyDB, char status, char cancelled, string dateFrom, string dateTo, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                string queryFilter = $"DocumentStatus eq '{status}' and Cancelled eq '{cancelled}' and DocDate ge '{dateFrom}' and DocDate le '{dateTo}'" + paginate.Filter;


                var count = await connection.Request(EntitiesKeys.DeliveryNotes)
                    .Filter(queryFilter)
                    .GetCountAsync();

                var result = await connection.Request(EntitiesKeys.DeliveryNotes)
                    .Filter(queryFilter)
                    .Skip(paginate.Page * paginate.Size)
                    .Top(paginate.Size)
                    .OrderBy($"{orderBy} {paginate.Direction}")
                    .GetAsync<List<DocumentList>>();

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

        // GET DELIVERY
        public async Task<Response> GetDeiveryAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                var result = await connection.Request(EntitiesKeys.DeliveryNotes, docEntry).GetAsync<dynamic>();

                return new Response
                {
                    Status = "success",
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

        // CREATE DELIVERY
        public async Task<Response> CreateDeliveryAsync(int userId, string companyDB, char forApproval, dynamic delivery) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                int? prevCode = null;
                try
                {
                    // GET THE APPROVAL CODE -- BEFORE
                    var approvals = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectType eq '15' and IsDraft eq 'Y' and OriginatorID eq {delivery.UserSign} and ObjectEntry eq null").Top(1).OrderBy("Code desc").GetAsync<List<SLApprovalRequest>>();
                    if (approvals.Count > 0) prevCode = approvals[0].Code;

                    var result = await connection.Request(EntitiesKeys.DeliveryNotes).PostAsync<dynamic>(delivery);
                    Logger.CreateLog(false, "CREATE DELIVERY", "SUCCESS", JsonConvert.SerializeObject(delivery));

                    return new Response
                    {
                        Status = "success",
                        Message = $"DR #{result.DocNum} CREATED successfully!",
                        Payload = result
                    };

                }
                catch (Exception ex)
                {
                    Logger.CreateLog(false, "CREATE DELIVERY - CATCH", ex.Message, "");
                    if (ex.Message.Contains("no matching records found", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (forApproval == 'Y')
                        {
                            int? newCode = null;
                            var approvals = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectType eq '15' and IsDraft eq 'Y' and OriginatorID eq {delivery.UserSign} and ObjectEntry eq null").Top(1).OrderBy("Code desc").GetAsync<List<SLApprovalRequest>>();
                            if (approvals.Count > 0) newCode = approvals[0].Code;
                            if (prevCode != newCode)
                            {
                                Logger.CreateLog(false, "CREATE DELIVERY APPROVAL", "SUCCESS", JsonConvert.SerializeObject(delivery));
                                return new Response
                                {
                                    Status = "success",
                                    Message = $"DR For Approval CREATED successfully!",
                                };
                            }
                            else
                            {
                                Logger.CreateLog(true, "CREATE DELIVERY ERROR", ex.Message, JsonConvert.SerializeObject(delivery));
                                return new Response
                                {
                                    Status = "failed",
                                    Message = ex.Message
                                };
                            }
                        }
                        else
                        {
                            Logger.CreateLog(true, "CREATE DELIVERY", ex.Message, JsonConvert.SerializeObject(delivery));
                            return new Response
                            {
                                Status = "failed",
                                Message = ex.Message
                            };
                        }
                    }
                    Logger.CreateLog(true, "CREATE DELIVERY", ex.Message, JsonConvert.SerializeObject(delivery));
                    return new Response
                    {
                        Status = "failed",
                        Message = ex.Message
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CREATE DELIVERY", ex.Message, JsonConvert.SerializeObject(delivery));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // UPDATE DELIVERY
        public async Task<Response> UpdateDeliveryAsync(int userId, string companyDB, dynamic delivery) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                await connection.Request(EntitiesKeys.DeliveryNotes, delivery.DocEntry).PatchAsync(delivery);
                var result = await connection.Request(EntitiesKeys.DeliveryNotes, delivery.DocEntry).GetAsync();
                var count = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectEntry eq {result.DocEntry} and IsDraft eq 'Y' and ObjectType eq '15'").GetCountAsync();
                Logger.CreateLog(false, "UPDATE DELIVERY - COUNT APPROVAL", "SUCCESS", count.ToString());


                string message = "";
                if (count > 0)
                {

                    message = $"DR For Approval UPDATED successfully!";
                    Logger.CreateLog(false, "UPDATE DELIVERY REROUTING APPROVAL", "SUCCESS", JsonConvert.SerializeObject(delivery));
                }
                else
                {
                    Logger.CreateLog(false, "UPDATE DELIVERY", "SUCCESS", JsonConvert.SerializeObject(delivery));
                    message = $"DR #{result.DocNum} UPDATED successfully!";
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

                Logger.CreateLog(true, "UPDATE DELIVERY", ex.Message, JsonConvert.SerializeObject(delivery));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // CANCEL DELIVERY
        public async Task<Response> CancelDeliveryAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.DeliveryNotes}({docEntry})/Cancel";

                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.DeliveryNotes, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"DR #{result.DocNum} CANCELED successfully!",
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

        // CLOSE DELIVERY
        public async Task<Response> CloseDeliveryAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.DeliveryNotes}({docEntry})/Close";
                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.DeliveryNotes, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"DR #{result.DocNum} CLOSED successfully!",
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

        // GET SALES ORDERS
        public async Task<Response> GetSalesOrdersAsync(int userId, string companyDB, string cardCode, string docType, string priceMode) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                var salesOrders = await connection.Request(EntitiesKeys.Orders)
                    .Filter($"CardCode eq '{cardCode}' and DocType eq '{docType}' and PriceMode eq '{priceMode}' and DocumentStatus eq 'O'")
                    .GetAllAsync<dynamic>();


                return new Response
                {
                    Status = "success",
                    Payload = salesOrders
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
