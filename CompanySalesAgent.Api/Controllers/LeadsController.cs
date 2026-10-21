using CompanySalesAgent.Api.Data;
using CompanySalesAgent.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Required for ToListAsync and Include

namespace CompanySalesAgent.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeadsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public LeadsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET: api/leads
        [HttpGet]
        public async Task<IActionResult> GetAllLeads()
        {
            // The explicit <Lead> fixes the "type arguments cannot be inferred" error
            var leads = await _dbContext.Leads.ToListAsync<Lead>();
            return Ok(leads);
        }

        // POST: api/leads
        [HttpPost]
        public async Task<IActionResult> CreateLead([FromBody] Lead newLead)
        {
            if (string.IsNullOrWhiteSpace(newLead.Name))
            {
                return BadRequest("Lead name is required.");
            }

            _dbContext.Leads.Add(newLead);
            await _dbContext.SaveChangesAsync();

            return Ok(newLead);
        }

        // GET: api/leads/5/interactions
        [HttpGet("{id}/interactions")]
        public async Task<IActionResult> GetLeadInteractions(int id)
        {
            var lead = await _dbContext.Leads
                .Include(l => l.Interactions) // Pulls the chat history for this lead
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lead == null)
            {
                return NotFound("Lead not found.");
            }

            return Ok(lead);
        }
    }
}