using B1SLayer;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SLayerConnectionLib;

namespace SAPB1SLayerWebAPI.Services
{
    public class HomeService
    {
        // GET DOCUMENTS COUNT
        public async Task<Response> GetDocumentsCount(int userId, string companyDB) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string queryFilter = "DocumentStatus eq 'O'";

                // SALES
                var salesQuotationCount = await connection.Request(EntitiesKeys.Quotations).Filter(queryFilter).GetCountAsync();
                var salesOrderCount = await connection.Request(EntitiesKeys.Orders).Filter(queryFilter).GetCountAsync();
                var deliveryCount = await connection.Request(EntitiesKeys.DeliveryNotes).Filter(queryFilter).GetCountAsync();
                var returnCount = await connection.Request(EntitiesKeys.Returns).Filter(queryFilter).GetCountAsync();
                var arInvoiceCount = await connection.Request(EntitiesKeys.Invoices).Filter(queryFilter).GetCountAsync();
                var arCreditMemoCount = await connection.Request(EntitiesKeys.CreditNotes).Filter(queryFilter).GetCountAsync();

                // PURCHASING
                var purchaseRequestCount = await connection.Request(EntitiesKeys.PurchaseRequests).Filter(queryFilter).GetCountAsync();
                var purchaseOrderCount = await connection.Request(EntitiesKeys.PurchaseOrders).Filter(queryFilter).GetCountAsync();
                var goodsReceiptPOCount = await connection.Request(EntitiesKeys.PurchaseDeliveryNotes).Filter(queryFilter).GetCountAsync();
                var purchaseReturnCount = await connection.Request(EntitiesKeys.PurchaseReturns).Filter(queryFilter).GetCountAsync();
                var apInvoiceCount = await connection.Request(EntitiesKeys.PurchaseInvoices).Filter(queryFilter).GetCountAsync();


                // INVENTORY
                var pickListCount = await connection.Request(EntitiesKeys.PickLists).GetCountAsync();
                var goodsReceiptCount = await connection.Request(EntitiesKeys.InventoryGenEntries).Filter(queryFilter).GetCountAsync();
                var goodsIssueCount = await connection.Request(EntitiesKeys.InventoryGenExits).Filter(queryFilter).GetCountAsync();
                var inventoryTransferRequestCount = await connection.Request(EntitiesKeys.InventoryTransferRequests).Filter(queryFilter).GetCountAsync();
                var inventoryTransferCount = await connection.Request(EntitiesKeys.StockTransfers).Filter(queryFilter).GetCountAsync();

                // BANKING
                var incomingPaymentCount = await connection.Request(EntitiesKeys.IncomingPayments).GetCountAsync();
                var outgoingPaymentCount = await connection.Request(EntitiesKeys.VendorPayments).GetCountAsync();

                return new Response
                {
                    Status = "success",
                    Payload = new DocumentsCount
                    {
                        SalesQuotation = salesQuotationCount,
                        SalesOrder = salesOrderCount,
                        Delivery = deliveryCount,
                        Return = returnCount,
                        ARInvoice = arInvoiceCount,
                        ARCreditNote = arCreditMemoCount,
                        //
                        PurchaseRequest = purchaseRequestCount,
                        PurchaseOrder = purchaseOrderCount,
                        GoodsReceiptPO = goodsReceiptPOCount,
                        PurchaseReturn = purchaseReturnCount,
                        APInvoice = apInvoiceCount,
                        //
                        PickList = pickListCount,
                        GoodsReceipt = goodsReceiptCount,
                        GoodsIssue = goodsIssueCount,
                        InventoryTransferRequest = inventoryTransferRequestCount,
                        InventoryTransfer = inventoryTransferCount,
                        //
                        IncomingPayment = incomingPaymentCount,
                        OutgoingPayment = outgoingPaymentCount
                    }
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
