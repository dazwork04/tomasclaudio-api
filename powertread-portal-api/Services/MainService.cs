using B1SLayer;
using Microsoft.EntityFrameworkCore;
using SAPB1SLayerWebAPI.Context;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Models.EFCore;
using SAPB1SLayerWebAPI.Utils;
using SLayerConnectionLib;
using SAPB1SLayerWebAPI.EFCore;

namespace SAPB1SLayerWebAPI.Services
{
    public class MainService
    {
        private readonly MainDbContext mainDbContext;
        private readonly string connString;
        public MainService(MainDbContext mainDbContext)
        {
            this.mainDbContext = mainDbContext;
            connString = mainDbContext.Database.GetDbConnection().ConnectionString;
        }

        // GET SERIES
        public async Task<Response> GetSeriesAsync(int userId, string companyDB, string document) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                
                var parameters = new { DocumentTypeParams = new { Document = document } };
                string reqParam = $"{ActionsKeys.SeriesService}_GetDocumentSeries";

                var result = await connection.Request(reqParam)
                    .OrderBy("Name")
                    .PostAsync<List<SLSeries>>(parameters);


                return new Response
                {
                    Status = "success",
                    Payload = result
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

        // GET PAYMENT TERMS
        public async Task<Response> GetPaymentTermsAsync(int userId, string companyDB) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                var result = await connection.Request(EntitiesKeys.PaymentTermsTypes)
                    //.Select("GroupNumber, PaymentTermsGroupName, NumberOfAdditionalDays")
                    .OrderBy("PaymentTermsGroupName")
                    .GetAsync<List<SLPaymentTermType>>();

                return new Response
                {
                    Status = "success",
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

        // GET SALES PERSOMS
        public async Task<Response> GetSalesPersonsAsync(int userId, string companyDB) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                var result = await connection.Request(EntitiesKeys.SalesPersons)
                    //.Select("SalesEmployeeCode, SalesEmployeeName")
                    .Filter("Active eq 'Y'")
                    .OrderBy("SalesEmployeeName")
                    .GetAsync<List<SalesPerson>>();

                return new Response
                {
                    Status = "success",
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

        // GET SHIPPING TYPE
        public async Task<Response> GetShippingTypesAsync(int userId, string companyDB) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                var result = await connection.Request(EntitiesKeys.ShippingTypes)
                    //.Select("Code, Name")
                    .OrderBy("Name")
                    .GetAsync<List<ShippingType>>();

                return new Response
                {
                    Status = "success",
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

        //GET VAT GROUPS
        public async Task<Response> GetVatGroupsAsync(int userId, string companyDB, char category) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                var result = await connection.Request(EntitiesKeys.VatGroups)
                    //.Select("Code, Name, VatGroups_Lines")
                    .Filter($"Inactive eq 'N' and Category eq '{category}'")
                    .GetAsync<List<VatGroup>>();

                return new Response
                {
                    Status = "success",
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

        //GET ITEM GROUPS
        public async Task<Response> GetItemGroupsAsync(int userId, string companyDB) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                var result = await connection.Request(EntitiesKeys.ItemGroups)
                    //.Select("Number, GroupName")
                    .OrderBy("GroupName")
                    .GetAsync<List<ItemGroup>>();

                return new Response
                {
                    Status = "success",
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

        //GET ITEMS
        public async Task<Response> GetItemsAsync(int userId, string companyDB, short itemsGroupCode) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                var result = await connection.Request(EntitiesKeys.Items)
                    .Filter($"ItemsGroupCode eq {itemsGroupCode}")
                    .OrderBy("ItemName")
                    .GetAsync<List<Item>>();

                return new Response
                {
                    Status = "success",
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

        // GET ITEM
        public async Task<Response> GetItemAsync(int userId, string companyDB, string itemCode) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                var result = await connection.Request(EntitiesKeys.Items, itemCode).GetAsync<Item>();
                return new Response
                {
                    Status = "success",
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
        // GET PER ITEM
        public async Task<Response> GetPerItemAsync(int userId, string companyDB, List<string> itemCodes) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string filter = "";
                for (int i = 0; i < itemCodes.Count; i++)
                {
                    filter += $"ItemCode eq '{itemCodes[i]}'";
                    if (i < itemCodes.Count - 1) filter += " or ";
                }

                var result = await connection.Request(EntitiesKeys.Items)
                    .Filter(filter)
                    .GetAllAsync<Item>();

                return new Response
                {
                    Status = "success",
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
        // GET BATCH TRANSACTIONS
        public Task<Response> GetBatchQtyAsync(string itemCode, string whsCode, string database) => Task.Run(() =>
        {
            try
            {
                mainDbContext.Database.GetDbConnection().ConnectionString = string.Format(connString, database);

                List<BatchQty> items = mainDbContext.OIBT.Where(o => o.Quantity > 0 && o.ItemCode == itemCode && o.WhsCode == whsCode)
                    .Select(o => new BatchQty
                    {
                        ItemCode = o.ItemCode,
                        ItemName = o.ItemName,
                        BatchNum = o.BatchNum,
                        WhsCode = o.WhsCode,
                        Quantity = o.Quantity,
                    }).ToList();

                return new Response()
                {
                    Status = "success",
                    Payload = items
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // GET SERIAL TRANSACTIONS
        public Task<Response> GetSerialQtyAsync(string itemCode, string whsCode, string database) => Task.Run(() =>
        {
            try
            {
                mainDbContext.Database.GetDbConnection().ConnectionString = string.Format(connString, database);

                List<SerialQty> items = mainDbContext.OSRI.Where(o => o.Status == 0 && o.ItemCode == itemCode && o.WhsCode == whsCode)
                    .Select(o => new SerialQty
                    {
                        ItemCode = o.ItemCode,
                        ItemName = o.ItemName,
                        SerialNum = o.IntrSerial,
                        WhsCode = o.WhsCode,
                        Quantity = o.Status == 0 ? 1 : 0,
                        ManufacturerSerialNumber = o.SuppSerial,
                        ExpiryDate = o.ExpDate,
                        ManufactureDate = o.PrdDate,
                        ReceptionDate = o.InDate
                    }).ToList();

                return new Response()
                {
                    Status = "success",
                    Payload = items
                };
            }
            catch (Exception ex)
            {
                return new Response()
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        //GET BUSINESS PARTNERS
        public async Task<Response> GetBusinessPartnersAsync(int userId, string companyDB, char cardType) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                var result = await connection.Request(EntitiesKeys.BusinessPartners)
                    .Filter($"CardType eq '{cardType}' and Frozen eq 'N'")
                    .OrderBy("CardName")
                    .GetAsync<List<BusinessPartner>>();

                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        result.Count,
                        Data = result
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

        // GET BUSINESS PARTNER
        public async Task<Response> GetBusinessPartnerAsync(int userId, string companyDB, string cardCode) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                var result = await connection.Request(EntitiesKeys.BusinessPartners, cardCode).GetAsync<BusinessPartner>();

                return new Response
                {
                    Status = "success",
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

        // GET UOMS
        public async Task<Response> GetUomsAsync(int userId, string companyDB, int ugpEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                var uomGroups = await connection.Request(EntitiesKeys.UnitOfMeasurementGroups, ugpEntry)
                    .GetAsync<UomGroup>();

                var collection = uomGroups.UoMGroupDefinitionCollection;

                List<Uom> uomLists = [];

                foreach (var item in collection)
                {
                    int uomEntry = item.AlternateUoM;
                    var uom = await connection.Request(EntitiesKeys.UnitOfMeasurements, uomEntry).GetAsync<Uom>();
                    uomLists.Add(uom);
                }

                return new Response
                {
                    Status = "success",
                    Payload = uomLists
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

        // GET UOM
        public async Task<Response> GetUomAsync(int userId, string companyDB, int uomEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                var uom = await connection.Request(EntitiesKeys.UnitOfMeasurements, uomEntry)
                    .GetAsync<Uom>();

                return new Response
                {
                    Status = "success",
                    Payload = uom
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

        // GET EXCHANGE RATES
        public async Task<Response> GetExchangeRatesAsync(int userId, string companyDB, string postingDate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                var body = new { ParamList = $"postingDate='{postingDate}'" };
                string request = $"{EntitiesKeys.SQLQueries}('getCurrencyRates')/List";
                var result = await connection.Request(request).PostAsync<List<ExchangeRate>>(body);

                return new Response
                {
                    Status = "success",
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

        // GET ADMIN INFO
        public async Task<Response> GetAdminInfoAsync(int userId, string companyDB) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string request = $"{ActionsKeys.CompanyService}_GetAdminInfo";
                AdminInfo result = await connection.Request(request)
                    .PostAsync<AdminInfo>();

                return new Response
                {
                    Status = "success",
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

        // GET COUNTRY
        public async Task<Response> GetCountryAsync(int userId, string companyDB, string code) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                var result = await connection.Request(EntitiesKeys.Countries, code)
                    .GetAsync<Country>();

                return new Response
                {
                    Status = "success",
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

        // GET GL ACCOUNT ADVANCED RULES
        public async Task<Response> GetGLAccountAdvancedRulesAsync(int userId, string companyDB, int itemsGroupCode, int financialYear) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                var result = await connection.Request(EntitiesKeys.GLAccountAdvancedRules)
                    .Filter($"ItemGroup eq {itemsGroupCode} and IsActive eq 'Y' and FinancialYear eq {financialYear}")
                    .GetAsync<List<GLAccountAdvancedRule>>();

                if (result.Count == 0)
                {
                    result = await connection.Request(EntitiesKeys.GLAccountAdvancedRules)
                        .Filter($"ItemGroup eq -1 and IsActive eq 'Y' and FinancialYear eq {financialYear}")
                        .GetAsync<List<GLAccountAdvancedRule>>();
                }

                return new Response
                {
                    Status = "success",
                    Payload = result.Count > 0 ? result[0] : null
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

        // GET EMPLOYEES
        public async Task<Response> GetEmployeesAsync(int userId, string companyDB) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                var result = await connection.Request(EntitiesKeys.EmployeesInfo)
                    .Filter("Active eq 'Y'")
                    .OrderBy("EmployeeCode")
                    .GetAllAsync<EmployeeInfo>();

                return new Response
                {
                    Status = "success",
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

        // GET BRANCHES
        public async Task<Response> GetBranchesAsync(int userId, string companyDB) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                var result = await connection.Request(EntitiesKeys.Branches).OrderBy("Name")
                    .GetAllAsync<Branch>();

                return new Response
                {
                    Status = "success",
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

        // GET DEPARTMENTS
        public async Task<Response> GetDepartmentsAsync(int userId, string companyDB) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                var result = await connection.Request(EntitiesKeys.Departments).OrderBy("Name")
                    .GetAllAsync<Department>();

                return new Response
                {
                    Status = "success",
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

        // GET COST CENTER
        public async Task<Response> GetCostCenterAsync(int userId, string companyDB, string centerCode) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                var result = await connection.Request(EntitiesKeys.ProfitCenters, centerCode)
                    .GetAsync<CostCenter>();

                return new Response
                {
                    Status = "success",
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

        // GET COST CENTERS
        public async Task<Response> GetCostCentersAsync(int userId, string companyDB, int dimCode) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                var result = await connection.Request(EntitiesKeys.ProfitCenters).Filter($"InWhichDimension eq {dimCode}").GetAllAsync<CostCenter>();

                return new Response
                {
                    Status = "success",
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

        // GET ATTACHMENT
        public async Task<Response> GetAttachmentAsync(int userId, string companyDB, int attachmentEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                var result = await connection.Request(EntitiesKeys.Attachments2, attachmentEntry).GetAsync<Attachment>();

                return new Response
                {
                    Status = "success",
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

        // DOWNLOAD ATTACHMENT
        public async Task<Response> GetAttachmentFileAsync(int userId, string companyDB, int attachmentEntry, string fileName) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                byte[] byteArray = await connection.GetAttachmentAsBytesAsync(attachmentEntry, fileName);

                return new Response
                {
                    Status = "success",
                    Payload = byteArray,
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

        // CREATE ATTACHMENT
        public async Task<Response> CreateAttachmentAsync(int userId, string companyDB, List<AttachmentParams> attachmentParams) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                //var result = await connection.Request(EntitiesKeys.Attachments2).GetAsync<Attachment>();
                //SLAttachment attachment = await connection.PostAttachmentAsync($@"{fileSource}", file);

                List<Response> responses = [];

                foreach (var attachment in attachmentParams)
                {
                    try
                    {
                        SLAttachment slAttachment = await connection.PostAttachmentAsync($@"{attachment.FileName}", attachment.File);
                        responses.Add(new Response
                        {
                            Status = "success",
                            Payload = slAttachment,
                            Id = attachment.Key
                        });
                    }
                    catch (Exception ex)
                    {
                        responses.Add(new Response
                        {
                            Status = "failed",
                            Message = ex.Message,
                            Id = attachment.Key
                        });
                    }
                }


                return new Response
                {
                    Status = "success",
                    Payload = responses
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

        // GET DOCUMENT LINES CHART OF ACCOUNTS
        public async Task<Response> GetDocumentLinesChartOfAccountsAsync(int userId, string companyDB, List<string> accountCodes) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string filter = "";
                for(int i = 0; i < accountCodes.Count; i++)
                {
                    filter += $"Code eq '{accountCodes[i]}'";
                    if (i < accountCodes.Count - 1) filter += " or ";
                }

                var result = await connection.Request(EntitiesKeys.ChartOfAccounts)
                    .Filter(filter += " and ActiveAccount eq 'Y' and FrozenFor eq 'N'")
                    .GetAllAsync<ChartOfAccount>();

                return new Response
                {
                    Status = "success",
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

        // GET DOCUMENT LINES BUSINESS PARTNERS
        public async Task<Response> GetDocumentLinesBusinessPartnersAsync(int userId, string companyDB, List<string> bpCodes) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string filter = "";
                for (int i = 0; i < bpCodes.Count; i++)
                {
                    filter += $"CardCode eq '{bpCodes[i]}'";
                    if (i < bpCodes.Count - 1) filter += " or ";
                }

                var result = await connection.Request(EntitiesKeys.BusinessPartners)
                    .Filter(filter += " and Frozen eq 'N'")
                    .GetAllAsync<BusinessPartner>();

                return new Response
                {
                    Status = "success",
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


        // GET WAREHOUSES
        public async Task<Response> GetWarehousesAsync(int userId, string companyDB, List<string> warehouseCodes) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string filter = "";
                for (int i = 0; i < warehouseCodes.Count; i++)
                {
                    filter += $"WarehouseCode eq '{warehouseCodes[i]}'";
                    if (i < warehouseCodes.Count - 1) filter += " or ";
                }

                var result = await connection.Request(EntitiesKeys.Warehouses)
                    .Filter(filter)
                    .GetAllAsync<Warehouse>();

                return new Response
                {
                    Status = "success",
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

        // GET ALL WAREHOUSES
        public async Task<Response> GetAllWarehousesAsync(int userId, string companyDB) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string filter = "Inactive eq 'N'";
                var result = await connection.Request(EntitiesKeys.Warehouses)
                    .Filter(filter)
                    .OrderBy("WarehouseCode asc")
                    .GetAllAsync<Warehouse>();

                return new Response
                {
                    Status = "success",
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

        // GET WITHOLDING TAX CODES
        public async Task<Response> GetWitholdingTaxCodesAsync(int userId, string companyDB, List<string> witholdingTaxCodes) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string filter = "";
                for (int i = 0; i < witholdingTaxCodes.Count; i++)
                {
                    filter += $"WTCode eq '{witholdingTaxCodes[i]}'";
                    if (i < witholdingTaxCodes.Count - 1) filter += " or ";
                }

                var result = await connection.Request(EntitiesKeys.WithholdingTaxCodes)
                    .Filter(filter)
                    .GetAllAsync<WitholdingTaxCode>();

                return new Response
                {
                    Status = "success",
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

        // GET PAGE DATA -- FPNLR
        public async Task<Response> GetPageDataAsync(int userId, string companyDB, int objType, char actionType, int docEntry, int series) => await Task.Run(async () =>
        {
            try
            {
                string reqParam = ObjectTypesHelper.GetObjectType(objType);
                string filter = $"Series eq {series}";

                var connection = Main.GetConnection(userId, companyDB);

                dynamic? payload = null;
                var result1 = await connection.Request(reqParam).Filter(filter).OrderBy("DocEntry asc").Top(1).GetAsync<List<dynamic>>(); //list
                var result2 = await connection.Request(reqParam).Filter(filter).OrderBy("DocEntry desc").Top(1).GetAsync<List<dynamic>>(); // list
                var firstData = result1.First();
                var lastData = result2.First();

                switch (actionType)
                {
                    case 'F': // First
                        payload = await connection.Request(reqParam, firstData.DocEntry).GetAsync();
                        break;
                    case 'P': // Prev
                        if (docEntry == (int)firstData.DocEntry || docEntry == 0) payload = lastData;
                        else payload = await connection.Request(reqParam, docEntry - 1).GetAsync();
                        break;
                    case 'N': // Next
                        if (docEntry == (int)lastData.DocEntry || docEntry == 0) payload = firstData;
                        else payload = await connection.Request(reqParam, docEntry + 1).GetAsync();
                        break;
                    case 'L': // Last
                        payload = await connection.Request(reqParam, lastData.DocEntry).GetAsync();
                        break;
                    case 'R': // Refresh
                        payload = await connection.Request(reqParam, docEntry).GetAsync();
                        break;
                    default:
                        return new Response
                        {
                            Status = "failed",
                            Message = "Invalid Action Type."
                        };
                }


                return new Response
                {
                    Status = "success",
                    Payload = payload
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

        // GET PAGE DATA MULTI -- FPNLR
        public async Task<Response> GetPageDataMultiAsync(int userId, string companyDB, int objType, char actionType, int docEntry, int series) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string reqParam = ObjectTypesHelper.GetObjectType(objType);

                string request = $"$crossjoin({reqParam}, {reqParam}/DocumentLines)"; // InventoryGenEntries, InventoryGenExits
                string expand = $"{reqParam}($select = DocEntry)"; //,{reqParam}/DocumentLines($select = BaseType, DocEntry, LineNum)
                string filter = $"{reqParam}/Series eq {series} and {reqParam}/DocEntry eq {reqParam}/DocumentLines/DocEntry and {reqParam}/DocumentLines/BaseType ne 202 and {reqParam}/DocumentLines/LineNum eq 0";

                dynamic? result1 = null;
                dynamic? result2 = null;

                if (objType == 59) // GR
                {
                    result1 = await connection.Request(request).Expand(expand).Filter(filter).OrderBy("DocEntry asc").Top(1).GetAsync<List<dynamic>>();
                    result2 = await connection.Request(request).Expand(expand).Filter(filter).OrderBy("DocEntry desc").Top(1).GetAsync<List<dynamic>>();
                } 
                else // GI
                {
                    result1 = await connection.Request(request).Expand(expand).Filter(filter).OrderBy("DocEntry asc").Top(1).GetAsync<List<dynamic>>();
                    result2 = await connection.Request(request).Expand(expand).Filter(filter).OrderBy("DocEntry desc").Top(1).GetAsync<List<dynamic>>();
                }

                int docEntry1 = result1.Count > 0 ? result1[0][reqParam]["DocEntry"] : 0;
                var firstData = await connection.Request(reqParam, docEntry1).GetAsync();
                int docEntry2 = result2[0][reqParam]["DocEntry"];
                var lastData = await connection.Request(reqParam, docEntry2).GetAsync();

                dynamic? payload = null;
                switch (actionType)
                {
                    case 'F': // First
                        payload = firstData;
                        break;
                    case 'P': // Prev
                        if (docEntry == (int)firstData.DocEntry || docEntry == 0) payload = lastData;
                        else
                        {
                            var result = await connection.Request(request).Expand(expand).Filter($"{filter} and {reqParam}/DocEntry lt {docEntry}").OrderBy("DocEntry desc").Top(1).GetAsync<List<dynamic>>();
                            int resDocEntry = result[0][reqParam]["DocEntry"];
                            var data = await connection.Request(reqParam, resDocEntry).GetAsync();
                            payload = data;
                        }
                        break;
                    case 'N': // Next
                        if (docEntry == (int)lastData.DocEntry || docEntry == 0) payload = firstData;
                        else
                        {
                            var result = await connection.Request(request).Expand(expand).Filter($"{filter} and {reqParam}/DocEntry gt {docEntry}").OrderBy("DocEntry asc").Top(1).GetAsync<List<dynamic>>();
                            int resDocEntry = result[0][reqParam]["DocEntry"];
                            var data = await connection.Request(reqParam, resDocEntry).GetAsync();
                            payload = data;
                        }
                        break;
                    case 'L': // Last
                        payload = lastData;
                        break;
                    case 'R': // Refresh
                        payload = await connection.Request(reqParam, docEntry).GetAsync();
                        break;
                    default:
                        return new Response
                        {
                            Status = "failed",
                            Message = "Invalid Action Type."
                        };
                }


                return new Response
                {
                    Status = "success",
                    Payload = payload
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

        // GET PAGE BP DATA -- FPNLR
        public async Task<Response> GetPageBPDataAsync(int userId, string companyDB, char actionType, string cardCode) => await Task.Run(async () =>
        {
            try
            {
                string reqParam = EntitiesKeys.BusinessPartners;

                var connection = Main.GetConnection(userId, companyDB);

                dynamic? payload = null;
                List<BusinessPartner> results = await connection.Request(reqParam).OrderBy("CardCode asc").GetAsync<List<BusinessPartner>>(); // list
                var firstData = results.First();
                var lastData = results.Last() ;

                switch (actionType)
                {
                    case 'F': // First
                        payload = firstData;
                        break;
                    case 'P': // Prev
                        if (cardCode == firstData.CardCode || cardCode == "") payload = lastData;
                        else
                        {
                            int index = results.FindIndex(r => r.CardCode == cardCode);
                            payload =  results[index - 1];
                        }
                        break;
                    case 'N': // Next
                        if (cardCode == lastData.CardCode || cardCode == "") payload = firstData;
                        else
                        {
                            int index = results.FindIndex(r => r.CardCode == cardCode);
                            payload = results[index + 1];
                        }
                        break;
                    case 'L': // Last
                        payload = lastData;
                        break;
                    case 'R': // Refresh
                        payload = await connection.Request(reqParam, cardCode).GetAsync<Item>();
                        break;
                    default:
                        return new Response
                        {
                            Status = "failed",
                            Message = "Invalid Action Type."
                        };
                }

                return new Response
                {
                    Status = "success",
                    Payload = payload
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

        // GET PAGE ITEM DATA -- FPNLR
        public async Task<Response> GetPageItemDataAsync(int userId, string companyDB, char actionType, string itemCode) => await Task.Run(async () =>
        {
            try
            {
                string reqParam = EntitiesKeys.Items;

                var connection = Main.GetConnection(userId, companyDB);

                dynamic? payload = null;
                List<Item> results = await connection.Request(reqParam).OrderBy("ItemCode asc").GetAsync<List<Item>>(); // list
                var firstData = results.First();
                var lastData = results.Last();

                switch (actionType)
                {
                    case 'F': // First
                        payload = firstData;
                        break;
                    case 'P': // Prev
                        if (itemCode == firstData.ItemCode || itemCode == "") payload = lastData;
                        else
                        {
                            int index = results.FindIndex(r => r.ItemCode == itemCode);
                            payload = results[index - 1];
                        }
                        break;
                    case 'N': // Next
                        if (itemCode == lastData.ItemCode || itemCode == "") payload = firstData;
                        else
                        {
                            int index = results.FindIndex(r => r.ItemCode == itemCode);
                            payload = results[index + 1];
                        }
                        break;
                    case 'L': // Last
                        payload = lastData;
                        break;
                    case 'R': // Refresh
                        payload = await connection.Request(reqParam, itemCode).GetAsync<Item>();
                        break;
                    default:
                        return new Response
                        {
                            Status = "failed",
                            Message = "Invalid Action Type."
                        };
                }

                return new Response
                {
                    Status = "success",
                    Payload = payload
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


        // GET DOCUMENT DATA
        public async Task<Response> GetDocumentDataAsync(int userId, string companyDB, int objType,  int docEntry) => await Task.Run(async () =>
        {
            try
            {
                string reqParam = ObjectTypesHelper.GetObjectType(objType);
                var connection = Main.GetConnection(userId, companyDB);

                var result = await connection.Request($"{reqParam}({docEntry})").GetAsync();

                dynamic? approvalRequest = null;
                if (objType == 112) // DRAFT
                {
                    var appReqs = await connection.Request(EntitiesKeys.ApprovalRequests).Filter($"DraftEntry eq {docEntry}").GetAsync<List<SLApprovalRequest>>();
                    approvalRequest = appReqs[0];
                }

                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        Document = result,
                        ApprovalRequest = approvalRequest
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


        // GET RELATIONSHIP MAP
        public async Task<Response> GetRelationshipMapAsync(int userId, string companyDB, int docEntry, int objType) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string reqParam = ObjectTypesHelper.GetObjectType(objType);

                List<BusinessPartnerMap> businessPartners = new();

                var result = await connection.Request(reqParam, docEntry).GetAsync<Document>();
                var mainDoc = await connection.Request(reqParam, docEntry).GetAsync<DocumentMap>();
                businessPartners.Add(new () { CardCode = result.CardCode, CardName = result.CardName });

                var docLines = result.DocumentLines.Select(x => new { x.BaseEntry, x.BaseType, x.TargetEntry, x.TargetType }).Distinct().ToList();

                List<DocumentMap> bases = new();
                foreach (var docLine in docLines)
                {
                    if (docLine.BaseType != -1 && docLine.BaseType != 0)
                    {
                        string lineResource = ObjectTypesHelper.GetObjectType(docLine.BaseType);
                        var baseDoc = await connection.Request(lineResource, docLine.BaseEntry).GetAsync<DocumentMap>();
                        businessPartners.Add(new()
                        {
                            CardCode = baseDoc.CardCode,
                            CardName = baseDoc.CardName
                        });
                        bases.Add(baseDoc);
                    }
                }

                List<DocumentMap> targets = new();
                foreach (var docLine in docLines)
                {
                    if (docLine.TargetType != -1 && docLine.TargetType != 0)
                    {
                        string lineResource = ObjectTypesHelper.GetObjectType(docLine.TargetType);
                        var targetDoc = await connection.Request(lineResource, docLine.TargetEntry).GetAsync<DocumentMap>();
                        businessPartners.Add(new()
                        {
                            CardCode = targetDoc.CardCode,
                            CardName = targetDoc.CardName
                        });
                        targets.Add(targetDoc);
                    }
                }

                RelationshipMap relationshipMap = new()
                {
                    BusinessPartners = businessPartners.DistinctBy(x => x.CardCode).ToList(),
                    Main = mainDoc,
                    Bases = bases,
                    Targets = targets
                };


                return new Response
                {
                    Status = "success",
                    Payload = relationshipMap
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

        // INIT
        // PURCHASING
        // GET INIT PURCHASE REQUEST
        public async Task<Response> GetInitPurchaseRequestAsync(int userId, string companyDB, InitParams initParams) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                // SERIES
                var series = await connection.Request($"{ActionsKeys.SeriesService}_GetDocumentSeries").OrderBy("Name").PostAsync<List<SLSeries>>(new { DocumentTypeParams = new { Document = 1470000113 } });
                // VAT GROUPS
                var vatGroups = await connection.Request(EntitiesKeys.VatGroups).Filter($"Inactive eq 'N' and Category eq 'I'").OrderBy("Name").GetAllAsync<VatGroup>();
                // ITEM GROUPS
                var itemGroups = await connection.Request(EntitiesKeys.ItemGroups).OrderBy("GroupName").GetAllAsync<ItemGroup>();
                // BRANCHES
                var branches = await connection.Request(EntitiesKeys.Branches).OrderBy("Name").GetAllAsync<Branch>();
                // DEPARTMENTS
                var departments = await connection.Request(EntitiesKeys.Departments).OrderBy("Name").GetAllAsync<Department>();
                // EMPLOYEES
                var employees = await connection.Request(EntitiesKeys.EmployeesInfo).OrderBy("EmployeeCode").Filter("Active eq 'Y'").GetAllAsync<EmployeeInfo>();
                // USER FIELDS
                List<UserFieldsMD> userFields = [];

                if (initParams.UserFieldParams.Count > 0)
                {
                    string userFieldsQueryFilter = "";
                    for (int i = 0; i < initParams.UserFieldParams.Count; i++)
                    {
                        userFieldsQueryFilter += $"(TableName eq '{initParams.UserFieldParams[i].TableName}' and FieldID eq {initParams.UserFieldParams[i].FieldID})";
                        if (i < initParams.UserFieldParams.Count - 1) userFieldsQueryFilter += " or ";
                    }
                    userFields = await connection.Request(EntitiesKeys.UserFieldsMD).Filter(userFieldsQueryFilter).GetAsync<List<UserFieldsMD>>();
                }
                
                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        Series = series,
                        VatGroups = vatGroups,
                        ItemGroups = itemGroups,
                        Branches = branches,
                        Departments = departments,
                        Employees = employees,
                        UserFields = userFields,
                    }
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

        // GET INIT PURCHASE ORDER
        public async Task<Response> GetInitPurchaseOrderAsync(int userId, string companyDB, InitParams initParams) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                // SERIES
                var series = await connection.Request($"{ActionsKeys.SeriesService}_GetDocumentSeries").OrderBy("Name").PostAsync<List<SLSeries>>(new { DocumentTypeParams = new { Document = 22 } });
                // PAYMENT TERMS
                var paymentTerms = await connection.Request(EntitiesKeys.PaymentTermsTypes).OrderBy("PaymentTermsGroupName").GetAllAsync<SLPaymentTermType>();
                // SALES PERSONS
                var salesPersons = await connection.Request(EntitiesKeys.SalesPersons).Filter("Active eq 'Y'").OrderBy("SalesEmployeeName").GetAllAsync<SalesPerson>();
                // SHIPPING TYPES
                var shippingTypes = await connection.Request(EntitiesKeys.ShippingTypes).OrderBy("Name").GetAllAsync<ShippingType>();
                // VAT GROUPS
                var vatGroups = await connection.Request(EntitiesKeys.VatGroups).Filter($"Inactive eq 'N' and Category eq 'I'").OrderBy("Name").GetAllAsync<VatGroup>();
                // ITEM GROUPS
                var itemGroups = await connection.Request(EntitiesKeys.ItemGroups).OrderBy("GroupName").GetAllAsync<ItemGroup>();
                // USER FIELDS
                string userFieldsQueryFilter = "";
                for (int i = 0; i < initParams.UserFieldParams.Count; i++)
                {
                    userFieldsQueryFilter += $"(TableName eq '{initParams.UserFieldParams[i].TableName}' and FieldID eq {initParams.UserFieldParams[i].FieldID})";
                    if (i < initParams.UserFieldParams.Count - 1) userFieldsQueryFilter += " or ";
                }
                var userFields = await connection.Request(EntitiesKeys.UserFieldsMD).Filter(userFieldsQueryFilter).GetAsync<List<UserFieldsMD>>();

                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        Series = series,
                        PaymentTerms = paymentTerms,
                        SalesPersons = salesPersons,
                        ShippingTypes = shippingTypes,
                        VatGroups = vatGroups,
                        ItemGroups = itemGroups,
                        UserFields = userFields,
                    }
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

        // GET INIT GOODS RECEIPT PO
        public async Task<Response> GetInitGoodsReceiptPOAsync(int userId, string companyDB, InitParams initParams) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                // SERIES
                var series = await connection.Request($"{ActionsKeys.SeriesService}_GetDocumentSeries").OrderBy("Series").PostAsync<List<SLSeries>>(new { DocumentTypeParams = new { Document = 20 } });
                // PAYMENT TERMS
                var paymentTerms = await connection.Request(EntitiesKeys.PaymentTermsTypes).OrderBy("PaymentTermsGroupName").GetAllAsync<SLPaymentTermType>();
                // SALES PERSONS
                var salesPersons = await connection.Request(EntitiesKeys.SalesPersons).Filter("Active eq 'Y'").OrderBy("SalesEmployeeName").GetAllAsync<SalesPerson>();
                // SHIPPING TYPES
                var shippingTypes = await connection.Request(EntitiesKeys.ShippingTypes).OrderBy("Name").GetAllAsync<ShippingType>();
                // VAT GROUPS
                var vatGroups = await connection.Request(EntitiesKeys.VatGroups).Filter($"Inactive eq 'N' and Category eq 'I'").OrderBy("Name").GetAllAsync<VatGroup>();
                // ITEM GROUPS
                var itemGroups = await connection.Request(EntitiesKeys.ItemGroups).OrderBy("GroupName").GetAllAsync<ItemGroup>();
                // USER FIELDS
                string userFieldsQueryFilter = "";
                for (int i = 0; i < initParams.UserFieldParams.Count; i++)
                {
                    userFieldsQueryFilter += $"(TableName eq '{initParams.UserFieldParams[i].TableName}' and FieldID eq {initParams.UserFieldParams[i].FieldID})";
                    if (i < initParams.UserFieldParams.Count - 1) userFieldsQueryFilter += " or ";
                }
                var userFields = await connection.Request(EntitiesKeys.UserFieldsMD).Filter(userFieldsQueryFilter).GetAsync<List<UserFieldsMD>>();

                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        Series = series,
                        PaymentTerms = paymentTerms,
                        SalesPersons = salesPersons,
                        ShippingTypes = shippingTypes,
                        VatGroups = vatGroups,
                        ItemGroups = itemGroups,
                        UserFields = userFields,
                    }
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
        // GET INIT GOODS RETURN
        public async Task<Response> GetInitPurchaseReturnAsync(int userId, string companyDB, InitParams initParams) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                // SERIES
                var series = await connection.Request($"{ActionsKeys.SeriesService}_GetDocumentSeries").OrderBy("Series").PostAsync<List<SLSeries>>(new { DocumentTypeParams = new { Document = 21 } });
                // PAYMENT TERMS
                var paymentTerms = await connection.Request(EntitiesKeys.PaymentTermsTypes).OrderBy("PaymentTermsGroupName").GetAllAsync<SLPaymentTermType>();
                // SALES PERSONS
                var salesPersons = await connection.Request(EntitiesKeys.SalesPersons).Filter("Active eq 'Y'").OrderBy("SalesEmployeeName").GetAllAsync<SalesPerson>();
                // SHIPPING TYPES
                var shippingTypes = await connection.Request(EntitiesKeys.ShippingTypes).OrderBy("Name").GetAllAsync<ShippingType>();
                // VAT GROUPS
                var vatGroups = await connection.Request(EntitiesKeys.VatGroups).Filter($"Inactive eq 'N' and Category eq 'I'").OrderBy("Name").GetAllAsync<VatGroup>();
                // ITEM GROUPS
                var itemGroups = await connection.Request(EntitiesKeys.ItemGroups).OrderBy("GroupName").GetAllAsync<ItemGroup>();
                // USER FIELDS
                //string userFieldsQueryFilter = "";
                //for (int i = 0; i < initParams.UserFieldParams.Count; i++)
                //{
                //    userFieldsQueryFilter += $"(TableName eq '{initParams.UserFieldParams[i].TableName}' and FieldID eq {initParams.UserFieldParams[i].FieldID})";
                //    if (i < initParams.UserFieldParams.Count - 1) userFieldsQueryFilter += " or ";
                //}
                //var userFields = await connection.Request(EntitiesKeys.UserFieldsMD).Filter(userFieldsQueryFilter).GetAsync<List<UserFieldsMD>>();

                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        Series = series,
                        PaymentTerms = paymentTerms,
                        SalesPersons = salesPersons,
                        ShippingTypes = shippingTypes,
                        VatGroups = vatGroups,
                        ItemGroups = itemGroups,
                        //UserFields = userFields,
                    }
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

        // GET INIT AP INVOICE
        public async Task<Response> GetInitAPInvoiceAsync(int userId, string companyDB, InitParams initParams) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                // SERIES
                var series = await connection.Request($"{ActionsKeys.SeriesService}_GetDocumentSeries").OrderBy("Series").PostAsync<List<SLSeries>>(new { DocumentTypeParams = new { Document = 18 } });
                // PAYMENT TERMS
                var paymentTerms = await connection.Request(EntitiesKeys.PaymentTermsTypes).OrderBy("PaymentTermsGroupName").GetAllAsync<SLPaymentTermType>();
                // SALES PERSONS
                var salesPersons = await connection.Request(EntitiesKeys.SalesPersons).Filter("Active eq 'Y'").OrderBy("SalesEmployeeName").GetAllAsync<SalesPerson>();
                // SHIPPING TYPES
                var shippingTypes = await connection.Request(EntitiesKeys.ShippingTypes).OrderBy("Name").GetAllAsync<ShippingType>();
                // VAT GROUPS
                var vatGroups = await connection.Request(EntitiesKeys.VatGroups).Filter($"Inactive eq 'N' and Category eq 'I'").OrderBy("Name").GetAllAsync<VatGroup>();
                // ITEM GROUPS
                var itemGroups = await connection.Request(EntitiesKeys.ItemGroups).OrderBy("GroupName").GetAllAsync<ItemGroup>();
                // USER FIELDS
                string userFieldsQueryFilter = "";
                for (int i = 0; i < initParams.UserFieldParams.Count; i++)
                {
                    userFieldsQueryFilter += $"(TableName eq '{initParams.UserFieldParams[i].TableName}' and FieldID eq {initParams.UserFieldParams[i].FieldID})";
                    if (i < initParams.UserFieldParams.Count - 1) userFieldsQueryFilter += " or ";
                }
                var userFields = await connection.Request(EntitiesKeys.UserFieldsMD).Filter(userFieldsQueryFilter).GetAsync<List<UserFieldsMD>>();

                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        Series = series,
                        PaymentTerms = paymentTerms,
                        SalesPersons = salesPersons,
                        ShippingTypes = shippingTypes,
                        VatGroups = vatGroups,
                        ItemGroups = itemGroups,
                        UserFields = userFields,
                    }
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

        // GET INIT AP CREDIT MEMO
        public async Task<Response> GetInitAPCreditMemoAsync(int userId, string companyDB, InitParams initParams) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                // SERIES
                var series = await connection.Request($"{ActionsKeys.SeriesService}_GetDocumentSeries").OrderBy("Series").PostAsync<List<SLSeries>>(new { DocumentTypeParams = new { Document = 19 } });
                // PAYMENT TERMS
                var paymentTerms = await connection.Request(EntitiesKeys.PaymentTermsTypes).OrderBy("PaymentTermsGroupName").GetAllAsync<SLPaymentTermType>();
                // SALES PERSONS
                var salesPersons = await connection.Request(EntitiesKeys.SalesPersons).Filter("Active eq 'Y'").OrderBy("SalesEmployeeName").GetAllAsync<SalesPerson>();
                // SHIPPING TYPES
                var shippingTypes = await connection.Request(EntitiesKeys.ShippingTypes).OrderBy("Name").GetAllAsync<ShippingType>();
                // VAT GROUPS
                var vatGroups = await connection.Request(EntitiesKeys.VatGroups).Filter($"Inactive eq 'N' and Category eq 'I'").OrderBy("Name").GetAllAsync<VatGroup>();
                // ITEM GROUPS
                var itemGroups = await connection.Request(EntitiesKeys.ItemGroups).OrderBy("GroupName").GetAllAsync<ItemGroup>();
                // USER FIELDS
                string userFieldsQueryFilter = "";
                for (int i = 0; i < initParams.UserFieldParams.Count; i++)
                {
                    userFieldsQueryFilter += $"(TableName eq '{initParams.UserFieldParams[i].TableName}' and FieldID eq {initParams.UserFieldParams[i].FieldID})";
                    if (i < initParams.UserFieldParams.Count - 1) userFieldsQueryFilter += " or ";
                }
                var userFields = await connection.Request(EntitiesKeys.UserFieldsMD).Filter(userFieldsQueryFilter).GetAsync<List<UserFieldsMD>>();

                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        Series = series,
                        PaymentTerms = paymentTerms,
                        SalesPersons = salesPersons,
                        ShippingTypes = shippingTypes,
                        VatGroups = vatGroups,
                        ItemGroups = itemGroups,
                        UserFields = userFields,
                    }
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

        // SALES
        // GET INIT SALES QUOTATION
        public async Task<Response> GetInitSalesQuotationAsync(int userId, string companyDB, InitParams initParams) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                mainDbContext.Database.GetDbConnection().ConnectionString = string.Format(connString, companyDB);

                // SERIES
                var series = await connection.Request($"{ActionsKeys.SeriesService}_GetDocumentSeries").OrderBy("Series").PostAsync<List<SLSeries>>(new { DocumentTypeParams = new { Document = 23 } });
                // PAYMENT TERMS
                var paymentTerms = await connection.Request(EntitiesKeys.PaymentTermsTypes).OrderBy("PaymentTermsGroupName").GetAllAsync<SLPaymentTermType>();
                // SALES PERSONS
                var salesPersons = await connection.Request(EntitiesKeys.SalesPersons).Filter("Active eq 'Y'").OrderBy("SalesEmployeeName").GetAllAsync<SalesPerson>();
                // SHIPPING TYPES
                var shippingTypes = await connection.Request(EntitiesKeys.ShippingTypes).OrderBy("Name").GetAllAsync<ShippingType>();
                // VAT GROUPS
                var vatGroups = await connection.Request(EntitiesKeys.VatGroups).Filter($"Inactive eq 'N' and Category eq 'O'").OrderBy("Name").GetAllAsync<VatGroup>();
                // ITEM GROUPS
                var itemGroups = await connection.Request(EntitiesKeys.ItemGroups).OrderBy("GroupName").GetAllAsync<ItemGroup>();
                // USER FIELDS
                string userFieldsQueryFilter = "";
                for (int i = 0; i < initParams.UserFieldParams.Count; i++)
                {
                    userFieldsQueryFilter += $"(TableName eq '{initParams.UserFieldParams[i].TableName}' and FieldID eq {initParams.UserFieldParams[i].FieldID})";
                    if (i < initParams.UserFieldParams.Count - 1) userFieldsQueryFilter += " or ";
                }
                var userFields = await connection.Request(EntitiesKeys.UserFieldsMD).Filter(userFieldsQueryFilter).GetAsync<List<UserFieldsMD>>();

                // USER TABLES
                //var warranty = mainDbContext.Warranty.Select(s => new UserTable
                //{
                //    Code = s.Code,
                //    Name = s.Name
                //}).ToList();

                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        Series = series,
                        PaymentTerms = paymentTerms,
                        SalesPersons = salesPersons,
                        ShippingTypes = shippingTypes,
                        VatGroups = vatGroups,
                        ItemGroups = itemGroups,
                        UserFields = userFields,
                        //UserTables = new
                        //{
                        //    //Warranty = warranty
                        //}
                    }
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

        // GET INIT SALES ORDER
        public async Task<Response> GetInitSalesOrderAsync(int userId, string companyDB, InitParams initParams) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                mainDbContext.Database.GetDbConnection().ConnectionString = string.Format(connString, companyDB);

                var series = await connection.Request($"{ActionsKeys.SeriesService}_GetDocumentSeries").OrderBy("Series").PostAsync<List<SLSeries>>(new { DocumentTypeParams = new { Document = 17 } });
                // PAYMENT TERMS
                var paymentTerms = await connection.Request(EntitiesKeys.PaymentTermsTypes).OrderBy("PaymentTermsGroupName").GetAllAsync<SLPaymentTermType>();
                // SALES PERSONS
                var salesPersons = await connection.Request(EntitiesKeys.SalesPersons).Filter("Active eq 'Y'").OrderBy("SalesEmployeeName").GetAllAsync<SalesPerson>();
                // SHIPPING TYPES
                var shippingTypes = await connection.Request(EntitiesKeys.ShippingTypes).OrderBy("Name").GetAllAsync<ShippingType>();
                // VAT GROUPS
                var vatGroups = await connection.Request(EntitiesKeys.VatGroups).Filter($"Inactive eq 'N' and Category eq 'O'").OrderBy("Name").GetAllAsync<VatGroup>();
                // ITEM GROUPS
                var itemGroups = await connection.Request(EntitiesKeys.ItemGroups).OrderBy("GroupName").GetAllAsync<ItemGroup>();
                // USER FIELDS
                string userFieldsQueryFilter = "";
                for (int i = 0; i < initParams.UserFieldParams.Count; i++)
                {
                    userFieldsQueryFilter += $"(TableName eq '{initParams.UserFieldParams[i].TableName}' and FieldID eq {initParams.UserFieldParams[i].FieldID})";
                    if (i < initParams.UserFieldParams.Count - 1) userFieldsQueryFilter += " or ";
                }
                var userFields = await connection.Request(EntitiesKeys.UserFieldsMD).Filter(userFieldsQueryFilter).GetAsync<List<UserFieldsMD>>();
                // USER TABLES
                //var warranty = mainDbContext.Warranty.Select(s => new UserTable
                //{
                //    Code = s.Code,
                //    Name = s.Name
                //}).ToList();

                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        Series = series,
                        PaymentTerms = paymentTerms,
                        SalesPersons = salesPersons,
                        ShippingTypes = shippingTypes,
                        VatGroups = vatGroups,
                        ItemGroups = itemGroups,
                        UserFields = userFields,
                        //UserTables = new
                        //{
                        //    Warranty = warranty
                        //}
                    }
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

        // GET INIT DELIVERY
        public async Task<Response> GetInitDeliveryAsync(int userId, string companyDB, InitParams initParams) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                // SERIES
                var series = await connection.Request($"{ActionsKeys.SeriesService}_GetDocumentSeries").OrderBy("Series").PostAsync<List<SLSeries>>(new { DocumentTypeParams = new { Document = 15 } });
                // PAYMENT TERMS
                var paymentTerms = await connection.Request(EntitiesKeys.PaymentTermsTypes).OrderBy("PaymentTermsGroupName").GetAllAsync<SLPaymentTermType>();
                // SALES PERSONS
                var salesPersons = await connection.Request(EntitiesKeys.SalesPersons).Filter("Active eq 'Y'").OrderBy("SalesEmployeeName").GetAllAsync<SalesPerson>();
                //
                var shippingTypes = await connection.Request(EntitiesKeys.ShippingTypes).OrderBy("Name").GetAllAsync<ShippingType>();
                // VAT GROUPS
                var vatGroups = await connection.Request(EntitiesKeys.VatGroups).Filter($"Inactive eq 'N' and Category eq 'O'").OrderBy("Name").GetAllAsync<VatGroup>();
                // ITEM GROUPS
                var itemGroups = await connection.Request(EntitiesKeys.ItemGroups).OrderBy("GroupName").GetAllAsync<ItemGroup>();
                // USER FIELDS
                string userFieldsQueryFilter = "";
                for (int i = 0; i < initParams.UserFieldParams.Count; i++)
                {
                    userFieldsQueryFilter += $"(TableName eq '{initParams.UserFieldParams[i].TableName}' and FieldID eq {initParams.UserFieldParams[i].FieldID})";
                    if (i < initParams.UserFieldParams.Count - 1) userFieldsQueryFilter += " or ";
                }
                var userFields = await connection.Request(EntitiesKeys.UserFieldsMD).Filter(userFieldsQueryFilter).GetAsync<List<UserFieldsMD>>();

                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        Series = series,
                        PaymentTerms = paymentTerms,
                        SalesPersons = salesPersons,
                        ShippingTypes = shippingTypes,
                        VatGroups = vatGroups,
                        ItemGroups = itemGroups,
                        UserFields = userFields,
                    }
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

        // GET INIT AR RETURN
        public async Task<Response> GetInitReturnAsync(int userId, string companyDB, InitParams initParams) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                // SERIES
                var series = await connection.Request($"{ActionsKeys.SeriesService}_GetDocumentSeries").OrderBy("Series").PostAsync<List<SLSeries>>(new { DocumentTypeParams = new { Document = 16 } });
                // PAYMENT TERMS
                var paymentTerms = await connection.Request(EntitiesKeys.PaymentTermsTypes).OrderBy("PaymentTermsGroupName").GetAllAsync<SLPaymentTermType>();
                // SALES PERSONS
                var salesPersons = await connection.Request(EntitiesKeys.SalesPersons).Filter("Active eq 'Y'").OrderBy("SalesEmployeeName").GetAllAsync<SalesPerson>();
                //
                var shippingTypes = await connection.Request(EntitiesKeys.ShippingTypes).OrderBy("Name").GetAllAsync<ShippingType>();
                // VAT GROUPS
                var vatGroups = await connection.Request(EntitiesKeys.VatGroups).Filter($"Inactive eq 'N' and Category eq 'O'").OrderBy("Name").GetAllAsync<VatGroup>();
                // ITEM GROUPS
                var itemGroups = await connection.Request(EntitiesKeys.ItemGroups).OrderBy("GroupName").GetAllAsync<ItemGroup>();
                // USER FIELDS
                //string userFieldsQueryFilter = "";
                //for (int i = 0; i < initParams.UserFieldParams.Count; i++)
                //{
                //    userFieldsQueryFilter += $"(TableName eq '{initParams.UserFieldParams[i].TableName}' and FieldID eq {initParams.UserFieldParams[i].FieldID})";
                //    if (i < initParams.UserFieldParams.Count - 1) userFieldsQueryFilter += " or ";
                //}
                //var userFields = await connection.Request(EntitiesKeys.UserFieldsMD).Filter(userFieldsQueryFilter).GetAsync<List<UserFieldsMD>>();

                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        Series = series,
                        PaymentTerms = paymentTerms,
                        SalesPersons = salesPersons,
                        ShippingTypes = shippingTypes,
                        VatGroups = vatGroups,
                        ItemGroups = itemGroups,
                        //UserFields = userFields,
                    }
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


        // GET INIT AR INVOICE
        public async Task<Response> GetInitARInvoiceAsync(int userId, string companyDB, InitParams initParams) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                // SERIES
                var series = await connection.Request($"{ActionsKeys.SeriesService}_GetDocumentSeries").OrderBy("Series").PostAsync<List<SLSeries>>(new { DocumentTypeParams = new { Document = 13 } });
                // PAYMENT TERMS
                var paymentTerms = await connection.Request(EntitiesKeys.PaymentTermsTypes).OrderBy("PaymentTermsGroupName").GetAllAsync<SLPaymentTermType>();
                // SALES PERSONS
                var salesPersons = await connection.Request(EntitiesKeys.SalesPersons).Filter("Active eq 'Y'").OrderBy("SalesEmployeeName").GetAllAsync<SalesPerson>();
                // SHIPPING TYPES
                var shippingTypes = await connection.Request(EntitiesKeys.ShippingTypes).OrderBy("Name").GetAllAsync<ShippingType>();
                // VAT GROUPS
                var vatGroups = await connection.Request(EntitiesKeys.VatGroups).Filter($"Inactive eq 'N' and Category eq 'O'").OrderBy("Name").GetAllAsync<VatGroup>();
                // ITEM GROUPS
                var itemGroups = await connection.Request(EntitiesKeys.ItemGroups).OrderBy("GroupName").GetAllAsync<ItemGroup>();
                // USER FIELDS
                string userFieldsQueryFilter = "";
                for (int i = 0; i < initParams.UserFieldParams.Count; i++)
                {
                    userFieldsQueryFilter += $"(TableName eq '{initParams.UserFieldParams[i].TableName}' and FieldID eq {initParams.UserFieldParams[i].FieldID})";
                    if (i < initParams.UserFieldParams.Count - 1) userFieldsQueryFilter += " or ";
                }
                var userFields = await connection.Request(EntitiesKeys.UserFieldsMD).Filter(userFieldsQueryFilter).GetAsync<List<UserFieldsMD>>();

                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        Series = series,
                        PaymentTerms = paymentTerms,
                        SalesPersons = salesPersons,
                        ShippingTypes = shippingTypes,
                        VatGroups = vatGroups,
                        ItemGroups = itemGroups,
                        UserFields = userFields,
                    }
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

        // GET INIT AR CREDIT MEMO
        public async Task<Response> GetInitARCreditMemoAsync(int userId, string companyDB, InitParams initParams) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                // SERIES
                var series = await connection.Request($"{ActionsKeys.SeriesService}_GetDocumentSeries").OrderBy("Series").PostAsync<List<SLSeries>>(new { DocumentTypeParams = new { Document = 14 } });
                // PAYMENT TERMS
                var paymentTerms = await connection.Request(EntitiesKeys.PaymentTermsTypes).OrderBy("PaymentTermsGroupName").GetAllAsync<SLPaymentTermType>();
                // SALES PERSONS
                var salesPersons = await connection.Request(EntitiesKeys.SalesPersons).Filter("Active eq 'Y'").OrderBy("SalesEmployeeName").GetAllAsync<SalesPerson>();
                // SHIPPING TYPES
                var shippingTypes = await connection.Request(EntitiesKeys.ShippingTypes).OrderBy("Name").GetAllAsync<ShippingType>();
                // VAT GROUPS
                var vatGroups = await connection.Request(EntitiesKeys.VatGroups).Filter($"Inactive eq 'N' and Category eq 'O'").OrderBy("Name").GetAllAsync<VatGroup>();
                // ITEM GROUPS
                var itemGroups = await connection.Request(EntitiesKeys.ItemGroups).OrderBy("GroupName").GetAllAsync<ItemGroup>();
                // USER FIELDS
                string userFieldsQueryFilter = "";
                for (int i = 0; i < initParams.UserFieldParams.Count; i++)
                {
                    userFieldsQueryFilter += $"(TableName eq '{initParams.UserFieldParams[i].TableName}' and FieldID eq {initParams.UserFieldParams[i].FieldID})";
                    if (i < initParams.UserFieldParams.Count - 1) userFieldsQueryFilter += " or ";
                }
                var userFields = await connection.Request(EntitiesKeys.UserFieldsMD).Filter(userFieldsQueryFilter).GetAsync<List<UserFieldsMD>>();

                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        Series = series,
                        PaymentTerms = paymentTerms,
                        SalesPersons = salesPersons,
                        ShippingTypes = shippingTypes,
                        VatGroups = vatGroups,
                        ItemGroups = itemGroups,
                        UserFields = userFields,
                    }
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

        // INVENTORY
        // GET INIT GOODS RECEIPT
        public async Task<Response> GetInitGoodsReceiptAsync(int userId, string companyDB, InitParams initParams) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                // SERIES
                var series = await connection.Request($"{ActionsKeys.SeriesService}_GetDocumentSeries").OrderBy("Name").PostAsync<List<SLSeries>>(new { DocumentTypeParams = new { Document = 59 } });
                // ITEM GROUPS
                var itemGroups = await connection.Request(EntitiesKeys.ItemGroups).OrderBy("GroupName").OrderBy("Name").GetAllAsync<ItemGroup>();
                // USER FIELDS
                string userFieldsQueryFilter = "";
                for (int i = 0; i < initParams.UserFieldParams.Count; i++)
                {
                    userFieldsQueryFilter += $"(TableName eq '{initParams.UserFieldParams[i].TableName}' and FieldID eq {initParams.UserFieldParams[i].FieldID})";
                    if (i < initParams.UserFieldParams.Count - 1) userFieldsQueryFilter += " or ";
                }
                var userFields = await connection.Request(EntitiesKeys.UserFieldsMD).Filter(userFieldsQueryFilter).GetAsync<List<UserFieldsMD>>();

                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        Series = series,
                        ItemGroups = itemGroups,
                        UserFields = userFields,
                    }
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

        // GET INIT GOODS ISSUE
        public async Task<Response> GetInitGoodsIssueAsync(int userId, string companyDB, InitParams initParams) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                // SERIES
                var series = await connection.Request($"{ActionsKeys.SeriesService}_GetDocumentSeries").OrderBy("Name").PostAsync<List<SLSeries>>(new { DocumentTypeParams = new { Document = 60 } });
                // ITEM GROUPS
                var itemGroups = await connection.Request(EntitiesKeys.ItemGroups).OrderBy("GroupName").GetAllAsync<ItemGroup>();
                // USER FIELDS
                List<UserFieldsMD> userFields = [];

                if (initParams.UserFieldParams.Count > 0)
                {
                    string userFieldsQueryFilter = "";
                    for (int i = 0; i < initParams.UserFieldParams.Count; i++)
                    {
                        userFieldsQueryFilter += $"(TableName eq '{initParams.UserFieldParams[i].TableName}' and FieldID eq {initParams.UserFieldParams[i].FieldID})";
                        if (i < initParams.UserFieldParams.Count - 1) userFieldsQueryFilter += " or ";
                    }
                    userFields = await connection.Request(EntitiesKeys.UserFieldsMD).Filter(userFieldsQueryFilter).GetAsync<List<UserFieldsMD>>();
                }


                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        Series = series,
                        ItemGroups = itemGroups,
                        UserFields = userFields,
                    }
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

        // GET INIT INVENTORY TRANSFER REQUEST
        public async Task<Response> GetInitInventoryTransferRequestAsync(int userId, string companyDB, InitParams initParams) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                // SERIES
                var series = await connection.Request($"{ActionsKeys.SeriesService}_GetDocumentSeries").OrderBy("Name").PostAsync<List<SLSeries>>(new { DocumentTypeParams = new { Document = 1250000001 } });
                // SALES PERSONS
                var salesPersons = await connection.Request(EntitiesKeys.SalesPersons).Filter("Active eq 'Y'").OrderBy("SalesEmployeeName").GetAllAsync<SalesPerson>();
                // ITEM GROUPS
                var itemGroups = await connection.Request(EntitiesKeys.ItemGroups).OrderBy("GroupName").GetAllAsync<ItemGroup>();
                // USER FIELDS
                List<UserFieldsMD> userFields = [];

                if (initParams.UserFieldParams.Count > 0)
                {
                    string userFieldsQueryFilter = "";
                    for (int i = 0; i < initParams.UserFieldParams.Count; i++)
                    {
                        userFieldsQueryFilter += $"(TableName eq '{initParams.UserFieldParams[i].TableName}' and FieldID eq {initParams.UserFieldParams[i].FieldID})";
                        if (i < initParams.UserFieldParams.Count - 1) userFieldsQueryFilter += " or ";
                    }
                    userFields = await connection.Request(EntitiesKeys.UserFieldsMD).Filter(userFieldsQueryFilter).GetAsync<List<UserFieldsMD>>();
                }

                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        Series = series,
                        //PaymentTerms = paymentTerms,
                        SalesPersons = salesPersons,
                        //ShippingTypes = shippingTypes,
                        //VatGroups = vatGroups,
                        ItemGroups = itemGroups,
                        UserFields = userFields,
                    }
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

        // GET INIT INVENTORY TRANSFER
        public async Task<Response> GetInitInventoryTransferAsync(int userId, string companyDB, InitParams initParams) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                // SERIES
                var series = await connection.Request($"{ActionsKeys.SeriesService}_GetDocumentSeries").OrderBy("Name").PostAsync<List<SLSeries>>(new { DocumentTypeParams = new { Document = 67 } });
                // SALES PERSONS
                var salesPersons = await connection.Request(EntitiesKeys.SalesPersons).Filter("Active eq 'Y'").OrderBy("SalesEmployeeName").GetAllAsync<SalesPerson>();
                // ITEM GROUPS
                var itemGroups = await connection.Request(EntitiesKeys.ItemGroups).OrderBy("GroupName").GetAllAsync<ItemGroup>();
                // USER FIELDS
                List<UserFieldsMD> userFields = [];

                if (initParams.UserFieldParams.Count > 0)
                {
                    string userFieldsQueryFilter = "";
                    for (int i = 0; i < initParams.UserFieldParams.Count; i++)
                    {
                        userFieldsQueryFilter += $"(TableName eq '{initParams.UserFieldParams[i].TableName}' and FieldID eq {initParams.UserFieldParams[i].FieldID})";
                        if (i < initParams.UserFieldParams.Count - 1) userFieldsQueryFilter += " or ";
                    }
                    userFields = await connection.Request(EntitiesKeys.UserFieldsMD).Filter(userFieldsQueryFilter).GetAsync<List<UserFieldsMD>>();
                }


                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        Series = series,
                        //PaymentTerms = paymentTerms,
                        SalesPersons = salesPersons,
                        //ShippingTypes = shippingTypes,
                        //VatGroups = vatGroups,
                        ItemGroups = itemGroups,
                        UserFields = userFields,
                    }
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


        // BANKING
        // GET INIT INCOMING PAYMENT
        public async Task<Response> GetInitIncomingPaymentAsync(int userId, string companyDB) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                // SERIES
                var series = await connection.Request($"{ActionsKeys.SeriesService}_GetDocumentSeries").OrderBy("Series").PostAsync<List<SLSeries>>(new { DocumentTypeParams = new { Document = 24 } });
                // CURRENCIES
                var currencies = await connection.Request($"{EntitiesKeys.Currencies}").OrderBy("Name").GetAllAsync<Currency>();
                // PROJECTS
                var projects = await connection.Request($"{EntitiesKeys.Projects}").OrderBy("Name").GetAllAsync<Project>();
                // COUNTRIES
                string request = $"$crossjoin({EntitiesKeys.Banks}, {EntitiesKeys.Countries})"; // InventoryGenEntries, InventoryGenExits
                string expand = $"{EntitiesKeys.Countries}($select = Code,Name)"; //{EntitiesKeys.Banks}($select = CountryCode),
                string filter = $"{EntitiesKeys.Banks}/CountryCode eq {EntitiesKeys.Countries}/Code";
                var countriesResult = await connection.Request(request).Expand(expand).Filter(filter).Apply($"groupby(({EntitiesKeys.Countries}/Code, {EntitiesKeys.Countries}/Name))").GetAllAsync<dynamic>();
                List<Country> countries = [];

                foreach (var item in countriesResult)
                {
                    countries.Add(new Country
                    {
                        Code = item[EntitiesKeys.Countries]["Code"],
                        Name = item[EntitiesKeys.Countries]["Name"]
                    });
                }
                // Banks
                var banks = await connection.Request(EntitiesKeys.Banks).OrderBy("BankName").GetAllAsync<Bank>();
                // CREDIT CARDS
                var creditCards = await connection.Request(EntitiesKeys.CreditCards).OrderBy("CreditCardName").GetAllAsync<CreditCard>();
                // CREDIT PAYMENT METHODS
                var creditPaymentMethods = await connection.Request(EntitiesKeys.CreditPaymentMethods).OrderBy("Name").GetAllAsync<CreditPaymentMethod>();
                // VAT GROUPS
                var vatGroups = await connection.Request(EntitiesKeys.VatGroups).Filter($"Inactive eq 'N' and Category eq 'O'").GetAllAsync<VatGroup>();


                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        Series = series,
                        Currencies = currencies,
                        Projects = projects,
                        Countries = countries,
                        Banks = banks,
                        CreditCards = creditCards,
                        CreditPaymentMethods = creditPaymentMethods,
                        VatGroups = vatGroups
                    }
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

        // GET INIT OUTGOING PAYMENT
        public async Task<Response> GetInitOutgoingPaymentAsync(int userId, string companyDB, InitParams initParams) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                // SERIES
                var series = await connection.Request($"{ActionsKeys.SeriesService}_GetDocumentSeries").OrderBy("Series").PostAsync<List<SLSeries>>(new { DocumentTypeParams = new { Document = 46 } });
                // CURRENCIES
                var currencies = await connection.Request($"{EntitiesKeys.Currencies}").OrderBy("Name").GetAllAsync<Currency>();
                // PROJECTS
                var projects = await connection.Request($"{EntitiesKeys.Projects}").OrderBy("Name").GetAllAsync<Project>();
                // COUNTRIES
                string request = $"$crossjoin({EntitiesKeys.Banks}, {EntitiesKeys.Countries})"; // InventoryGenEntries, InventoryGenExits
                string expand = $"{EntitiesKeys.Countries}($select = Code,Name)"; //{EntitiesKeys.Banks}($select = CountryCode),
                string filter = $"{EntitiesKeys.Banks}/CountryCode eq {EntitiesKeys.Countries}/Code";
                var countriesResult = await connection.Request(request).Expand(expand).Filter(filter).Apply($"groupby(({EntitiesKeys.Countries}/Code, {EntitiesKeys.Countries}/Name))").GetAllAsync<dynamic>();
                List<Country> countries = [];

                foreach (var item in countriesResult)
                {
                    countries.Add(new Country
                    {
                        Code = item[EntitiesKeys.Countries]["Code"],
                        Name = item[EntitiesKeys.Countries]["Name"]
                    });
                }
                // Banks
                var banks = await connection.Request(EntitiesKeys.Banks).OrderBy("BankName").GetAllAsync<Bank>();
                // House Banks
                var houseBankAccounts = await connection.Request(EntitiesKeys.HouseBankAccounts).OrderBy("BankCode").GetAllAsync<HouseBankAccount>();
                // CREDIT CARDS
                var creditCards = await connection.Request(EntitiesKeys.CreditCards).OrderBy("CreditCardName").GetAllAsync<CreditCard>();
                // CREDIT PAYMENT METHODS
                var creditPaymentMethods = await connection.Request(EntitiesKeys.CreditPaymentMethods).OrderBy("Name").GetAllAsync<CreditPaymentMethod>();
                // VAT GROUPS
                var vatGroups = await connection.Request(EntitiesKeys.VatGroups).Filter($"Inactive eq 'N' and Category eq 'O'").GetAllAsync<VatGroup>();
                // USER FIELDS
                List<UserFieldsMD> userFields = [];

                if (initParams.UserFieldParams.Count > 0)
                {
                    string userFieldsQueryFilter = "";
                    for (int i = 0; i < initParams.UserFieldParams.Count; i++)
                    {
                        userFieldsQueryFilter += $"(TableName eq '{initParams.UserFieldParams[i].TableName}' and FieldID eq {initParams.UserFieldParams[i].FieldID})";
                        if (i < initParams.UserFieldParams.Count - 1) userFieldsQueryFilter += " or ";
                    }
                    userFields = await connection.Request(EntitiesKeys.UserFieldsMD).Filter(userFieldsQueryFilter).GetAsync<List<UserFieldsMD>>();
                }

                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        Series = series,
                        Currencies = currencies,
                        Projects = projects,
                        Countries = countries,
                        Banks = banks,
                        CreditCards = creditCards,
                        CreditPaymentMethods = creditPaymentMethods,
                        VatGroups = vatGroups,
                        HouseBankAccounts = houseBankAccounts,
                        UserFields = userFields
                    }
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

    }
}
