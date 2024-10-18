using B1SLayer;
using Microsoft.EntityFrameworkCore;
using SAPB1SLayerWebAPI.Context;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Utils;
using SLayerConnectionLib;

namespace SAPB1SLayerWebAPI.Services
{
    public class PaginateService
    {

        private readonly MainDbContext mainDbContext;
        private readonly string connString;

        public PaginateService(MainDbContext mainDbContext)
        {
            this.mainDbContext = mainDbContext;
            connString = mainDbContext.Database.GetDbConnection().ConnectionString;
        }

        // GET BUSINESS PARTNERS PAGINATE
        public async Task<Response> GetBusinessPartnersAsync(int userId, string companyDB, string cardType, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                string queryFilter = $"CardType eq '{cardType}' and Frozen eq 'N'" + paginate.Filter;

                var count = await connection.Request(EntitiesKeys.BusinessPartners)
                    .Filter(queryFilter)
                    .GetCountAsync();

                var result = await connection.Request(EntitiesKeys.BusinessPartners)
                    .Filter(queryFilter)
                    .Skip(paginate.Page * paginate.Size)
                    .Top(paginate.Size)
                    .OrderBy($"{orderBy} {paginate.Direction}")
                    .GetAsync<List<BusinessPartner>>();

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
                    Message = ex.Message
                };
            }
        });

        // GET ITEMS PAGINATE
        public async Task<Response> GetItemsAsync(int userId, string companyDB, int itemsGroupCode, char type, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                string queryFilter = $"{(type == 'S' ? "SalesItem" : type == 'P' ? "PurchaseItem" : "InventoryItem")} eq 'Y'" + paginate.Filter;

                if (itemsGroupCode != 0) queryFilter = $"ItemsGroupCode  eq {itemsGroupCode} and " + queryFilter;

                var count = await connection.Request(EntitiesKeys.Items)
                    .Filter(queryFilter)
                    .GetCountAsync();

                var result = await connection.Request(EntitiesKeys.Items)
                    .Filter(queryFilter)
                    .Skip(paginate.Page * paginate.Size)
                    .Top(paginate.Size)
                    .OrderBy($"{orderBy} {paginate.Direction}")
                    .GetAsync<List<Item>>();

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
                    Message = ex.Message
                };
            }
        });

        // GET CHART OF ACCOUNTS PAGINATE
        public async Task<Response> GetChartOfAccountsAsync(int userId, string companyDB, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                string queryFilter = "ActiveAccount eq 'Y' and FrozenFor eq 'N'" + paginate.Filter;

                var count = await connection.Request(EntitiesKeys.ChartOfAccounts)
                    .Filter(queryFilter)
                    .GetCountAsync();

                var result = await connection.Request(EntitiesKeys.ChartOfAccounts)
                    .Filter(queryFilter)
                    .Skip(paginate.Page * paginate.Size)
                    .Top(paginate.Size)
                    .OrderBy($"{orderBy} {paginate.Direction}")
                    .GetAsync<List<ChartOfAccount>>();

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
                    Message = ex.Message
                };
            }
        });

        // GET WAREHOUSES PAGINATE
        public async Task<Response> GetWarehousesAsync(int userId, string companyDB, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                string queryFilter = "Inactive eq 'N'" + paginate.Filter;

                var count = await connection.Request(EntitiesKeys.Warehouses)
                    .Filter(queryFilter)
                    .GetCountAsync();

                var result = await connection.Request(EntitiesKeys.Warehouses)
                    .Filter(queryFilter)
                    .Skip(paginate.Page * paginate.Size)
                    .Top(paginate.Size)
                    .OrderBy($"{orderBy} {paginate.Direction}")
                    .GetAsync<List<Warehouse>>();

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
                    Message = ex.Message
                };
            }
        });

        // GET COSTING CODE PAGINATE
        public async Task<Response> GetCostingCodesAsync(int userId, string companyDB, int dimCode, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                string queryFilter = $"InWhichDimension eq {dimCode} and Active eq 'Y' " + paginate.Filter;

                var count = await connection.Request(EntitiesKeys.ProfitCenters)
                    .Filter(queryFilter)
                    .GetCountAsync();

                var result = await connection.Request(EntitiesKeys.ProfitCenters)
                    .Filter(queryFilter)
                    .Skip(paginate.Page * paginate.Size)
                    .Top(paginate.Size)
                    .OrderBy($"{orderBy} {paginate.Direction}")
                    .GetAsync<List<CostCenter>>();

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
                    Message = ex.Message
                };
            }
        });

        // GET BUSINESS PARTNERS PAGINATE
        public async Task<Response> GetBusinessPartnersAsync1(int userId, string companyDB, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                string queryFilter = $"Frozen eq 'N'" + paginate.Filter;
                var count = await connection.Request(EntitiesKeys.BusinessPartners)
                    .Filter(queryFilter)
                    .GetCountAsync();
                var result = await connection.Request(EntitiesKeys.BusinessPartners)
                    .Filter(queryFilter)
                    .Skip(paginate.Page * paginate.Size)
                    .Top(paginate.Size)
                    .OrderBy($"{orderBy} {paginate.Direction}")
                    .GetAsync<List<BusinessPartner>>();
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
                    Message = ex.Message
                };
            }
        });

        // GET PAYMENT DRAFTS
        public async Task<Response> GetPaymentDraftsAsync(int userId, string companyDB, string dateFrom, string dateTo, int objectType, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                mainDbContext.Database.GetDbConnection().ConnectionString = string.Format(connString, companyDB);

                // GET DRAFT ENTRIES FROM GIVEN DATE RANGE 
                var draftEntriesResult = mainDbContext.OPDF.Where(o => (o.DocDate >= DateTime.Parse(dateFrom) && o.DocDate <= DateTime.Parse(dateTo)) && o.ObjType == objectType).Select(o => o.DocEntry).ToList();
                // GET DRAFT ENTRIES WITH NO EXISTING DOCUMENT
                List<int?> draftKeys = [];
                if (objectType == 24) draftKeys = [.. mainDbContext.ORCT.Where(o1 => o1.DraftKey != null).Select(o1 => o1.DraftKey)];
                else draftKeys = [.. mainDbContext.OVPM.Where(o1 => o1.DraftKey != null).Select(o1 => o1.DraftKey)];

                draftEntriesResult = draftEntriesResult.Where(o => !draftKeys.Contains(o)).ToList();

                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                //string queryFilter = $"DocObjectCode eq '{objectType}' and DocDate ge '{dateFrom}' and DocDate le '{dateTo}'" + paginate.Filter;


                //var count = await connection.Request(EntitiesKeys.PaymentDrafts)
                //    .Filter(queryFilter)
                //    .GetCountAsync();

                //var results = await connection.Request(EntitiesKeys.PaymentDrafts)
                //    .Filter(queryFilter)
                //    .Skip(paginate.Page * paginate.Size)
                //    .Top(paginate.Size)
                //    .OrderBy($"{orderBy} {paginate.Direction}")
                //    .GetAsync<List<dynamic>>();

                List<dynamic> resultDrafts = [];
                long countDrafts = draftEntriesResult.Count;
                long count = 0;

                if (countDrafts > 0)
                {
                    //draftEntriesResult = draftEntriesResult.Skip(paginate.Page * paginate.Size).Take(paginate.Size).ToList();

                    string queryFilter = "";
                    for (int i = 0; i < draftEntriesResult.Count; i++)
                    {
                        queryFilter += $"DocEntry eq {draftEntriesResult[i]}";
                        if (i < draftEntriesResult.Count - 1) queryFilter += " or ";
                    }

                    if (queryFilter != string.Empty) queryFilter = $"({queryFilter})";

                    count = await connection.Request(EntitiesKeys.PaymentDrafts)
                   .Filter(queryFilter + paginate.Filter)
                   .GetCountAsync();

                    resultDrafts = (List<dynamic>)await connection.Request(EntitiesKeys.PaymentDrafts)
                    .Filter(queryFilter + paginate.Filter)
                    .Skip(paginate.Page * paginate.Size)
                    .Top(paginate.Size)
                    .OrderBy($"{orderBy} {paginate.Direction}")
                    .GetAllAsync<dynamic>();
                }


                return new Response
                {
                    Status = "success",
                    Payload = new
                    {
                        Count = count,
                        Data = resultDrafts
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

        // GET REFERENCES (AR)
        public async Task<Response> GetJOReferencesAsync(int userId, string companyDB, int objType, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];

                string reqParam = ObjectTypesHelper.GetObjectType(objType);

                string queryFilter = $"DocumentStatus eq 'O'" + paginate.Filter;
                var count = await connection.Request(reqParam)
                    .Filter(queryFilter)
                    .GetCountAsync();

                if (count == 0)
                {
                    reqParam = ObjectTypesHelper.GetObjectType(objType == 15 ? 17 : 15);
                    count = await connection.Request(reqParam)
                   .Filter(queryFilter)
                   .GetCountAsync();
                } 

                var result = await connection.Request(reqParam)
                    .Filter(queryFilter)
                    .Skip(paginate.Page * paginate.Size)
                    .Top(paginate.Size)
                    .OrderBy($"{orderBy} {paginate.Direction}")
                    .GetAsync<List<dynamic>>();
                
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
                    Message = ex.Message
                };
            }
        });
    }
}
