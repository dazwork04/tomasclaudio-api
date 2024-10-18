using B1SLayer;
using Newtonsoft.Json;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Utils;
using SLayerConnectionLib;
using System.Reflection.Metadata;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace SAPB1SLayerWebAPI.Services
{
    public class PurchaseRequestService
    {
        // GET PURCHASE REQUESTS
        public async Task<Response> GetPurchaseRequestsAsync(int userId, string companyDB, char status, char cancelled, string dateFrom, string dateTo, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                string queryFilter = $"DocumentStatus eq '{status}' and Cancelled eq '{cancelled}' and DocDate ge '{dateFrom}' and DocDate le '{dateTo}'" + paginate.Filter;


                var count = await connection.Request(EntitiesKeys.PurchaseRequests)
                    .Filter(queryFilter)
                    .GetCountAsync();

                var result = await connection.Request(EntitiesKeys.PurchaseRequests)
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

        // CREATE PURCHASE REQUEST
        public async Task<Response> CreatePurchaseRequestAsync(int userId, string companyDB, char forApproval, dynamic purchaseRequest) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                int? prevCode = null;
                try
                {
                    // GET THE APPROVAL CODE -- BEFORE
                    var approvals = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectType eq '1470000113' and IsDraft eq 'Y' and OriginatorID eq {purchaseRequest.UserSign} and ObjectEntry eq null").Top(1).OrderBy("Code desc").GetAsync<List<SLApprovalRequest>>();
                    if (approvals.Count > 0) prevCode = approvals[0].Code;

                    var result = await connection.Request(EntitiesKeys.PurchaseRequests).PostAsync<dynamic>(purchaseRequest);
                    Logger.CreateLog(false, "CREATE PURCHASE REQUEST", "SUCCESS", JsonConvert.SerializeObject(purchaseRequest));

                    return new Response
                    {
                        Status = "success",
                        Message = $"PR #{result.DocNum} CREATED successfully!",
                        Payload = result
                    };

                }
                catch (Exception ex)
                {
                    Logger.CreateLog(false, "CREATE PURCHASE REQUEST - CATCH", ex.Message, "");
                    if (ex.Message.Contains("no matching records found", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (forApproval == 'Y')
                        {
                            int? newCode = null;
                            var approvals = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectType eq '1470000113' and IsDraft eq 'Y' and OriginatorID eq {purchaseRequest.UserSign} and ObjectEntry eq null").Top(1).OrderBy("Code desc").GetAsync<List<SLApprovalRequest>>();
                            if (approvals.Count > 0) newCode = approvals[0].Code;
                            if (prevCode != newCode)
                            {
                                Logger.CreateLog(false, "CREATE PURCHASE REQUEST APPROVAL", "SUCCESS", JsonConvert.SerializeObject(purchaseRequest));
                                return new Response
                                {
                                    Status = "success",
                                    Message = $"PR For Approval CREATED successfully!",
                                };
                            }
                            else
                            {
                                Logger.CreateLog(true, "CREATE PURCHASE REQUEST ERROR", ex.Message, JsonConvert.SerializeObject(purchaseRequest));
                                return new Response
                                {
                                    Status = "failed",
                                    Message = ex.Message
                                };
                            }
                        }
                        else
                        {
                            Logger.CreateLog(true, "CREATE PURCHASE REQUEST", ex.Message, JsonConvert.SerializeObject(purchaseRequest));
                            return new Response
                            {
                                Status = "failed",
                                Message = ex.Message
                            };
                        }
                    }
                    Logger.CreateLog(true, "CREATE PURCHAE REQUEST", ex.Message, JsonConvert.SerializeObject(purchaseRequest));
                    return new Response
                    {
                        Status = "failed",
                        Message = ex.Message
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CREATE PURCHAE REQUEST", ex.Message, JsonConvert.SerializeObject(purchaseRequest));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // UPDATE PURCHASE REQUEST
        public async Task<Response> UpdatePurchaseRequestAsync(int userId, string companyDB, dynamic purchaseRequest) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                await connection.Request(EntitiesKeys.PurchaseRequests, purchaseRequest.DocEntry).PutAsync(purchaseRequest);


                var result = await connection.Request(EntitiesKeys.PurchaseRequests, purchaseRequest.DocEntry).GetAsync();
                var count = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectEntry eq {result.DocEntry} and IsDraft eq 'Y' and ObjectType eq '1470000113'").GetCountAsync();
                Logger.CreateLog(false, "UPDATE PURCHASE REQUEST - COUNT APPROVAL", "SUCCESS", count.ToString());


                string message = "";
                if (count > 0)
                {
                    message = $"PR For Approval UPDATED successfully!";
                    Logger.CreateLog(false, "UPDATE PURCHASE REQUEST REROUTING APPROVAL", "SUCCESS", JsonConvert.SerializeObject(purchaseRequest));
                }
                else
                {
                    Logger.CreateLog(false, "UPDATE PURCHASE REQUEST", "SUCCESS", JsonConvert.SerializeObject(purchaseRequest));
                    message = $"PR #{result.DocNum} UPDATED successfully!";
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

                Logger.CreateLog(true, "UPDATE PURCHASE REQUEST", ex.Message, JsonConvert.SerializeObject(purchaseRequest));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // CANCEL PURCHASE REQUEST
        public async Task<Response> CancelPurchaseRequestAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.PurchaseRequests}({docEntry})/Cancel";

                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.PurchaseRequests, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"PR #{result.DocNum} CANCELED successfully!",
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

        // CLOSE PURCHASE REQUEST
        public async Task<Response> ClosePurchaseRequestAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.PurchaseRequests}({docEntry})/Close";
                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.PurchaseRequests, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"PR #{result.DocNum} CLOSED successfully!",
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
