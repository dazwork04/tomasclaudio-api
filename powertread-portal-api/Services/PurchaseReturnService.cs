using Newtonsoft.Json;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Models;
using SLayerConnectionLib;
using B1SLayer;
using SAPB1SLayerWebAPI.Utils;


namespace SAPB1SLayerWebAPI.Services
{
    public class PurchaseReturnService
    {
        // GET GOODS RETURNS
        public async Task<Response> GetPurchaseReturnsAsync(int userId, string companyDB, char status, char cancelled, string dateFrom, string dateTo, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                string queryFilter = $"DocumentStatus eq '{status}' and Cancelled eq '{cancelled}' and DocDate ge '{dateFrom}' and DocDate le '{dateTo}'" + paginate.Filter;


                var count = await connection.Request(EntitiesKeys.PurchaseReturns)
                    .Filter(queryFilter)
                    .GetCountAsync();

                var result = await connection.Request(EntitiesKeys.PurchaseReturns)
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

        public async Task<Response> CreatePurchaseReturnAsync(int userId, string companyDB, char forApproval, dynamic purchaseReturn) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                int? prevCode = null;
                try
                {
                    // GET THE APPROVAL CODE -- BEFORE
                    var approvals = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectType eq '21' and IsDraft eq 'Y' and OriginatorID eq {purchaseReturn.UserSign} and ObjectEntry eq null").Top(1).OrderBy("Code desc").GetAsync<List<SLApprovalRequest>>();
                    if (approvals.Count > 0) prevCode = approvals[0].Code;

                    var result = await connection.Request(EntitiesKeys.PurchaseReturns).PostAsync<dynamic>(purchaseReturn);
                    Logger.CreateLog(false, "CREATE GOODS RETURN", "SUCCESS", JsonConvert.SerializeObject(purchaseReturn));

                    return new Response
                    {
                        Status = "success",
                        Message = $"PRT #{result.DocNum} CREATED successfully!",
                        Payload = result
                    };

                }
                catch (Exception ex)
                {
                    Logger.CreateLog(false, "CREATE GOODS RETURN - CATCH", ex.Message, "");
                    if (ex.Message.Contains("no matching records found", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (forApproval == 'Y')
                        {
                            int? newCode = null;
                            var approvals = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectType eq '21' and IsDraft eq 'Y' and OriginatorID eq {purchaseReturn.UserSign} and ObjectEntry eq null").Top(1).OrderBy("Code desc").GetAsync<List<SLApprovalRequest>>();
                            if (approvals.Count > 0) newCode = approvals[0].Code;
                            if (prevCode != newCode)
                            {
                                Logger.CreateLog(false, "CREATE GOODS RETURN APPROVAL", "SUCCESS", JsonConvert.SerializeObject(purchaseReturn));
                                return new Response
                                {
                                    Status = "success",
                                    Message = $"PRT For Approval CREATED successfully!",
                                };
                            }
                            else
                            {
                                Logger.CreateLog(true, "CREATE GOODS RETURN ERROR", ex.Message, JsonConvert.SerializeObject(purchaseReturn));
                                return new Response
                                {
                                    Status = "failed",
                                    Message = ex.Message
                                };
                            }
                        }
                        else
                        {
                            Logger.CreateLog(true, "CREATE GOODS RETURN", ex.Message, JsonConvert.SerializeObject(purchaseReturn));
                            return new Response
                            {
                                Status = "failed",
                                Message = ex.Message
                            };
                        }
                    }
                    Logger.CreateLog(true, "CREATE GOODS RETURN", ex.Message, JsonConvert.SerializeObject(purchaseReturn));
                    return new Response
                    {
                        Status = "failed",
                        Message = ex.Message
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CREATE GOODS RETURN", ex.Message, JsonConvert.SerializeObject(purchaseReturn));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // UPDATE GOODS RETURN
        public async Task<Response> UpdatePurchaseReturnAsync(int userId, string companyDB, dynamic purchaseReturn) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                await connection.Request(EntitiesKeys.PurchaseReturns, purchaseReturn.DocEntry).PatchAsync(purchaseReturn);
                var result = await connection.Request(EntitiesKeys.PurchaseReturns, purchaseReturn.DocEntry).GetAsync();

                var count = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectEntry eq {result.DocEntry} and IsDraft eq 'Y' and ObjectType eq '21'").GetCountAsync();
                Logger.CreateLog(false, "UPDATE GOODS RETURN - COUNT APPROVAL", "SUCCESS", count.ToString());


                string message = "";
                if (count > 0)
                {

                    message = $"PRT For Approval UPDATED successfully!";
                    Logger.CreateLog(false, "UPDATE GOODS RETURN REROUTING APPROVAL", "SUCCESS", JsonConvert.SerializeObject(purchaseReturn));
                }
                else
                {
                    Logger.CreateLog(false, "UPDATE GOODS RETURN", "SUCCESS", JsonConvert.SerializeObject(purchaseReturn));
                    message = $"PRT #{result.DocNum} UPDATED successfully!";
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

                Logger.CreateLog(true, "UPDATE GOODS RETURN", ex.Message, JsonConvert.SerializeObject(purchaseReturn));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // CANCEL GOODS RETURN
        public async Task<Response> CancelPurchaseReturnAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.PurchaseReturns}({docEntry})/Cancel";

                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.PurchaseReturns, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"PRT #{result.DocNum} CANCELED successfully!",
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

        // CLOSE GOODS RETURN
        public async Task<Response> ClosePurchaseReturnAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.PurchaseReturns}({docEntry})/Close";
                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.PurchaseReturns, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"PRT #{result.DocNum} CLOSED successfully!",
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

        // GET GRPOs
        public async Task<Response> GetGoodsReceiptPOsAsync(int userId, string companyDB, string cardCode, string docType, string priceMode) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                var goodsReceiptPOs = await connection.Request(EntitiesKeys.PurchaseDeliveryNotes)
                    .Filter($"CardCode eq '{cardCode}' and DocType eq '{docType}' and PriceMode eq '{priceMode}' and DocumentStatus eq 'O'")
                    .GetAllAsync<Document>();

                List<dynamic> grpoResults = [];
                foreach (var grpo in goodsReceiptPOs)
                {
                    var result = await connection.Request(EntitiesKeys.PurchaseDeliveryNotes, grpo.DocEntry).GetAsync();
                    grpoResults.Add(result);
                }


                return new Response
                {
                    Status = "success",
                    Payload = grpoResults
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
