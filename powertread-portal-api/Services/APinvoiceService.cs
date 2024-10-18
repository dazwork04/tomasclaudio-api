using B1SLayer;
using Newtonsoft.Json;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Utils;
using SLayerConnectionLib;

namespace SAPB1SLayerWebAPI.Services
{
    public class APinvoiceService
    {
        // GET AP INVOICES
        public async Task<Response> GetAPInvoicesAsync(int userId, string companyDB, char status, char cancelled, string dateFrom, string dateTo, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                string queryFilter = $"DocumentStatus eq '{status}' and Cancelled eq '{cancelled}' and DocDate ge '{dateFrom}' and DocDate le '{dateTo}'" + paginate.Filter;


                var count = await connection.Request(EntitiesKeys.PurchaseInvoices)
                    .Filter(queryFilter)
                    .GetCountAsync();

                var result = await connection.Request(EntitiesKeys.PurchaseInvoices)
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

        // CREATE AP INVOICE
        public async Task<Response> CreateAPInvoiceAsync(int userId, string companyDB, dynamic apInvoice) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                var result = await connection.Request(EntitiesKeys.PurchaseInvoices).PostAsync<dynamic>(apInvoice);

                Logger.CreateLog(false, "CREATE AP INVOICE", "SUCCESS", JsonConvert.SerializeObject(apInvoice));
                return new Response
                {
                    Status = "success",
                    Message = $"API #{result.DocNum} CREATED successfully!",
                    Payload = result
                };
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CREATE AP INVOICE", ex.Message, JsonConvert.SerializeObject(apInvoice));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // UPDATE AP INVOICE
        public async Task<Response> UpdateAPInvoiceAsync(int userId, string companyDB, dynamic apInvoice) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                await connection.Request(EntitiesKeys.PurchaseInvoices, apInvoice.DocEntry).PutAsync(apInvoice);

                var result = await connection.Request(EntitiesKeys.PurchaseInvoices, apInvoice.DocEntry).GetAsync();
                Logger.CreateLog(false, "UPDATE AP INVOICE", "SUCCESS", JsonConvert.SerializeObject(apInvoice));
                return new Response
                {
                    Status = "success",
                    Message = $"API #{result.DocNum} UPDATED successfully!",
                    Payload = result
                };
            }
            catch (Exception ex)
            {

                Logger.CreateLog(true, "UPDATE AP INVOICE", ex.Message, JsonConvert.SerializeObject(apInvoice));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // CANCEL AP INVOICE
        public async Task<Response> CancelAPInvoiceAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.PurchaseInvoices}({docEntry})/Cancel";

                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.PurchaseInvoices, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"API #{result.DocNum} CANCELED successfully!",
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

        // CLOSE AP INVOICE
        public async Task<Response> CloseAPInvoiceAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = $"{EntitiesKeys.PurchaseInvoices}({docEntry})/Close";
                await connection.Request(reqParam).PostAsync();
                var result = await connection.Request(EntitiesKeys.PurchaseInvoices, docEntry).GetAsync();

                return new Response
                {
                    Status = "success",
                    Message = $"API #{result.DocNum} CLOSED successfully!",
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

        // GET GOODS RECEIPT POS
        public async Task<Response> GetGoodsReceiptPOsAsync(int userId, string companyDB, string cardCode, string docType, string priceMode) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                var goodsReceiptPOs = await connection.Request(EntitiesKeys.PurchaseDeliveryNotes)
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
    }
}
