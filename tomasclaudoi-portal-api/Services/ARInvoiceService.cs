using B1SLayer;
using Newtonsoft.Json;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Utils;
using SLayerConnectionLib;
using System.Runtime.Intrinsics.Arm;

namespace SARB1SLayerWebARI.Services
{
    public class ARInvoiceService
    {
        // GET AR INVOICES
        public async Task<Response> GetARInvoicesAsync(int userId, string companyDB, char status, string dateFrom, string dateTo, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                //string queryFilter = $"DocumentStatus eq '{status}' and Cancelled eq '{cancelled}' and DocDate ge '{dateFrom}' and DocDate le '{dateTo}'" + paginate.Filter;
                string queryFilter = "";
                switch (status)
                {
                    case 'A': // OPEN
                        queryFilter = "DocumentStatus eq 'bost_Open' and ";
                        break;
                    case 'B': // CLOSED
                        queryFilter = "DocumentStatus eq 'bost_Close' and Cancelled eq 'tNO' ";
                        break;
                    case 'C': // CANCELED
                        queryFilter = "Cancelled eq 'tYES' and ";
                        break;
                    case 'D': // CANCELLATION
                        queryFilter = "CancelStatus eq 'csCancellation' and ";
                        break;
                }

                queryFilter += $"DocDate ge '{dateFrom}' and DocDate le '{dateTo}'" + paginate.Filter;

                var count = await connection.Request(EntitiesKeys.Invoices)
                    .Filter(queryFilter)
                    .GetCountAsync();

                var result = await connection.Request(EntitiesKeys.Invoices)
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

        // CREATE AR INVOICE
        public async Task<Response> CreateARInvoiceAsync(int userId, string companyDB, char forApproval, dynamic arInvoice) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                int? prevCode = null;
                try
                {
                    // GET THE APPROVAL CODE -- BEFORE
                    var approvals = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectType eq '13' and IsDraft eq 'Y' and OriginatorID eq {arInvoice.UserSign} and ObjectEntry eq null").Top(1).OrderBy("Code desc").GetAsync<List<SLApprovalRequest>>();
                    if (approvals.Count > 0) prevCode = approvals[0].Code;

                    var result = await connection.Request(EntitiesKeys.Invoices).PostAsync<dynamic>(arInvoice);

                    var drafts = await connection.Request(EntitiesKeys.Drafts).Filter($"NumAtCard eq '{result.NumAtCard}'").GetAllAsync<dynamic>();

                    foreach (var draft in drafts) await connection.Request(EntitiesKeys.Drafts, draft.DocEntry).DeleteAsync();

                    Logger.CreateLog(false, "CREATE AR INVOICE", "SUCCESS", JsonConvert.SerializeObject(arInvoice));

                    return new Response
                    {
                        Status = "success",
                        Message = $"ARI #{result.DocNum} CREATED successfully!",
                        Payload = result
                    };

                }
                catch (Exception ex)
                {
                    Logger.CreateLog(false, "CREATE AR INVOICE - CATCH", ex.Message, "");
                    if (ex.Message.Contains("no matching records found", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (forApproval == 'Y')
                        {
                            int? newCode = null;
                            var approvals = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectType eq '13' and IsDraft eq 'Y' and OriginatorID eq {arInvoice.UserSign} and ObjectEntry eq null").Top(1).OrderBy("Code desc").GetAsync<List<SLApprovalRequest>>();
                            if (approvals.Count > 0) newCode = approvals[0].Code;
                            if (prevCode != newCode)
                            {
                                Logger.CreateLog(false, "CREATE AR INVOICE APPROVAL", "SUCCESS", JsonConvert.SerializeObject(arInvoice));
                                return new Response
                                {
                                    Status = "success",
                                    Message = $"ARI For Approval CREATED successfully!",
                                };
                            }
                            else
                            {
                                Logger.CreateLog(true, "CREATE AR INVOICE ERROR", ex.Message, JsonConvert.SerializeObject(arInvoice));
                                return new Response
                                {
                                    Status = "failed",
                                    Message = ex.Message
                                };
                            }
                        }
                        else
                        {
                            Logger.CreateLog(true, "CREATE AR INVOICE", ex.Message, JsonConvert.SerializeObject(arInvoice));
                            return new Response
                            {
                                Status = "failed",
                                Message = ex.Message
                            };
                        }
                    }
                    Logger.CreateLog(true, "CREATE AR INVOICE", ex.Message, JsonConvert.SerializeObject(arInvoice));
                    return new Response
                    {
                        Status = "failed",
                        Message = ex.Message
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CREATE AR INVOICE", ex.Message, JsonConvert.SerializeObject(arInvoice));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // UPDATE AR INVOICE
        public async Task<Response> UpdateARInvoiceAsync(int userId, string companyDB, dynamic arInvoice) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                await connection.Request(EntitiesKeys.Invoices, arInvoice.DocEntry).PutAsync(arInvoice);

                var result = await connection.Request(EntitiesKeys.Invoices, arInvoice.DocEntry).GetAsync();
                var count = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"ObjectEntry eq {result.DocEntry} and IsDraft eq 'Y' and ObjectType eq '13'").GetCountAsync();
                Logger.CreateLog(false, "UPDATE AR INVOICE - COUNT APPROVAL", "SUCCESS", count.ToString());


                string message = "";
                if (count > 0)
                {

                    message = $"AR For Approval UPDATED successfully!";
                    Logger.CreateLog(false, "UPDATE AR INVOICE REROUTING APPROVAL", "SUCCESS", JsonConvert.SerializeObject(arInvoice));
                }
                else
                {
                    Logger.CreateLog(false, "UPDATE AR INVOICE", "SUCCESS", JsonConvert.SerializeObject(arInvoice));
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

                Logger.CreateLog(true, "UPDATE AR INVOICE", ex.Message, JsonConvert.SerializeObject(arInvoice));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // CANCEL AR INVOICE
        public async Task<Response> CancelARInvoiceAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.Invoices}({docEntry})/Cancel";

                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.Invoices, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"ARI #{result.DocNum} CANCELED successfully!",
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

        // CLOSE AR INVOICE
        public async Task<Response> CloseARInvoiceAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.Invoices}({docEntry})/Close";
                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.Invoices, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"ARI #{result.DocNum} CLOSED successfully!",
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

        // CANCELLATION AR INVOICE
        public async Task<Response> CancellationARInvoiceAsync(int userId, string companyDB, dynamic arInvoice) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                //await connection.Request(EntitiesKeys.Invoices, arInvoice.DocEntry).PutAsync(arInvoice);
                await connection.Request($"{ActionsKeys.InvoicesService}_Cancel2").PostAsync(new
                {
                    Document = arInvoice
                });

                var results = await connection.Request(EntitiesKeys.Invoices).OrderBy("DocEntry desc").Top(1).GetAsync<List<dynamic>>();
                var result = results.First();
                Logger.CreateLog(false, "CANCELLATION AR INVOICE", "SUCCESS", JsonConvert.SerializeObject(arInvoice));

                return new Response
                {
                    Status = "success",
                    Message = $"AR #{result.DocNum} CREATED CANCELLATION DOCUMENT successfully!",
                    Payload = result
                };
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CANCELLATION AR INVOICE", ex.Message, JsonConvert.SerializeObject(arInvoice));
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
                    .GetAllAsync<Document>();

                List<dynamic> soResults = [];
                foreach (var so in salesOrders)
                {
                    var result = await connection.Request(EntitiesKeys.Orders, so.DocEntry).GetAsync();
                    soResults.Add(result);
                }


                return new Response
                {
                    Status = "success",
                    Payload = soResults
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
                        Document arInvoice = await connection.Request(EntitiesKeys.Invoices, searchInvoice[EntitiesKeys.Invoices]["DocEntry"]).GetAsync<Document>();
                        DownPaymentsToDraw dptd = arInvoice.DownPaymentsToDraw.Find(d => d.DocEntry == r.DocEntry)!;

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
