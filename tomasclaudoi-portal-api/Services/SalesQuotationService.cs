using Newtonsoft.Json;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Models;
using SLayerConnectionLib;
using B1SLayer;
using SAPB1SLayerWebAPI.Utils;

namespace SAPB1SLayerWebAPI.Services
{
    public class SalesQuotationService
    {
        // GET SALES QUOTATION
        public async Task<Response> GetSalesQuotationsAsync(int userId, string companyDB, char status, char cancelled, string dateFrom, string dateTo, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                string queryFilter = $"DocumentStatus eq '{status}' and Cancelled eq '{cancelled}' and DocDate ge '{dateFrom}' and DocDate le '{dateTo}'" + paginate.Filter;


                var count = await connection.Request(EntitiesKeys.Quotations)
                    .Filter(queryFilter)
                    .GetCountAsync();

                var result = await connection.Request(EntitiesKeys.Quotations)
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

        // CREATE SALES QUOTATION
        public async Task<Response> CreateSalesQuotationAsync(int userId, string companyDB, char forApproval, dynamic salesQuotation) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                int? prevCode = null;
                try
                {
                    // GET THE APPROVAL CODE -- BEFORE
                    var approvals = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectType eq '23' and IsDraft eq 'Y' and OriginatorID eq {salesQuotation.UserSign} and ObjectEntry eq null").Top(1).OrderBy("Code desc").GetAsync<List<SLApprovalRequest>>();
                    if (approvals.Count > 0) prevCode = approvals[0].Code;

                    var result = await connection.Request(EntitiesKeys.Quotations).PostAsync<dynamic>(salesQuotation);
                    Logger.CreateLog(false, "CREATE SALES QUOTATION", "SUCCESS", JsonConvert.SerializeObject(salesQuotation));

                    return new Response
                    {
                        Status = "success",
                        Message = $"SQ #{result.DocNum} CREATED successfully!",
                        Payload = result
                    };

                }
                catch (Exception ex)
                {
                    Logger.CreateLog(false, "CREATE SALES QUOTATION - CATCH", ex.Message, "");
                    if (ex.Message.Contains("no matching records found", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (forApproval == 'Y')
                        {
                            int? newCode = null;
                            var approvals = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectType eq '23' and IsDraft eq 'Y' and OriginatorID eq {salesQuotation.UserSign} and ObjectEntry eq null").Top(1).OrderBy("Code desc").GetAsync<List<SLApprovalRequest>>();
                            if (approvals.Count > 0) newCode = approvals[0].Code;
                            if (prevCode != newCode)
                            {
                                Logger.CreateLog(false, "CREATE SALES QUOTATION APPROVAL", "SUCCESS", JsonConvert.SerializeObject(salesQuotation));
                                return new Response
                                {
                                    Status = "success",
                                    Message = $"SQ For Approval CREATED successfully!",
                                };
                            }
                            else
                            {
                                Logger.CreateLog(true, "CREATE SALES QUOTATION ERROR", ex.Message, JsonConvert.SerializeObject(salesQuotation));
                                return new Response
                                {
                                    Status = "failed",
                                    Message = ex.Message
                                };
                            }
                        }
                        else
                        {
                            Logger.CreateLog(true, "CREATE SALES QUOTATION", ex.Message, JsonConvert.SerializeObject(salesQuotation));
                            return new Response
                            {
                                Status = "failed",
                                Message = ex.Message
                            };
                        }
                    }
                    Logger.CreateLog(true, "CREATE SALES QUOTATION", ex.Message, JsonConvert.SerializeObject(salesQuotation));
                    return new Response
                    {
                        Status = "failed",
                        Message = ex.Message
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CREATE SALES QUOTATION", ex.Message, JsonConvert.SerializeObject(salesQuotation));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // UPDATE SALES QUOTATION
        public async Task<Response> UpdateSalesQuotationAsync(int userId, string companyDB, dynamic salesQuotation) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                await connection.Request(EntitiesKeys.Quotations, salesQuotation.DocEntry).PatchAsync(salesQuotation);
                var result = await connection.Request(EntitiesKeys.Quotations, salesQuotation.DocEntry).GetAsync();

                var count = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectEntry eq {result.DocEntry} and IsDraft eq 'Y' and ObjectType eq '23'").GetCountAsync();
                Logger.CreateLog(false, "UPDATE SALES QUOTATION - COUNT APPROVAL", "SUCCESS", count.ToString());


                string message = "";
                if (count > 0)
                {

                    message = $"SQ For Approval UPDATED successfully!";
                    Logger.CreateLog(false, "UPDATE SALES QUOTATION REROUTING APPROVAL", "SUCCESS", JsonConvert.SerializeObject(salesQuotation));
                }
                else
                {
                    Logger.CreateLog(false, "UPDATE SALES QUOTATION", "SUCCESS", JsonConvert.SerializeObject(salesQuotation));
                    message = $"SQ #{result.DocNum} UPDATED successfully!";
                }

                return new Response
                {
                    Status = "success",
                    Message = message,
                    Payload = result
                };

                //Logger.CreateLog(false, "UPDATE SALES QUOTATION", "SUCCESS", JsonConvert.SerializeObject(salesQuotation));
                //return new Response
                //{
                //    Status = "success",
                //    Message = $"SQ #{result.DocNum} UPDATED successfully!",
                //    Payload = result
                //};
            }
            catch (Exception ex)
            {

                Logger.CreateLog(true, "UPDATE SALES QUOTATION", ex.Message, JsonConvert.SerializeObject(salesQuotation));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // CANCEL SALES QUOTATION
        public async Task<Response> CancelSalesQuotationAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.Quotations}({docEntry})/Cancel";

                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.Quotations, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"SQ #{result.DocNum} CANCELED successfully!",
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

        // CLOSE SALES QUOTATION
        public async Task<Response> CloseSalesQuotationAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.Quotations}({docEntry})/Close";
                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.Quotations, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"SQ #{result.DocNum} CLOSED successfully!",
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
