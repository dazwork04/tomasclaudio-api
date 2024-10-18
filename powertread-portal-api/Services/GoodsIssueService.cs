using B1SLayer;
using Newtonsoft.Json;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Utils;
using SLayerConnectionLib;

namespace SAPB1SLayerWebAPI.Services
{
    public class GoodsIssueService
    {
        // GET GOODS ISSUES
        public async Task<Response> GetGoodsIssuesAsync(int userId, string companyDB, string dateFrom, string dateTo, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                string queryFilter = $"DocDate ge '{dateFrom}' and DocDate le '{dateTo}'" + paginate.Filter;

                var count = await connection.Request(EntitiesKeys.InventoryGenExits)
                    .Filter(queryFilter)
                    .GetCountAsync();

                var result = await connection.Request(EntitiesKeys.InventoryGenExits)
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

        // CREATE GOODS ISSUE
        public async Task<Response> CreateGoodsIssueAsync(int userId, string companyDB, char forApproval, dynamic goodsIssue) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                int? prevCode = null;
                try
                {
                    // GET THE APPROVAL CODE -- BEFORE
                    var approvals = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectType eq '60' and IsDraft eq 'Y' and OriginatorID eq {goodsIssue.UserSign} and ObjectEntry eq null").Top(1).OrderBy("Code desc").GetAsync<List<SLApprovalRequest>>();
                    if (approvals.Count > 0) prevCode = approvals[0].Code;

                    var result = await connection.Request(EntitiesKeys.InventoryGenExits).PostAsync<dynamic>(goodsIssue);
                    Logger.CreateLog(false, "CREATE GOODS ISSUE", "SUCCESS", JsonConvert.SerializeObject(goodsIssue));

                    return new Response
                    {
                        Status = "success",
                        Message = $"GI #{result.DocNum} CREATED successfully!",
                        Payload = result
                    };

                }
                catch (Exception ex)
                {
                    Logger.CreateLog(false, "CREATE GOODS ISSUE - CATCH", ex.Message, "");
                    if (ex.Message.Contains("no matching records found", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (forApproval == 'Y')
                        {
                            int? newCode = null;
                            var approvals = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectType eq '60' and IsDraft eq 'Y' and OriginatorID eq {goodsIssue.UserSign} and ObjectEntry eq null").Top(1).OrderBy("Code desc").GetAsync<List<SLApprovalRequest>>();
                            if (approvals.Count > 0) newCode = approvals[0].Code;
                            if (prevCode != newCode)
                            {
                                Logger.CreateLog(false, "CREATE GOODS ISSUE", "SUCCESS", JsonConvert.SerializeObject(goodsIssue));
                                return new Response
                                {
                                    Status = "success",
                                    Message = $"GI For Approval CREATED successfully!",
                                };
                            }
                            else
                            {
                                Logger.CreateLog(true, "CREATE GOODS ISSUE ERROR", ex.Message, JsonConvert.SerializeObject(goodsIssue));
                                return new Response
                                {
                                    Status = "failed",
                                    Message = ex.Message
                                };
                            }
                        }
                        else
                        {
                            Logger.CreateLog(true, "CREATE GOODS ISSUE", ex.Message, JsonConvert.SerializeObject(goodsIssue));
                            return new Response
                            {
                                Status = "failed",
                                Message = ex.Message
                            };
                        }
                    }
                    Logger.CreateLog(true, "CREATE GOODS ISSUE", ex.Message, JsonConvert.SerializeObject(goodsIssue));
                    return new Response
                    {
                        Status = "failed",
                        Message = ex.Message
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CREATE GOODS ISSUE", ex.Message, JsonConvert.SerializeObject(goodsIssue));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // UPDATE GOODS ISSUE
        public async Task<Response> UpdateGoodsIssueAsync(int userId, string companyDB, dynamic goodsIssue) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                await connection.Request(EntitiesKeys.InventoryGenExits, goodsIssue.DocEntry).PatchAsync(goodsIssue);
                var result = await connection.Request(EntitiesKeys.InventoryGenExits, goodsIssue.DocEntry).GetAsync();
                var count = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectEntry eq {result.DocEntry} and IsDraft eq 'Y' and ObjectType eq '60'").GetCountAsync();
                Logger.CreateLog(false, "UPDATE GOODS ISSUE - COUNT APPROVAL", "SUCCESS", count.ToString());


                string message = "";
                if (count > 0)
                {
                    message = $"GI For Approval UPDATED successfully!";
                    Logger.CreateLog(false, "UPDATE GOODS ISSUE REROUTING APPROVAL", "SUCCESS", JsonConvert.SerializeObject(goodsIssue));
                }
                else
                {
                    Logger.CreateLog(false, "UPDATE GOODS ISSUE", "SUCCESS", JsonConvert.SerializeObject(goodsIssue));
                    message = $"GI #{result.DocNum} UPDATED successfully!";
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

                Logger.CreateLog(true, "UPDATE GOODS ISSUE", ex.Message, JsonConvert.SerializeObject(goodsIssue));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });
    }
}
