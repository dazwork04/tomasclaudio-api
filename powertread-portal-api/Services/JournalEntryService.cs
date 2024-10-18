using Newtonsoft.Json;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Models;
using SLayerConnectionLib;
using B1SLayer;
using SAPB1SLayerWebAPI.Utils;

namespace SAPB1SLayerWebAPI.Services
{
    public class JournalEntryService
    {
        // GET JOURNAL ENTRIES
        public async Task<Response> GetJournalEntriesAsync(int userId, string companyDB, string dateFrom, string dateTo, Paginate paginate) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                string orderBy = paginate.OrderBy[0].ToString().ToUpper() + paginate.OrderBy[1..];
                //string queryFilter = $"DocumentStatus eq '{status}' and Cancelled eq '{cancelled}' and DocDate ge '{dateFrom}' and DocDate le '{dateTo}'" + paginate.Filter;
                string queryFilter = $"ReferenceDate ge '{dateFrom}' and ReferenceDate le '{dateTo}'" + paginate.Filter;


                var count = await connection.Request(EntitiesKeys.JournalEntries)
                    .Filter(queryFilter)
                    .GetCountAsync();

                var result = await connection.Request(EntitiesKeys.JournalEntries)
                    .Filter(queryFilter)
                    .Skip(paginate.Page * paginate.Size)
                    .Top(paginate.Size)
                    .OrderBy($"{orderBy} {paginate.Direction}")
                    .GetAsync<List<JournalEntry>>();

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

        // CREATE JOURNAL ENTRY
        public async Task<Response> CreateJournalEntryAsync(int userId, string companyDB, JournalEntry journalEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                var result = await connection.Request(EntitiesKeys.JournalEntries).PostAsync<JournalEntry>(journalEntry);
                Logger.CreateLog(false, "CREATE JOURNAL ENTRY", "SUCCESS", JsonConvert.SerializeObject(journalEntry));

                return new Response
                {
                    Status = "success",
                    Message = $"JE #{result.Number} CREATED successfully!",
                    Payload = result
                };
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CREATE JOURNAL ENTRY", ex.Message, JsonConvert.SerializeObject(journalEntry));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // UPDATE JOURNAL ENTRY
        public async Task<Response> UpdateJournalEntryAsync(int userId, string companyDB, JournalEntry journalEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                await connection.Request(EntitiesKeys.JournalEntries, journalEntry.JdtNum).PatchAsync(journalEntry);

                var result = await connection.Request(EntitiesKeys.JournalEntries, journalEntry.JdtNum).GetAsync<JournalEntry>();
                Logger.CreateLog(false, "UPDATE JOURNAL ENTRY", "SUCCESS", JsonConvert.SerializeObject(journalEntry));

                return new Response
                {
                    Status = "success",
                    Message = $"JE #{result.Number} UPDATED successfully!",
                    Payload = result
                };
            }
            catch (Exception ex)
            {

                Logger.CreateLog(true, "UPDATE JOURNAL ENTRY", ex.Message, JsonConvert.SerializeObject(journalEntry));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });

        // CREATE JOURNAL VOUCHER
        public async Task<Response> CreateJournalVoucherAsync(int userId, string companyDB, JournalEntry journalVoucher) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);

                var result = await connection.Request($"{ActionsKeys.JournalVouchersService}_Add").PostAsync<JournalEntry>(journalVoucher);
                Logger.CreateLog(false, "CREATE JOURNAL VOUCHER", "SUCCESS", JsonConvert.SerializeObject(journalVoucher));

                return new Response
                {
                    Status = "success",
                    Message = $"JV #{result.Number} CREATED successfully!",
                    Payload = result
                };
            }
            catch (Exception ex)
            {
                Logger.CreateLog(true, "CREATE JOURNAL VOUCHER", ex.Message, JsonConvert.SerializeObject(journalVoucher));
                return new Response
                {
                    Status = "failed",
                    Message = ex.Message
                };
            }
        });


        // GET JOURNAL ENTRY
        public async Task<Response> GetJournalEntryAsync(int userId, string companyDB, int docEntry) => await Task.Run(async () =>
        {
            try
            {
                var connection = Main.GetConnection(userId, companyDB);
                var result = await connection.Request(EntitiesKeys.JournalEntries, docEntry).GetAsync<JournalEntry>();

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
    }
}
