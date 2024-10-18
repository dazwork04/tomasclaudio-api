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
    public class OutgoingPaymentService
    {
        private readonly MainDbContext mainDbContext;
        private readonly string connString;
        public OutgoingPaymentService(MainDbContext mainDbContext)
        {
            this.mainDbContext = mainDbContext;
            connString = mainDbContext.Database.GetDbConnection().ConnectionString;
        }

        // GET OUTGOING PAYMENTS
        public async Task<Response> GetOutgoingPaymentsAsync(int userId, string companyDB, string cancelled, string dateFrom, string dateTo, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                string queryFilter = $"Cancelled eq '{cancelled}' and DocDate ge '{dateFrom}' and DocDate le '{dateTo}'" + paginate.Filter;


                var count = await connection.Request(EntitiesKeys.VendorPayments)
                    .Filter(queryFilter)
                    .GetCountAsync();

                var result = await connection.Request(EntitiesKeys.VendorPayments)
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

        // GET OUTGOING PAYMENT OTHER DATA
        public async Task<Response> GetOutgoingPaymentOtherDataAsync(string database, int docEntry, MainDbContext mainDbContext) => await Task.Run(() =>
        {
            try
            {
                mainDbContext.Database.GetDbConnection().ConnectionString = string.Format(connString, database);

                var result = (from a in mainDbContext.OVPM
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
            catch (Exception ex)
            {
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message,
                };
            }
        });

        // GET OUTGOING PAYMENT INVOICES
        public async Task<Response> GetOutgoingPaymentLinesAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                PaymentObject outgoingPayment = await connection.Request(EntitiesKeys.VendorPayments, docEntry).GetAsync<PaymentObject>();

                List<PaymentInvoiceData> paymentInvoices = [];
                foreach (var paymentInvoice in outgoingPayment.PaymentInvoices)
                {
                    string reqParam = ObjectTypesHelper.GetPaymentInvoiceType(paymentInvoice.InvoiceType);
                    if (reqParam != string.Empty)
                    {
                        var result = await connection.Request(reqParam, paymentInvoice.DocEntry).GetAsync<dynamic>();

                        if (paymentInvoice.InvoiceType == "it_JournalEntry")
                        {
                            double sum = 0;
                            foreach (var item in result["JournalEntryLines"])
                            {
                                string cardCode = item["ShortName"];
                                int lineID = item["Line_ID"];

                                if (cardCode == outgoingPayment.CardCode && lineID == paymentInvoice.DocLine)
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
                                OverdueDays = (int)(DateTime.Parse(outgoingPayment.DocDate) - DateTime.Parse(dueDate)).TotalDays,
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
                                BalanceDue = docTotal - sumApplied,
                                DocDate = docDate,
                                DocNum = docNum,
                                NumAtCard = numAtCard,
                                DocType = paymentInvoice.InvoiceType,
                                OverdueDays = (int)(DateTime.Parse(outgoingPayment.DocDate) - DateTime.Parse(dueDate)).TotalDays,
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

                Logger.CreateLog(true, "GET OUTGOING PAYMENT LINES", ex.Message, JsonConvert.SerializeObject(new { DocEntry = docEntry }));
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

                // AP Invoice
                var ariResult = await connection.Request(EntitiesKeys.PurchaseInvoices).Filter($"CardCode eq '{cardCode}' and DocumentStatus eq 'O'").OrderBy("DocEntry asc").GetAsync<List<Document>>();

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
                        DocType = "it_PurchaseInvoice",
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
                                && a.TransType != 18 && a.ShortName == cardCode
                                select new PaymentInvoiceData
                                {
                                    Checked = false,
                                    DocEntry = a.TransId,
                                    DocNum = a.TransId,
                                    NumAtCard = a.TransId.ToString(),
                                    DocDate = a.RefDate.ToString("MM/dd/yyyy"),
                                    DocType = "it_JournalEntry",
                                    LineNum = 0,
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

                Logger.CreateLog(true, "GET OUTGOING PAYMENT LINES", ex.Message, JsonConvert.SerializeObject(new { CardCode = cardCode }));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // CREATE OUTGOING PAYMENT
        public async Task<Response> CreateOutgoingPaymentAsync(int userId, string companyDB, PostPaymentObject outgoingPayment) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                var result = await connection.Request(EntitiesKeys.VendorPayments).PostAsync<dynamic>(outgoingPayment);

                Logger.CreateLog(false, "CREATE OUTGOING PAYMENT", "SUCCESS", JsonConvert.SerializeObject(outgoingPayment));
                return new Response
                {
                    Status = "success",
                    Message = $"OP #{result.DocNum} CREATED successfully!",
                    Payload = result
                };
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CREATE OUTGOING PAYMENT", ex.Message, JsonConvert.SerializeObject(outgoingPayment));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // UPDATE OUTGOING PAYMENT
        public async Task<Response> UpdateOutgoingPaymentAsync(int userId, string companyDB, PaymentObject outgoingPayment) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                await connection.Request(EntitiesKeys.VendorPayments, outgoingPayment.DocEntry).PatchAsync(outgoingPayment);

                var result = await connection.Request(EntitiesKeys.VendorPayments, outgoingPayment.DocEntry).GetAsync();
                Logger.CreateLog(false, "UPDATE OUTGOING PAYMENT", "SUCCESS", JsonConvert.SerializeObject(outgoingPayment));
                return new Response
                {
                    Status = "success",
                    Message = $"OP #{result.DocNum} UPDATED successfully!",
                    Payload = result
                };
            }
            catch (Exception ex)
            {

                Logger.CreateLog(true, "UPDATE OUTGOING PAYMENT", ex.Message, JsonConvert.SerializeObject(outgoingPayment));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // CANCEL OUTGOING PAYMENT
        public async Task<Response> CancelOutgoingPaymentAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.VendorPayments}({docEntry})/Cancel";

                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.VendorPayments, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"OP #{result.DocNum} CANCELED successfully!",
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

        // CLOSE OUTGOING PAYMENT
        public async Task<Response> CloseOutgoingPaymentAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.VendorPayments}({docEntry})/Close";
                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.VendorPayments, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"OP #{result.DocNum} CLOSED successfully!",
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
