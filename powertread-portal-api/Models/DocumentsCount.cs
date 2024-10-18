namespace SAPB1SLayerWebAPI.Models
{
    public class DocumentsCount
    {
        // SALES
        public long SalesQuotation { get; set; }
        public long SalesOrder { get; set; }
        public long Delivery { get; set; }
        public long Return { get; set; }
        public long ARInvoice { get; set; }
        public long ARCreditNote { get; set; }

        // PURCHASING
        public long PurchaseRequest { get; set; }
        public long PurchaseOrder { get; set; }
        public long GoodsReceiptPO { get; set; }
        public long PurchaseReturn { get; set; }
        public long APInvoice { get; set; }
        // INVENTORY
        public long PickList { get; set; }
        public long GoodsReceipt { get; set; }
        public long GoodsIssue { get; set; }
        public long InventoryTransferRequest { get; set; }
        public long InventoryTransfer { get; set; }
        //
        public long IncomingPayment { get; set; }
        public long OutgoingPayment { get; set; }

    }
}
