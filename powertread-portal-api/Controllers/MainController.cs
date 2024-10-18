using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Context;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly MainService mainService;
        public MainController(MainDbContext mainDbContext) => mainService = new(mainDbContext);
   
        // GET SERIES - userId, companyDB, document
        [HttpGet("GetSeries/{userId}/{companyDB}/{document}")]
        public async Task<IActionResult> GetSeries(int userId, string companyDB, string document) =>
            Ok(await mainService.GetSeriesAsync(userId, companyDB, document));

        // GET PAYMENT TERMS - userId, companyDB
        [HttpGet("GetPaymentTerms/{userId}/{companyDB}")]
        public async Task<IActionResult> GetPaymentTerms(int userId, string companyDB) =>
           Ok(await mainService.GetPaymentTermsAsync(userId, companyDB));

        // GET SALES PERSONS - userId, companyDB
        [HttpGet("GetSalesPersons/{userId}/{companyDB}")]
        public async Task<IActionResult> GetSalesPersons(int userId, string companyDB) =>
          Ok(await mainService.GetSalesPersonsAsync(userId, companyDB));

        // GET SHIPPING TYPES - userId, companyDB
        [HttpGet("GetShippingTypes/{userId}/{companyDB}")]
        public async Task<IActionResult> GetShippingTypes(int userId, string companyDB) =>
          Ok(await mainService.GetShippingTypesAsync(userId, companyDB));

        // GET VAT GROUPS - userId, companyDB
        [HttpGet("GetVatGroups/{userId}/{companyDB}/{category}")]
        public async Task<IActionResult> GetVatGroups(int userId, string companyDB, char category) =>
          Ok(await mainService.GetVatGroupsAsync(userId, companyDB, category));

        // GET ITEM GROUPS - userId, companyDB
        [HttpGet("GetItemGroups/{userId}/{companyDB}")]
        public async Task<IActionResult> GetItemGroups(int userId, string companyDB) =>
          Ok(await mainService.GetItemGroupsAsync(userId, companyDB));

        // GET ITEM - userId, companyDB, itemCode
        [HttpGet("GetItem/{userId}/{companyDB}/{itemCode}")]
        public async Task<IActionResult> GetItem(int userId, string companyDB, string itemCode) => Ok(await mainService.GetItemAsync(userId, companyDB, itemCode));

        // GET PER ITEM - userId, companyDB, itemCodes
        [HttpPost("GetPerItem/{userId}/{companyDB}")]
        public async Task<IActionResult> GetPerItem(int userId, string companyDB, List<string> itemCodes) => Ok(await mainService.GetPerItemAsync(userId, companyDB, itemCodes));

        // GET PER ITEM - userId, database, itemCodes
        [HttpGet("GetBatchQty/{itemCode}/{whsCode}/{database}")]
        public async Task<IActionResult> GetBatchQty(string itemCode, string whsCode, string database) => Ok(await mainService.GetBatchQtyAsync(itemCode, whsCode, database));

        // GET PER ITEM - userId, database, itemCodes
        [HttpGet("GetSerialQty/{itemCode}/{whsCode}/{database}")]
        public async Task<IActionResult> GetSerialQty(string itemCode, string whsCode, string database) => Ok(await mainService.GetSerialQtyAsync(itemCode, whsCode, database));

        // GET BUSINESS PARTNER - userId, companyDB, cardCode
        [HttpGet("GetBusinessPartner/{userId}/{companyDB}/{cardCode}")]
        public async Task<IActionResult> GetBusinessPartner(int userId, string companyDB, string cardCode) =>
             Ok(await mainService.GetBusinessPartnerAsync(userId, companyDB, cardCode));

        // GET UOMS - userId, companyDB, ugpEntry
        [HttpGet("GetUoms/{userId}/{companyDB}/{ugpEntry}")]
        public async Task<IActionResult> GetUoms(int userId, string companyDB, int ugpEntry) =>
            Ok(await mainService.GetUomsAsync(userId, companyDB, ugpEntry));

        // GET UOM - userId, companyDB, uomEntry
        [HttpGet("GetUom/{userId}/{companyDB}/{uomEntry}")]
        public async Task<IActionResult> GetUom(int userId, string companyDB, int uomEntry) =>
            Ok(await mainService.GetUomAsync(userId, companyDB, uomEntry));

        // GET EXCHANGE RATES - userId, companyDB, postingDate
        [HttpGet("GetExchangeRates/{userId}/{companyDB}/{postingDate}")]
        public async Task<IActionResult> GetUoms(int userId, string companyDB, string postingDate) =>
           Ok(await mainService.GetExchangeRatesAsync(userId, companyDB, postingDate));

        // GET ADMIN INFO - userId, companyDB
        [HttpGet("GetAdminInfo/{userId}/{companyDB}")]
        public async Task<IActionResult> GetAdminInfo(int userId, string companyDB) =>
           Ok(await mainService.GetAdminInfoAsync(userId, companyDB));

        // GET COUNTRY - userId, companyDB, code
        [HttpGet("GetCountry/{userId}/{companyDB}/{code}")]
        public async Task<IActionResult> GetCountry(int userId, string companyDB, string code) =>
           Ok(await mainService.GetCountryAsync(userId, companyDB, code));

        // GET GL ACCOUNT ADVANCED RULES - userId, companyDB, itemsGroupCode, financialYear
        [HttpGet("GetGLAccountAdvancedRules/{userId}/{companyDB}/{itemsGroupCode}/{financialYear}")]
        public async Task<IActionResult> GetGLAccountAdvancedRules(int userId, string companyDB, int itemsGroupCode, int financialYear) =>
           Ok(await mainService.GetGLAccountAdvancedRulesAsync(userId, companyDB, itemsGroupCode, financialYear));

        // GET EMPLOYEES
        [HttpGet("GetEmployees/{userId}/{companyDB}")]
        public async Task<IActionResult> GetEmployees(int userId, string companyDB) => Ok(await mainService.GetEmployeesAsync(userId, companyDB));

        // GET BRANCHES
        [HttpGet("GetBranches/{userId}/{companyDB}")]
        public async Task<IActionResult> GetBranches(int userId, string companyDB) => Ok(await mainService.GetBranchesAsync(userId, companyDB));

        // GET DEPARTMENTS
        [HttpGet("GetDepartments/{userId}/{companyDB}")]
        public async Task<IActionResult> GetDepartments(int userId, string companyDB) => Ok(await mainService.GetDepartmentsAsync(userId, companyDB));

        // GET COST CENTER
        [HttpGet("GetCostCenter/{userId}/{companyDB}/{centerCode}")]
        public async Task<IActionResult> GetCostCenter(int userId, string companyDB, string centerCode) => Ok(await mainService.GetCostCenterAsync(userId, companyDB, centerCode));

        // GET COST CENTERS
        [HttpGet("GetCostCenters/{userId}/{companyDB}/{dimCode}")]
        public async Task<IActionResult> GetCostCenters(int userId, string companyDB, int dimCode) => Ok(await mainService.GetCostCentersAsync(userId, companyDB, dimCode));

        // GET ATTACHMENT
        [HttpGet("GetAttachment/{userId}/{companyDB}/{attachmentEntry}")]
        public async Task<IActionResult> GetAttachment(int userId, string companyDB, int attachmentEntry) => Ok(await mainService.GetAttachmentAsync(userId, companyDB, attachmentEntry));

        // GET ATTACHMENT FILE
        [HttpGet("GetAttachmentFile/{userId}/{companyDB}/{attachmentEntry}/{fileName}")]
        public async Task<IActionResult> GetAttachmentFile(int userId, string companyDB, int attachmentEntry, string fileName) => Ok(await mainService.GetAttachmentFileAsync(userId, companyDB, attachmentEntry, fileName));

        // CREATE ATTACHMENT FILE
        [HttpGet("CreateAttachmentFile/{userId}/{companyDB}/{fileName}")]
        public async Task<IActionResult> CreateAttachmentFile(int userId, string companyDB, List<AttachmentParams> attachmentParams) => Ok(await mainService.CreateAttachmentAsync(userId, companyDB, attachmentParams));

        // GET DOCUMENT LINES CHART OF ACCOUNTS
        [HttpPost("GetDocumentLinesChartOfAccounts/{userId}/{companyDB}")]
        public async Task<IActionResult> GetDocumentLinesChartOfAccounts(int userId, string companyDB, List<string> accountCodes) => Ok(await mainService.GetDocumentLinesChartOfAccountsAsync(userId, companyDB, accountCodes));

        // GET DOCUMENT LINES BUSINESS PARTNERS
        [HttpPost("GetDocumentLinesBusinessPartners/{userId}/{companyDB}")]
        public async Task<IActionResult> GetDocumentLinesBusinessPartners(int userId, string companyDB, List<string> bpCodes) => Ok(await mainService.GetDocumentLinesBusinessPartnersAsync(userId, companyDB, bpCodes));

        // GET WAREHOUSES
        [HttpPost("GetWarehouses/{userId}/{companyDB}")]
        public async Task<IActionResult> GetWarehouses(int userId, string companyDB, List<string> warehouseCodes) => Ok(await mainService.GetWarehousesAsync(userId, companyDB, warehouseCodes));

        // GET ALL WAREHOUSES
        [HttpGet("GetAllWarehouses/{userId}/{companyDB}")]
        public async Task<IActionResult> GetAllWarehouses(int userId, string companyDB) => Ok(await mainService.GetAllWarehousesAsync(userId, companyDB));

        // GET WITHOLDING TAX CODES
        [HttpPost("GetWithholdingTaxCodes/{userId}/{companyDB}")]
        public async Task<IActionResult> GetWitholdingTaxCodes(int userId, string companyDB, List<string> witholdingTaxCodes) => Ok(await mainService.GetWitholdingTaxCodesAsync(userId, companyDB, witholdingTaxCodes));

        // GET PAGE DATA - userId, companyDB, objType, actionType, docEntry, series
        [HttpGet("GetPageData/{userId}/{companyDB}/{objType}/{actionType}/{docEntry}/{series}")]
        public async Task<IActionResult> GetPageData(int userId, string companyDB, int objType, char actionType, int docEntry, int series) => Ok(await mainService.GetPageDataAsync(userId, companyDB, objType, actionType, docEntry, series));

        // GET PAGE DATA MULTI - userId, companyDB, objType, actionType, docEntry, series
        [HttpGet("GetPageDataMulti/{userId}/{companyDB}/{objType}/{actionType}/{docEntry}/{series}")]
        public async Task<IActionResult> GetPageDataMulti(int userId, string companyDB, int objType, char actionType, int docEntry, int series) => Ok(await mainService.GetPageDataMultiAsync(userId, companyDB, objType, actionType, docEntry, series));

        // GET PAGE BP DATA - userId, companyDB, actionType, cardCode
        [HttpGet("GetPageBPData/{userId}/{companyDB}/{actionType}/{cardCode}")]
        public async Task<IActionResult> GetPageBPData(int userId, string companyDB, char actionType, string cardCode) => Ok(await mainService.GetPageBPDataAsync(userId, companyDB, actionType, cardCode));

        // GET PAGE ITEM DATA - userId, companyDB, actionType, itemCode
        [HttpGet("GetPageItemData/{userId}/{companyDB}/{actionType}/{itemCode}")]
        public async Task<IActionResult> GetPageItemData(int userId, string companyDB, char actionType, string itemCode) => Ok(await mainService.GetPageItemDataAsync(userId, companyDB, actionType, itemCode));

        // GET DOCUMENT DATA - userId, companyDB, objType, docEntry
        [HttpGet("GetDocumentData/{userId}/{companyDB}/{objType}/{docEntry}")]
        public async Task<IActionResult> GetDocumentData(int userId, string companyDB, int objType, int docEntry) => Ok(await mainService.GetDocumentDataAsync(userId, companyDB, objType, docEntry));

        // GET RELATIONSHIPMAP
        [HttpGet("GetRelationshipMap/{userId}/{companyDB}/{docEntry}/{objType}")]
        public async Task<IActionResult> GetRelationshipMap(int userId, string companyDB, int docEntry, int objType) => Ok(await mainService.GetRelationshipMapAsync(userId, companyDB, docEntry, objType));

        // PURCHASING
        // GET INIT PURCHASE REQUEST
        [HttpPost("GetInitPurchaseRequest/{userId}/{companyDB}")]
        public async Task<IActionResult> GetInitPurchaseRequest(int userId, string companyDB, InitParams initParams) => Ok(await mainService.GetInitPurchaseRequestAsync(userId, companyDB, initParams));

        // GET INIT PURCHASE ORDER
        [HttpPost("GetInitPurchaseOrder/{userId}/{companyDB}")]
        public async Task<IActionResult> GetInitPurchaseOrder(int userId, string companyDB, InitParams initParams) => Ok(await mainService.GetInitPurchaseOrderAsync(userId, companyDB, initParams));

        // GET INIT GOODS RECEIPT PO
        [HttpPost("GetInitGoodsReceiptPO/{userId}/{companyDB}")]
        public async Task<IActionResult> GetInitGoodsReceiptPO(int userId, string companyDB, InitParams initParams) => Ok(await mainService.GetInitGoodsReceiptPOAsync(userId, companyDB, initParams));

        // GET INIT GOODS RETURN
        [HttpPost("GetInitPurchaseReturn/{userId}/{companyDB}")]
        public async Task<IActionResult> GetInitPurchaseReturn(int userId, string companyDB, InitParams initParams) => Ok(await mainService.GetInitPurchaseReturnAsync(userId, companyDB, initParams));

        // GET INIT AP INVOICE
        [HttpPost("GetInitAPInvoice/{userId}/{companyDB}")]
        public async Task<IActionResult> GetInitAPInvoice(int userId, string companyDB, InitParams initParams) => Ok(await mainService.GetInitAPInvoiceAsync(userId, companyDB, initParams));

        // GET INIT AP INVOICE
        [HttpPost("GetInitAPCreditMemo/{userId}/{companyDB}")]
        public async Task<IActionResult> GetInitAPCreditMemo(int userId, string companyDB, InitParams initParams) => Ok(await mainService.GetInitAPCreditMemoAsync(userId, companyDB, initParams));

        // SALES
        // GET INIT SALES QUOTATION
        [HttpPost("GetInitSalesQuotation/{userId}/{companyDB}")]
        public async Task<IActionResult> GetInitSalesQuotation(int userId, string companyDB, InitParams initParams) => Ok(await mainService.GetInitSalesQuotationAsync(userId, companyDB, initParams));

        // GET INIT SALES ORDER
        [HttpPost("GetInitSalesOrder/{userId}/{companyDB}")]
        public async Task<IActionResult> GetInitSalesOrder(int userId, string companyDB, InitParams initParams) => Ok(await mainService.GetInitSalesOrderAsync(userId, companyDB, initParams));

        // GET INIT DELIVERY
        [HttpPost("GetInitDelivery/{userId}/{companyDB}")]
        public async Task<IActionResult> GetInitDelivery(int userId, string companyDB, InitParams initParams) => Ok(await mainService.GetInitDeliveryAsync(userId, companyDB, initParams));

        // GET INIT AR RETURN
        [HttpPost("GetInitReturn/{userId}/{companyDB}")]
        public async Task<IActionResult> GetInitReturn(int userId, string companyDB, InitParams initParams) => Ok(await mainService.GetInitReturnAsync(userId, companyDB, initParams));

        // GET INIT AR INVOICE
        [HttpPost("GetInitARInvoice/{userId}/{companyDB}")]
        public async Task<IActionResult> GetInitARInvoice(int userId, string companyDB, InitParams initParams) => Ok(await mainService.GetInitARInvoiceAsync(userId, companyDB, initParams));

        // GET INIT AR CREDIT MEMO
        [HttpPost("GetInitARCreditMemo/{userId}/{companyDB}")]
        public async Task<IActionResult> GetInitARCreditMemo(int userId, string companyDB, InitParams initParams) => Ok(await mainService.GetInitARCreditMemoAsync(userId, companyDB, initParams));

        // INVENTORY
        // GET INIT GOODS RECEIPT
        [HttpGet("GetInitGoodsReceipt/{userId}/{companyDB}")]
        public async Task<IActionResult> GetInitGoodsReceipt(int userId, string companyDB, InitParams initParams) => Ok(await mainService.GetInitGoodsReceiptAsync(userId, companyDB, initParams));

        // GET INIT GOODS ISSUE
        [HttpPost("GetInitGoodsIssue/{userId}/{companyDB}")]
        public async Task<IActionResult> GetInitGoodsIssue(int userId, string companyDB, InitParams initParams) => Ok(await mainService.GetInitGoodsIssueAsync(userId, companyDB, initParams));

        // GET INIT INVENTORY TRANSFER REQUEST
        [HttpPost("GetInitInventoryTransferRequest/{userId}/{companyDB}")]
        public async Task<IActionResult> GetInitInventoryTransferRequest(int userId, string companyDB, InitParams initParams) => Ok(await mainService.GetInitInventoryTransferRequestAsync(userId, companyDB, initParams));

        // GET INIT INVENTORY TRANSFER
        [HttpPost("GetInitInventoryTransfer/{userId}/{companyDB}")]
        public async Task<IActionResult> GetInitInventoryTransfer(int userId, string companyDB, InitParams initParams) => Ok(await mainService.GetInitInventoryTransferAsync(userId, companyDB, initParams));

        // BANKING
        // GET INIT INCOMING PAYMENT
        [HttpGet("GetInitIncomingPayment/{userId}/{companyDB}")]
        public async Task<IActionResult> GetInitIncomingPayment(int userId, string companyDB) => Ok(await mainService.GetInitIncomingPaymentAsync(userId, companyDB));

        // GET INIT OUTGOING PAYMENT
        [HttpPost("GetInitOutgoingPayment/{userId}/{companyDB}")]
        public async Task<IActionResult> GetInitOutgoingPayment(int userId, string companyDB, InitParams initParams) => Ok(await mainService.GetInitOutgoingPaymentAsync(userId, companyDB, initParams));
    }
}
