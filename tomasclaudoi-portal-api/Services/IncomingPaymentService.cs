using B1SLayer;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SAPB1SLayerWebAPI.Context;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Utils;
using SLayerConnectionLib;

namespace SAPB1SLayerWebAPI.Services
{
    public class IncomingPaymentService
    {
        private readonly MainDbContext mainDbContext;
        private readonly string connString;
        public IncomingPaymentService(MainDbContext mainDbContext)
        {
            this.mainDbContext = mainDbContext;
            connString = mainDbContext.Database.GetDbConnection().ConnectionString;
        }

        // GET INCOMING PAYMENTS
        public async Task<Response> GetIncomingPaymentsAsync(int userId, string companyDB, string cancelled, string dateFrom, string dateTo, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                string queryFilter = $"Cancelled eq '{cancelled}' and DocDate ge '{dateFrom}' and DocDate le '{dateTo}'" + paginate.Filter;


                var count = await connection.Request(EntitiesKeys.IncomingPayments)
                    .Filter(queryFilter)
                    .GetCountAsync();

                var result = await connection.Request(EntitiesKeys.IncomingPayments)
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

        // GET INCOMING PAYMENT OTHER DATA
        public async Task<Response> GetIncomingPaymentOtherDataAsync(string database, int docEntry, char posted, MainDbContext mainDbContext) => await Task.Run(() =>
        {
            try
            {
                mainDbContext.Database.GetDbConnection().ConnectionString = string.Format(connString, database);

                if (posted == 'N')
                {
                    var result = (from a in mainDbContext.OPDF
                                  where a.DocEntry == docEntry
                                  select new PaymentOtherData
                                  {
                                      OpenBalance = a.OpenBal,
                                      DocEntry = a.DocEntry,
                                      DocNum = a.DocNum,
                                      TotalAmountDue = a.DocTotal,
                                      TransId = a.TransId ?? 0,
                                      PaymentOnAccount = a.NoDocSum
                                  }).FirstOrDefault();

                    return new Response
                    {
                        Status = "success",
                        Payload = result,
                    };
                }
                else
                {
                    var result = (from a in mainDbContext.ORCT
                                  where a.DocEntry == docEntry
                                  select new PaymentOtherData
                                  {
                                      OpenBalance = a.OpenBal,
                                      DocEntry = a.DocEntry,
                                      DocNum = a.DocNum,
                                      TotalAmountDue = a.DocTotal,
                                      TransId = a.TransId,
                                      PaymentOnAccount = a.NoDocSum
                                  }).FirstOrDefault();

                    return new Response
                    {
                        Status = "success",
                        Payload = result,
                    };
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message,
                };
            }
        });

        // GET INCOMING PAYMENT INVOICES
        public async Task<Response> GetIncomingPaymentLinesAsync(int userId, string companyDB, int docEntry, char posted) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                PaymentObject incomingPayment = await connection.Request(posted == 'N' ? EntitiesKeys.PaymentDrafts : EntitiesKeys.IncomingPayments, docEntry).GetAsync<PaymentObject>();

                List<PaymentInvoiceData> paymentInvoices = [];
                foreach (var paymentInvoice in incomingPayment.PaymentInvoices)
                {
                    string reqParam = ObjectTypesHelper.GetPaymentInvoiceType(paymentInvoice.InvoiceType);
                    if (reqParam != string.Empty)
                    {
                        var result = await connection.Request(reqParam, paymentInvoice.DocEntry).GetAsync<dynamic>();

                        if (paymentInvoice.InvoiceType == "it_JournalEntry" || paymentInvoice.InvoiceType == "it_Receipt")
                        {
                            double sum = 0;
                            foreach (var item in result["JournalEntryLines"])
                            {
                                string cardCode = item["ShortName"];
                                int lineID = item["Line_ID"];

                                if (cardCode == incomingPayment.CardCode && lineID == paymentInvoice.DocLine)
                                {
                                    double debit = item["Debit"];
                                    double credit = item["Credit"];
                                    sum += (debit - credit);
                                }
                            }

                            string docDate = result["ReferenceDate"];
                            int docNum = result["Number"];
                            string dueDate = result["DueDate"];

                            paymentInvoices.Add(new PaymentInvoiceData
                            {
                                Checked = true,
                                LineNum = paymentInvoice.LineNum,
                                DocEntry = paymentInvoice.DocEntry,
                                BalanceDue = sum - paymentInvoice.SumApplied,
                                DocDate = docDate,
                                DocNum = docNum,
                                NumAtCard = docNum.ToString(),
                                DocType = "it_JournalEntry",
                                OverdueDays = (int)(DateTime.Parse(incomingPayment.DocDate) - DateTime.Parse(dueDate)).TotalDays,
                                Total = sum,
                                TotalPayment = paymentInvoice.SumApplied
                            });
                        }
                        else
                        {
                            bool isNegate = paymentInvoice.InvoiceType.Contains("purchase", StringComparison.CurrentCultureIgnoreCase);

                            double docTotal = result["DocTotal"] * (isNegate ? -1 : 1);
                            string docDate = result["DocDate"];
                            int docNum = result["DocNum"];
                            string dueDate = result["DocDueDate"];
                            string numAtCard = result["NumAtCard"];

                            double sumApplied = paymentInvoice.SumApplied * (isNegate ? -1 : 1);

                            paymentInvoices.Add(new PaymentInvoiceData
                            {
                                Checked = true,
                                LineNum = paymentInvoice.LineNum,
                                DocEntry = paymentInvoice.DocEntry,
                                BalanceDue = docTotal - (posted == 'N' ? 0 : sumApplied),
                                DocDate = docDate,
                                DocNum = docNum,
                                NumAtCard = numAtCard,
                                DocType = paymentInvoice.InvoiceType,
                                OverdueDays = (int)(DateTime.Parse(incomingPayment.DocDate) - DateTime.Parse(dueDate)).TotalDays,
                                Total = docTotal,
                                TotalPayment = sumApplied
                            });
                        }

                    }
                }

                return new Response
                {
                    Status = "success",
                    Payload = paymentInvoices
                };
            }
            catch (Exception ex)
            {

                Logger.CreateLog(true, "GET INCOMING PAYMENT LINES", ex.Message, JsonConvert.SerializeObject(new { DocEntry = docEntry }));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // GET BP INVOICES
        public async Task<Response> GetBPInvoicesAsync(int userId, string companyDB, string cardCode, MainDbContext mainDbContext) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                mainDbContext.Database.GetDbConnection().ConnectionString = string.Format(connString, companyDB);

                List<PaymentInvoiceData> paymentInvoices = [];
                // AR Invoice
                var ariResult = await connection.Request(EntitiesKeys.Invoices).Filter($"(CardCode eq '{cardCode}' or FatherCard eq '{cardCode}') and DocumentStatus eq 'bost_Open'").OrderBy("DocEntry asc").GetAllAsync<Document>();

                foreach (var ari in ariResult)
                {
                    double balanceDue = ari.DocTotal - ari.PaidToDate;
                    var data = new PaymentInvoiceData
                    {
                        Checked = false,
                        DocEntry = ari.DocEntry,
                        DocNum = ari.DocNum,
                        NumAtCard = ari.NumAtCard,
                        DocDate = ari.DocDate,
                        DocType = "it_Invoice",
                        LineNum = 0,
                        OverdueDays = (int)(DateTime.Now - DateTime.Parse(ari.DocDueDate)).TotalDays,
                        BalanceDue = balanceDue,
                        Total = ari.DocTotal,
                        TotalPayment = balanceDue,
                    };
                    paymentInvoices.Add(data);
                }

                // JE 
                var jeResult = (from a in mainDbContext.JDT1
                                join b in mainDbContext.OACT on a.Account equals b.AcctCode into OACT
                                from b1 in OACT.DefaultIfEmpty()
                                where a.Account != a.ShortName && (a.BalDueCred != 0 || a.BalDueDeb != 0)
                                && a.TransType != 13 && a.ShortName == cardCode
                                select new PaymentInvoiceData
                                {
                                    Checked = false,
                                    DocEntry = a.TransId,
                                    DocNum = a.TransId,
                                    NumAtCard = a.TransId.ToString(),
                                    DocDate = a.RefDate.ToString("MM/dd/yyyy"),
                                    DocType = "it_JournalEntry",
                                    LineNum = a.Line_ID,
                                    OverdueDays = (int)(DateTime.Now - a.DueDate).TotalDays,
                                    BalanceDue = b1.LocManTran == 'Y' ? (a.DebCred == 'D' ? a.BalDueDeb : a.BalDueCred * -1) : (a.DebCred == 'C' ? a.BalDueCred : a.BalDueDeb),
                                    Total = b1.LocManTran == 'Y' ? (a.DebCred == 'D' ? a.Debit : a.Credit * -1) : (a.DebCred == 'C' ? a.Credit : a.Debit),
                                    TotalPayment = b1.LocManTran == 'Y' ? (a.DebCred == 'D' ? a.BalDueDeb : a.BalDueCred * -1) : (a.DebCred == 'C' ? a.BalDueCred : a.BalDueDeb),
                                }).ToList();

                paymentInvoices.AddRange(jeResult);

                return new Response
                {
                    Status = "success",
                    Payload = paymentInvoices.OrderByDescending(p => p.OverdueDays),
                };
            }
            catch (Exception ex)
            {

                Logger.CreateLog(true, "GET INCOMING PAYMENT LINES", ex.Message, JsonConvert.SerializeObject(new { CardCode = cardCode }));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // CREATE INCOMING PAYMENT
        public async Task<Response> CreateIncomingPaymentAsync(int userId, string companyDB, char forPosted, PostPaymentObject incomingPayment) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                if (forPosted == 'Y')
                {
                    var result = await connection.Request(EntitiesKeys.IncomingPayments).PostAsync<dynamic>(incomingPayment);

                    Logger.CreateLog(false, "CREATE POSTED INCOMING PAYMENT", "SUCCESS", JsonConvert.SerializeObject(incomingPayment));
                    return new Response
                    {
                        Status = "success",
                        Message = $"IP #{result.DocNum} CREATED successfully!",
                        Payload = result
                    };

                }
                else
                {

                    await connection.Request(EntitiesKeys.PaymentDrafts).PostAsync<dynamic>(incomingPayment);
                    var results = await connection.Request(EntitiesKeys.PaymentDrafts).Filter("DocObjectCode eq 'bopot_IncomingPayments'").OrderBy("DocEntry desc").Top(1).GetAsync<List<dynamic>>();
                    var result = results.First();

                    Logger.CreateLog(false, "CREATE DRAFT INCOMING PAYMENT", "SUCCESS", JsonConvert.SerializeObject(incomingPayment));
                    return new Response
                    {
                        Status = "success",
                        Message = $"DRAFT IP #{result.DocNum} CREATED successfully!",
                        Payload = result
                    };
                }

            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CREATE INCOMING PAYMENT", ex.Message, JsonConvert.SerializeObject(incomingPayment));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // UPDATE INCOMING PAYMENT
        public async Task<Response> UpdateIncomingPaymentAsync(int userId, string companyDB, char forPosted, dynamic incomingPayment) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                if (forPosted == 'Y')
                {
                    await connection.Request(EntitiesKeys.IncomingPayments, incomingPayment.DocEntry).PatchAsync(incomingPayment);

                    var result = await connection.Request(EntitiesKeys.IncomingPayments, incomingPayment.DocEntry).GetAsync();
                    Logger.CreateLog(false, "UPDATE INCOMING PAYMENT", "SUCCESS", JsonConvert.SerializeObject(incomingPayment));
                    return new Response
                    {
                        Status = "success",
                        Message = $"IP #{result.DocNum} UPDATED successfully!",
                        Payload = result
                    };
                }
                else
                {
                    await connection.Request(EntitiesKeys.PaymentDrafts).PostAsync<dynamic>(incomingPayment);
                    var results = await connection.Request(EntitiesKeys.PaymentDrafts).Filter("DocObjectCode eq 'bopot_IncomingPayments'").OrderBy("DocEntry desc").Top(1).GetAsync<List<dynamic>>();
                    var result = results.First();

                    await connection.Request($"{EntitiesKeys.PaymentDrafts}({incomingPayment.DocEntry})").DeleteAsync();

                    Logger.CreateLog(false, "UPDATE DRAFT INCOMING PAYMENT", "SUCCESS", JsonConvert.SerializeObject(incomingPayment));
                    return new Response
                    {
                        Status = "success",
                        Message = $"DRAFT IP #{result.DocNum} UPDATED successfully!",
                        Payload = result
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "UPDATE INCOMING PAYMENT", ex.Message, JsonConvert.SerializeObject(incomingPayment));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // CANCEL INCOMING PAYMENT
        public async Task<Response> CancelIncomingPaymentAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.IncomingPayments}({docEntry})/Cancel";

                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.IncomingPayments, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"IP #{result.DocNum} CANCELED successfully!",
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

        // CLOSE INCOMING PAYMENT
        public async Task<Response> CloseAPInvoiceAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.IncomingPayments}({docEntry})/Close";
                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.IncomingPayments, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"IP #{result.DocNum} CLOSED successfully!",
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
