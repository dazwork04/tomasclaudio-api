using Newtonsoft.Json;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Models;
using SLayerConnectionLib;
using B1SLayer;
using SAPB1SLayerWebAPI.Utils;

namespace SAPB1SLayerWebAPI.Services
{
    public class ReturnService
    {
        // GET DELIVERY RETURNS
        public async Task<Response> GetReturnsAsync(int userId, string companyDB, char status, char cancelled, string dateFrom, string dateTo, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                string queryFilter = $"DocumentStatus eq '{status}' and Cancelled eq '{cancelled}' and DocDate ge '{dateFrom}' and DocDate le '{dateTo}'" + paginate.Filter;


                var count = await connection.Request(EntitiesKeys.Returns)
                    .Filter(queryFilter)
                    .GetCountAsync();

                var result = await connection.Request(EntitiesKeys.Returns)
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

        public async Task<Response> CreateReturnAsync(int userId, string companyDB, char forApproval, dynamic arReturn) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                int? prevCode = null;
                try
                {
                    // GET THE APPROVAL CODE -- BEFORE
                    var approvals = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectType eq '16' and IsDraft eq 'Y' and OriginatorID eq {arReturn.UserSign} and ObjectEntry eq null").Top(1).OrderBy("Code desc").GetAsync<List<SLApprovalRequest>>();
                    if (approvals.Count > 0) prevCode = approvals[0].Code;

                    var result = await connection.Request(EntitiesKeys.Returns).PostAsync<dynamic>(arReturn);
                    Logger.CreateLog(false, "CREATE DELIVERY RETURN", "SUCCESS", JsonConvert.SerializeObject(arReturn));

                    return new Response
                    {
                        Status = "success",
                        Message = $"ART #{result.DocNum} CREATED successfully!",
                        Payload = result
                    };

                }
                catch (Exception ex)
                {
                    Logger.CreateLog(false, "CREATE DELIVERY RETURN - CATCH", ex.Message, "");
                    if (ex.Message.Contains("no matching records found", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (forApproval == 'Y')
                        {
                            int? newCode = null;
                            var approvals = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectType eq '16' and IsDraft eq 'Y' and OriginatorID eq {arReturn.UserSign} and ObjectEntry eq null").Top(1).OrderBy("Code desc").GetAsync<List<SLApprovalRequest>>();
                            if (approvals.Count > 0) newCode = approvals[0].Code;
                            if (prevCode != newCode)
                            {
                                Logger.CreateLog(false, "CREATE DELIVERY RETURN APPROVAL", "SUCCESS", JsonConvert.SerializeObject(arReturn));
                                return new Response
                                {
                                    Status = "success",
                                    Message = $"ART For Approval CREATED successfully!",
                                };
                            }
                            else
                            {
                                Logger.CreateLog(true, "CREATE DELIVERY RETURN ERROR", ex.Message, JsonConvert.SerializeObject(arReturn));
                                return new Response
                                {
                                    Status = "failed",
                                    Message = ex.Message
                                };
                            }
                        }
                        else
                        {
                            Logger.CreateLog(true, "CREATE DELIVERY RETURN", ex.Message, JsonConvert.SerializeObject(arReturn));
                            return new Response
                            {
                                Status = "failed",
                                Message = ex.Message
                            };
                        }
                    }
                    Logger.CreateLog(true, "CREATE DELIVERY RETURN", ex.Message, JsonConvert.SerializeObject(arReturn));
                    return new Response
                    {
                        Status = "failed",
                        Message = ex.Message
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CREATE DELIVERY RETURN", ex.Message, JsonConvert.SerializeObject(arReturn));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // UPDATE DELIVERY RETURN
        public async Task<Response> UpdateReturnAsync(int userId, string companyDB, dynamic arReturn) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                await connection.Request(EntitiesKeys.Returns, arReturn.DocEntry).PatchAsync(arReturn);
                var result = await connection.Request(EntitiesKeys.Returns, arReturn.DocEntry).GetAsync();

                var count = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectEntry eq {result.DocEntry} and IsDraft eq 'Y' and ObjectType eq '16'").GetCountAsync();
                Logger.CreateLog(false, "UPDATE DELIVERY RETURN - COUNT APPROVAL", "SUCCESS", count.ToString());


                string message = "";
                if (count > 0)
                {

                    message = $"ART For Approval UPDATED successfully!";
                    Logger.CreateLog(false, "UPDATE DELIVERY RETURN REROUTING APPROVAL", "SUCCESS", JsonConvert.SerializeObject(arReturn));
                }
                else
                {
                    Logger.CreateLog(false, "UPDATE DELIVERY RETURN", "SUCCESS", JsonConvert.SerializeObject(arReturn));
                    message = $"ART #{result.DocNum} UPDATED successfully!";
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

                Logger.CreateLog(true, "UPDATE DELIVERY RETURN", ex.Message, JsonConvert.SerializeObject(arReturn));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // CANCEL DELIVERY RETURN
        public async Task<Response> CancelReturnAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.Returns}({docEntry})/Cancel";

                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.Returns, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"ART #{result.DocNum} CANCELED successfully!",
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

        // CLOSE DELIVERY RETURN
        public async Task<Response> CloseReturnAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.Returns}({docEntry})/Close";
                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.Returns, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"ART #{result.DocNum} CLOSED successfully!",
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

        // GET DELIVERIES
        public async Task<Response> GetDeliveriesAsync(int userId, string companyDB, string cardCode, string docType, string priceMode) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                var deliveries = await connection.Request(EntitiesKeys.DeliveryNotes)
                    .Filter($"CardCode eq '{cardCode}' and DocType eq '{docType}' and PriceMode eq '{priceMode}' and DocumentStatus eq 'O'")
                    .GetAllAsync<dynamic>();


                return new Response
                {
                    Status = "success",
                    Payload = deliveries
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
