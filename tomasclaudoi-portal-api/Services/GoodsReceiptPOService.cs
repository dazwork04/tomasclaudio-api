using Newtonsoft.Json;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Models;
using SLayerConnectionLib;
using B1SLayer;
using SAPB1SLayerWebAPI.Utils;

namespace SAPB1SLayerWebAPI.Services
{
    public class GoodsReceiptPOService
    {
        // GET GOODS RECEIPT POS
        //GetPurchaseOrders/{userId}/{companyDB}/{status}/{cancelled}/{dateFrom}/{dateTo}
        public async Task<Response> GetGoodsReceiptPOsAsync(int userId, string companyDB, char status, char cancelled, string dateFrom, string dateTo, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                string queryFilter = $"DocumentStatus eq '{status}' and Cancelled eq '{cancelled}' and DocDate ge '{dateFrom}' and DocDate le '{dateTo}'" + paginate.Filter;


                var count = await connection.Request(EntitiesKeys.PurchaseDeliveryNotes)
                    .Filter(queryFilter)
                    .GetCountAsync();

                var result = await connection.Request(EntitiesKeys.PurchaseDeliveryNotes)
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

        // CREATE GOODS RECEIPT PO
        /*public async Task<Response> CreateGoodsReceiptPOAsync(int userId, string companyDB, Document goodsReceiptPO) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                var result = await connection.Request(EntitiesKeys.PurchaseDeliveryNotes).PostAsync<dynamic>(goodsReceiptPO);

                Logger.CreateLog(false, "CREATE GOODS RECEIPT PO", "SUCCESS", JsonConvert.SerializeObject(goodsReceiptPO));
                return new Response
                {
                    Status = "success",
                    Message = $"GRPO #{result.DocNum} CREATED successfully!",
                    Payload = result
                };
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CREATE GOODS RECEIPT PO", ex.Message, JsonConvert.SerializeObject(goodsReceiptPO));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });*/
        public async Task<Response> CreateGoodsReceiptPOAsync(int userId, string companyDB, char forApproval, dynamic goodsReceiptPO) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                int? prevCode = null;
                try
                {
                    // GET THE APPROVAL CODE -- BEFORE
                    var approvals = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectType eq '20' and IsDraft eq 'Y' and OriginatorID eq {goodsReceiptPO.UserSign} and ObjectEntry eq null").Top(1).OrderBy("Code desc").GetAsync<List<SLApprovalRequest>>();
                    if (approvals.Count > 0) prevCode = approvals[0].Code;

                    var result = await connection.Request(EntitiesKeys.PurchaseDeliveryNotes).PostAsync<dynamic>(goodsReceiptPO);
                    Logger.CreateLog(false, "CREATE GOODS RECEIPT PO", "SUCCESS", JsonConvert.SerializeObject(goodsReceiptPO));

                    return new Response
                    {
                        Status = "success",
                        Message = $"GRPO #{result.DocNum} CREATED successfully!",
                        Payload = result
                    };

                }
                catch (Exception ex)
                {
                    Logger.CreateLog(false, "CREATE GOODS RECEIPT PO - CATCH", ex.Message, "");
                    if (ex.Message.Contains("no matching records found", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (forApproval == 'Y')
                        {
                            int? newCode = null;
                            var approvals = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectType eq '20' and IsDraft eq 'Y' and OriginatorID eq {goodsReceiptPO.UserSign} and ObjectEntry eq null").Top(1).OrderBy("Code desc").GetAsync<List<SLApprovalRequest>>();
                            if (approvals.Count > 0) newCode = approvals[0].Code;
                            if (prevCode != newCode)
                            {
                                Logger.CreateLog(false, "CREATE GOODS RECEIPT PO APPROVAL", "SUCCESS", JsonConvert.SerializeObject(goodsReceiptPO));
                                return new Response
                                {
                                    Status = "success",
                                    Message = $"GRPO For Approval CREATED successfully!",
                                };
                            }
                            else
                            {
                                Logger.CreateLog(true, "CREATE GOODS RECEIPT PO ERROR", ex.Message, JsonConvert.SerializeObject(goodsReceiptPO));
                                return new Response
                                {
                                    Status = "failed",
                                    Message = ex.Message
                                };
                            }
                        }
                        else
                        {
                            Logger.CreateLog(true, "CREATE GOODS RECEIPT PO", ex.Message, JsonConvert.SerializeObject(goodsReceiptPO));
                            return new Response
                            {
                                Status = "failed",
                                Message = ex.Message
                            };
                        }
                    }
                    Logger.CreateLog(true, "CREATE GOODS RECEIPT PO", ex.Message, JsonConvert.SerializeObject(goodsReceiptPO));
                    return new Response
                    {
                        Status = "failed",
                        Message = ex.Message
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CREATE GOODS RECEIPT PO", ex.Message, JsonConvert.SerializeObject(goodsReceiptPO));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // UPDATE GOODS RECEIPT PO
        public async Task<Response> UpdateGoodsReceiptPOAsync(int userId, string companyDB, dynamic goodsReceiptPO) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                await connection.Request(EntitiesKeys.PurchaseDeliveryNotes, goodsReceiptPO.DocEntry).PatchAsync(goodsReceiptPO);
                var result = await connection.Request(EntitiesKeys.PurchaseDeliveryNotes, goodsReceiptPO.DocEntry).GetAsync();

                var count = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectEntry eq {result.DocEntry} and IsDraft eq 'Y' and ObjectType eq '20'").GetCountAsync();
                Logger.CreateLog(false, "UPDATE GOODS RECEIPT PO - COUNT APPROVAL", "SUCCESS", count.ToString());


                string message = "";
                if (count > 0)
                {

                    message = $"GRPO For Approval UPDATED successfully!";
                    Logger.CreateLog(false, "UPDATE GOODS RECEIPT PO REROUTING APPROVAL", "SUCCESS", JsonConvert.SerializeObject(goodsReceiptPO));
                }
                else
                {
                    Logger.CreateLog(false, "UPDATE GOODS RECEIPT PO", "SUCCESS", JsonConvert.SerializeObject(goodsReceiptPO));
                    message = $"SO #{result.DocNum} UPDATED successfully!";
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

                Logger.CreateLog(true, "UPDATE GOODS RECEIPT PO", ex.Message, JsonConvert.SerializeObject(goodsReceiptPO));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // CANCEL PURCHASE ORDER
        public async Task<Response> CancelGoodsReceiptPOAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.PurchaseDeliveryNotes}({docEntry})/Cancel";

                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.PurchaseDeliveryNotes, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"GRPO #{result.DocNum} CANCELED successfully!",
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
        public async Task<Response> CloseGoodsReceiptPOAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.PurchaseDeliveryNotes}({docEntry})/Close";
                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.PurchaseDeliveryNotes, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"GRPO #{result.DocNum} CLOSED successfully!",
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

        // GET PURCHASE ORDERS
        public async Task<Response> GetPurchaseOrdersAsync(int userId, string companyDB, string cardCode, string docType, string priceMode) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                var purchaseOrders = await connection.Request(EntitiesKeys.PurchaseOrders)
                    .Filter($"CardCode eq '{cardCode}' and DocType eq '{docType}' and PriceMode eq '{priceMode}' and DocumentStatus eq 'O'")
                    .GetAllAsync<dynamic>();


                return new Response
                {
                    Status = "success",
                    Payload = purchaseOrders
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
