using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SAPB1SLayerWebAPI.Models;
using SAPB1SLayerWebAPI.Models.SLayer;
using SAPB1SLayerWebAPI.Services;

namespace SAPB1SLayerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JournalEntryController : ControllerBase
    {
        private readonly JournalEntryService jeService;
        public JournalEntryController() => jeService = new();

        // GET JOURNAL ENTRIES
        [HttpPost("GetJournalEntries/{userId}/{companyDB}/{dateFrom}/{dateTo}")]
        public async Task<IActionResult> GetJournalEntries(int userId, string companyDB, string dateFrom, string dateTo, Paginate paginate) =>
            Ok(await jeService.GetJournalEntriesAsync(userId, companyDB, dateFrom, dateTo, paginate));

        // CREATE JOURNAL ENTRY
        [HttpPost("CreateJournalEntry/{userId}/{companyDB}")]
        public async Task<IActionResult> CreateJournalEntry(int userId, string companyDB, JournalEntry journalEntry) => Ok(await jeService.CreateJournalEntryAsync(userId, companyDB, journalEntry));

        // UPDATE JOURNAL ENTRY
        [HttpPost("UpdateJournalEntry/{userId}/{companyDB}")]
        public async Task<IActionResult> UpdateJournalEntry(int userId, string companyDB, JournalEntry journalEntry) => Ok(await jeService.UpdateJournalEntryAsync(userId, companyDB, journalEntry));

        // CREATE JOURNAL VOUCHER
        [HttpPost("CreateJournalVoucher/{userId}/{companyDB}")]
        public async Task<IActionResult> CreateJournalVoucher(int userId, string companyDB, JournalEntry journalVoucher) => Ok(await jeService.CreateJournalEntryAsync(userId, companyDB, journalVoucher));

        // GET JOURNAL ENTRY
        [HttpGet("GetJournalEntry/{userId}/{companyDB}/{jdtNum}")]
        public async Task<IActionResult> GetJournalEntry(int userId, string companyDB, int jdtNum) =>
           Ok(await jeService.GetJournalEntryAsync(userId, companyDB, jdtNum));
    }
}
