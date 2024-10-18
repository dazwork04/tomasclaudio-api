using B1SLayer;
using Newtonsoft.Json;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Utils;
using SLayerConnectionLib;
using System.Runtime.Intrinsics.Arm;

namespace SARB1SLayerWebARI.Services
{
    public class ARCreditMemoService
    {
        // GET AR CREDIT MEMOS
        public async Task<Response> GetARCreditMemosAsync(int userId, string companyDB, char status, char cancelled, string dateFrom, string dateTo, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                string queryFilter = $"DocumentStatus eq '{status}' and Cancelled eq '{cancelled}' and DocDate ge '{dateFrom}' and DocDate le '{dateTo}'" + paginate.Filter;


                var count = await connection.Request(EntitiesKeys.CreditNotes)
                    .Filter(queryFilter)
                    .GetCountAsync();

                var result = await connection.Request(EntitiesKeys.CreditNotes)
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

        // CREATE AR CREDIT MEMO
        public async Task<Response> CreateARCreditMemoAsync(int userId, string companyDB, char forApproval, dynamic arCreditMemo) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                int? prevCode = null;
                try
                {
                    // GET THE APPROVAL CODE -- BEFORE
                    var approvals = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectType eq '14' and IsDraft eq 'Y' and OriginatorID eq {arCreditMemo.UserSign} and ObjectEntry eq null").Top(1).OrderBy("Code desc").GetAsync<List<SLApprovalRequest>>();
                    if (approvals.Count > 0) prevCode = approvals[0].Code;

                    var result = await connection.Request(EntitiesKeys.CreditNotes).PostAsync<dynamic>(arCreditMemo);
                    Logger.CreateLog(false, "CREATE AR CREDIT MEMO", "SUCCESS", JsonConvert.SerializeObject(arCreditMemo));

                    return new Response
                    {
                        Status = "success",
                        Message = $"ARCM #{result.DocNum} CREATED successfully!",
                        Payload = result
                    };

                }
                catch (Exception ex)
                {
                    Logger.CreateLog(false, "CREATE AR CREDIT MEMO - CATCH", ex.Message, "");
                    if (ex.Message.Contains("no matching records found", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (forApproval == 'Y')
                        {
                            int? newCode = null;
                            var approvals = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectType eq '14' and IsDraft eq 'Y' and OriginatorID eq {arCreditMemo.UserSign} and ObjectEntry eq null").Top(1).OrderBy("Code desc").GetAsync<List<SLApprovalRequest>>();
                            if (approvals.Count > 0) newCode = approvals[0].Code;
                            if (prevCode != newCode)
                            {
                                Logger.CreateLog(false, "CREATE AR CREDIT MEMO APPROVAL", "SUCCESS", JsonConvert.SerializeObject(arCreditMemo));
                                return new Response
                                {
                                    Status = "success",
                                    Message = $"ARI For Approval CREATED successfully!",
                                };
                            }
                            else
                            {
                                Logger.CreateLog(true, "CREATE AR CREDIT MEMO ERROR", ex.Message, JsonConvert.SerializeObject(arCreditMemo));
                                return new Response
                                {
                                    Status = "failed",
                                    Message = ex.Message
                                };
                            }
                        }
                        else
                        {
                            Logger.CreateLog(true, "CREATE AR CREDIT MEMO", ex.Message, JsonConvert.SerializeObject(arCreditMemo));
                            return new Response
                            {
                                Status = "failed",
                                Message = ex.Message
                            };
                        }
                    }
                    Logger.CreateLog(true, "CREATE AR CREDIT MEMO", ex.Message, JsonConvert.SerializeObject(arCreditMemo));
                    return new Response
                    {
                        Status = "failed",
                        Message = ex.Message
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CREATE AR CREDIT MEMO", ex.Message, JsonConvert.SerializeObject(arCreditMemo));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // UPDATE AR CREDIT MEMO
        public async Task<Response> UpdateARCreditMemoAsync(int userId, string companyDB, dynamic arCreditMemo) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                await connection.Request(EntitiesKeys.CreditNotes, arCreditMemo.DocEntry).PutAsync(arCreditMemo);

                var result = await connection.Request(EntitiesKeys.CreditNotes, arCreditMemo.DocEntry).GetAsync();
                var count = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectEntry eq {result.DocEntry} and IsDraft eq 'Y' and ObjectType eq '14'").GetCountAsync();
                Logger.CreateLog(false, "UPDATE AR CREDIT MEMO - COUNT APPROVAL", "SUCCESS", count.ToString());


                string message = "";
                if (count > 0)
                {

                    message = $"AR For Approval UPDATED successfully!";
                    Logger.CreateLog(false, "UPDATE AR CREDIT MEMO REROUTING APPROVAL", "SUCCESS", JsonConvert.SerializeObject(arCreditMemo));
                }
                else
                {
                    Logger.CreateLog(false, "UPDATE AR CREDIT MEMO", "SUCCESS", JsonConvert.SerializeObject(arCreditMemo));
                    message = $"AR #{result.DocNum} UPDATED successfully!";
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

                Logger.CreateLog(true, "UPDATE AR CREDIT MEMO", ex.Message, JsonConvert.SerializeObject(arCreditMemo));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // CANCEL AR CREDIT MEMO
        public async Task<Response> CancelARCreditMemoAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.CreditNotes}({docEntry})/Cancel";

                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.CreditNotes, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"ARCM #{result.DocNum} CANCELED successfully!",
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

        // CLOSE AR CREDIT MEMO
        public async Task<Response> CloseARCreditMemoAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.CreditNotes}({docEntry})/Close";
                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.CreditNotes, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"ARCM #{result.DocNum} CLOSED successfully!",
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
                var purchaseOrders = await connection.Request(EntitiesKeys.Orders)
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

        // GET DELIVERIES
        public async Task<Response> GetDeliveriesAsync(int userId, string companyDB, string cardCode, string docType, string priceMode) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                var goodsReceiptPOs = await connection.Request(EntitiesKeys.DeliveryNotes)
                    .Filter($"CardCode eq '{cardCode}' and DocType eq '{docType}' and PriceMode eq '{priceMode}' and DocumentStatus eq 'O'")
                    .GetAllAsync<dynamic>();


                return new Response
                {
                    Status = "success",
                    Payload = goodsReceiptPOs
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

        // GET AR INVOICES
        public async Task<Response> GetARInvoicesAsync(int userId, string companyDB, string cardCode, string docType, string priceMode) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                var arInvoices = await connection.Request(EntitiesKeys.Invoices)
                    .Filter($"CardCode eq '{cardCode}' and DocType eq '{docType}' and PriceMode eq '{priceMode}' and DocumentStatus eq 'O'")
                    .GetAllAsync<Document>();

                List<dynamic> ariResults = [];
                foreach (var ari in arInvoices)
                {
                    var result = await connection.Request(EntitiesKeys.Invoices, ari.DocEntry).GetAsync();
                    ariResults.Add(result);
                }


                return new Response
                {
                    Status = "success",
                    Payload = ariResults
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

        // GET DOWNPAYMENTS
        public async Task<Response> GetDownPaymentsAsync(int userId, string companyDB, string cardCode) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                var results = await connection.Request(EntitiesKeys.DownPayments)
                    .Filter($"CardCode eq '{cardCode}' and DownPaymentStatus eq 'O'")
                    .GetAllAsync<Document>();

                List<DownPayment> downPayments = [];

                foreach (var r in results)
                {
                    //double netDocTotal = r.DocumentLines.Sum(dl => dl.LineTotal);

                    var searchInvoices = await connection
                        .Request($"$crossjoin({EntitiesKeys.Invoices},{EntitiesKeys.Invoices}/DownPaymentsToDraw)")
                        .Expand($"{EntitiesKeys.Invoices}($select=DocEntry),{EntitiesKeys.Invoices}/DownPaymentsToDraw($select=DocEntry)")
                        .Filter($"{EntitiesKeys.Invoices}/DownPaymentsToDraw/DocEntry eq {r.DocEntry} and {EntitiesKeys.Invoices}/DownPaymentsToDraw/DownPaymentType eq 'dptInvoice' and {EntitiesKeys.Invoices}/DownPaymentsToDraw/DocInternalID eq {EntitiesKeys.Invoices}/DocEntry")
                        .GetAllAsync<dynamic>();

                    var taxCodes = await connection.Request(EntitiesKeys.VatGroups).GetAllAsync<VatGroup>();

                    var lines = r.DocumentLines.GroupBy(dl => dl.VatGroup).Select(dl => new DownPayment
                    {
                        DocEntry = r.DocEntry,
                        DocNum = 0,
                        //Remarks = "",
                        TaxCode = dl.Key,
                        NetAmountToDraw = dl.Sum(d => d.LineTotal) - (dl.Sum(d => d.LineTotal) * (r.DiscountPercent / 100)),
                        TaxAmountToDraw = dl.Sum(d => d.TaxTotal) ,
                        GrossAmountToDraw = dl.Sum(d => d.GrossTotal) - (dl.Sum(d => d.GrossTotal) * (r.DiscountPercent / 100)),
                        OpenNetAmount = dl.Sum(d => d.LineTotal) - (dl.Sum(d => d.LineTotal) * (r.DiscountPercent / 100)),
                        OpenTaxAmount = dl.Sum(d => d.TaxTotal),
                        OpenGrossAmount = dl.Sum(d => d.GrossTotal) - (dl.Sum(d => d.GrossTotal) * (r.DiscountPercent / 100)),
                        DocumentDate = "",
                    }).ToList();

                    foreach (var searchInvoice in searchInvoices)
                    {
                        Document arCreditMemo = await connection.Request(EntitiesKeys.Invoices, searchInvoice[EntitiesKeys.Invoices]["DocEntry"]).GetAsync<Document>();
                        DownPaymentsToDraw dptd = arCreditMemo.DownPaymentsToDraw.Find(d => d.DocEntry == r.DocEntry)!;

                        for (int i = 0; i < lines.Count; i++)
                        {
                            DownPaymentsToDrawDetail dptdd = dptd.DownPaymentsToDrawDetails.Find(a => a.VatGroupCode == lines[i].TaxCode)!;

                            var lineNetAmount = dptdd.AmountToDraw;
                            var lineTaxAmount = dptdd.Tax;
                            var lineGrossAmount = dptdd.GrossAmountToDraw;

                            lines[i].NetAmountToDraw -= lineNetAmount;
                            lines[i].TaxAmountToDraw -= lineTaxAmount;
                            lines[i].GrossAmountToDraw -= lineGrossAmount;
                            lines[i].OpenNetAmount -= lineNetAmount;
                            lines[i].OpenTaxAmount -= lineTaxAmount;
                            lines[i].OpenGrossAmount -= lineGrossAmount;
                        }
                    }

                    var netAmount = lines.Sum(l => l.NetAmountToDraw);
                    var taxAmount = lines.Sum(l => l.TaxAmountToDraw);
                    var grossAmount = lines.Sum(l => l.GrossAmountToDraw);

                    downPayments.Add(new DownPayment
                    {
                        DocEntry = r.DocEntry,
                        DocNum = r.DocNum,
                        //Remarks = r.Comments,
                        NetAmountToDraw = netAmount,
                        TaxAmountToDraw = taxAmount,
                        GrossAmountToDraw = grossAmount,
                        OpenNetAmount = netAmount,
                        OpenTaxAmount = taxAmount,
                        OpenGrossAmount = grossAmount,
                        DocumentDate = r.DocDate,
                        Lines = lines,
                    });

                }
              
                return new Response
                {
                    Status = "success",
                    Payload = downPayments
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
