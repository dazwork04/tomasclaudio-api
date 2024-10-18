using B1SLayer;
using Newtonsoft.Json;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Utils;
using SLayerConnectionLib;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SAPB1SLayerWebAPI.Services
{
    public class PurchaseOrderService
    {

        // GET PURCHASE ORDERS
        //GetPurchaseOrders/{userId}/{companyDB}/{status}/{cancelled}/{dateFrom}/{dateTo}
        public async Task<Response> GetPurchaseOrdersAsync(int userId, string companyDB, char status, char cancelled, string dateFrom, string dateTo, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                string queryFilter = $"DocumentStatus eq '{status}' and Cancelled eq '{cancelled}' and DocDate ge '{dateFrom}' and DocDate le '{dateTo}'" + paginate.Filter;


                var count = await connection.Request(EntitiesKeys.PurchaseOrders)
                    .Filter(queryFilter)
                    .GetCountAsync();

                var result = await connection.Request(EntitiesKeys.PurchaseOrders)
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

        // CREATE PURCHASE ORDER
        public async Task<Response> CreatePurchaseOrderAsync(int userId, string companyDB, char forApproval, dynamic purchaseOrder) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                int? prevCode = null;
                try
                {

                    // GET THE APPROVAL CODE -- BEFORE
                    var approvals = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectType eq '22' and IsDraft eq 'Y' and OriginatorID eq {purchaseOrder.UserSign} and ObjectEntry eq null").Top(1).OrderBy("Code desc").GetAsync<List<SLApprovalRequest>>();
                    if (approvals.Count > 0) prevCode = approvals[0].Code;

                    var result = await connection.Request(EntitiesKeys.PurchaseOrders).PostAsync<dynamic>(purchaseOrder);

                    Logger.CreateLog(false, "CREATE PURCHASE ORDER", "SUCCESS", JsonConvert.SerializeObject(purchaseOrder));

                    return new Response
                    {
                        Status = "success",
                        Message = $"PO #{result.DocNum} CREATED successfully!",
                        Payload = result
                    };

                }
                catch (Exception ex)
                {
                    Logger.CreateLog(false, "CREATE PURCHASE ORDER - CATCH", ex.Message, "");
                    if (ex.Message.Contains("no matching records found", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (forApproval == 'Y')
                        {
                            int? newCode = null;
                            var approvals = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectType eq '22' and IsDraft eq 'Y' and OriginatorID eq {purchaseOrder.UserSign} and ObjectEntry eq null").Top(1).OrderBy("Code desc").GetAsync<List<SLApprovalRequest>>();
                            if (approvals.Count > 0) newCode = approvals[0].Code;
                            if (prevCode != newCode)
                            {
                                Logger.CreateLog(false, "CREATE PURCHASE ORDER APPROVAL", "SUCCESS", JsonConvert.SerializeObject(purchaseOrder));
                                return new Response
                                {
                                    Status = "success",
                                    Message = $"PO For Approval CREATED successfully!",
                                };
                            }
                            else
                            {
                                Logger.CreateLog(true, "CREATE PURCHASE ORDER ERROR", ex.Message, JsonConvert.SerializeObject(purchaseOrder));
                                return new Response
                                {
                                    Status = "failed",
                                    Message = ex.Message
                                };
                            }
                        }
                        else
                        {
                            Logger.CreateLog(true, "CREATE PURCHASE ORDER", ex.Message, JsonConvert.SerializeObject(purchaseOrder));
                            return new Response
                            {
                                Status = "failed",
                                Message = ex.Message
                            };
                        }
                    }
                    Logger.CreateLog(true, "CREATE PURCHASE ORDER", ex.Message, JsonConvert.SerializeObject(purchaseOrder));
                    return new Response
                    {
                        Status = "failed",
                        Message = ex.Message
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CREATE PURCHASE ORDER", ex.Message, JsonConvert.SerializeObject(purchaseOrder));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // UPDATE PURCHASE ORDER
        public async Task<Response> UpdatePurchaseOrderAsync(int userId, string companyDB, dynamic purchaseOrder) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                await connection.Request(EntitiesKeys.PurchaseOrders, purchaseOrder.DocEntry).PutAsync(purchaseOrder);

                var result = await connection.Request(EntitiesKeys.PurchaseOrders, purchaseOrder.DocEntry).GetAsync();

                var count = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectEntry eq {result.DocEntry} and IsDraft eq 'Y' and ObjectType eq '22'").GetCountAsync();
                Logger.CreateLog(false, "UPDATE PURCHASE ORDRE - COUNT APPROVAL", "SUCCESS", count.ToString());


                string message = "";
                if (count > 0)
                {
                    
                    message = $"PO For Approval UPDATED successfully!";
                    Logger.CreateLog(false, "UPDATE PURCHASE ORDER REROUTING APPROVAL", "SUCCESS", JsonConvert.SerializeObject(purchaseOrder));
                }
                else
                {
                    Logger.CreateLog(false, "UPDATE PURCHASE ORDER", "SUCCESS", JsonConvert.SerializeObject(purchaseOrder));
                    message = $"PO #{result.DocNum} UPDATED successfully!";
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

                Logger.CreateLog(true, "UPDATE PURCHASE ORDER", ex.Message, JsonConvert.SerializeObject(purchaseOrder));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // CANCEL PURCHASE ORDER
        public async Task<Response> CancelPurchaseOrderAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.PurchaseOrders}({docEntry})/Cancel";

                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.PurchaseOrders, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"PO #{result.DocNum} CANCELED successfully!",
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

        // CLOSE PURCHASE ORDER
        public async Task<Response> ClosePurchaseOrderAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.PurchaseOrders}({docEntry})/Close";
                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.PurchaseOrders, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"PO #{result.DocNum} CLOSED successfully!",
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


        // GET PURCHASE REQUESTS
        public async Task<Response> GetPurchaseRequestsAsync(int userId, string companyDB, string docType, string priceMode) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                var purchaseRequests = await connection.Request(EntitiesKeys.PurchaseRequests)
                    .Filter($"DocType eq '{docType}' and ReqType eq 171 and PriceMode eq '{priceMode}' and DocumentStatus eq 'O'")
                    .GetAllAsync<dynamic>();


                return new Response
                {
                    Status = "success",
                    Payload = purchaseRequests
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
