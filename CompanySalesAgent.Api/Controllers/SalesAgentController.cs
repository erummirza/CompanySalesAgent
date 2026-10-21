using System.Text.Json;
using CompanySalesAgent.Api.Data;
using CompanySalesAgent.Api.Models;
using CompanySalesAgent.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CompanySalesAgent.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesAgentController : ControllerBase
    {
        private readonly OllamaService _ollamaService;
        private readonly AppDbContext _dbContext; // 1. Inject the Database

        public SalesAgentController(OllamaService ollamaService, AppDbContext dbContext)
        {
            _ollamaService = ollamaService;
            _dbContext = dbContext;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] CustomerMessage customerMessage)
        {
            if (string.IsNullOrWhiteSpace(customerMessage.Message))
            {
                return BadRequest("Please enter a customer message.");
            }

            // 2. Get the AI response
            var answer = await _ollamaService.GetResponseAsync(customerMessage.Message);

            // 3. Save to the database!
            // (For now, we will just save it without a specific LeadId, 
            // or you can create a dummy Lead first)

            var interaction = new Interaction
            {
                // LeadId = 1, // Uncomment this if you already have a Lead in the DB
                Message = customerMessage.Message,
                Response = answer,
                Timestamp = DateTime.UtcNow
            };

            _dbContext.Interactions.Add(interaction);
            await _dbContext.SaveChangesAsync();

            return Ok(new
            {
                customerMessage = customerMessage.Message,
                response = answer,
                savedToDatabase = true
            });
        }

        [HttpPost("extract-lead")]
        public async Task<IActionResult> ExtractLead([FromBody] CustomerMessage transcriptMessage)
        {
            if (string.IsNullOrWhiteSpace(transcriptMessage.Message))
            {
                return BadRequest("Transcript cannot be empty.");
            }

            // 1. Ask the AI to extract the data
            var jsonResponse = await _ollamaService.ExtractLeadDataAsync(transcriptMessage.Message);

            try
            {
                // 2. Parse the JSON the AI returned (clean it up just in case)
                jsonResponse = jsonResponse.Replace("```json", "").Replace("```", "").Trim();

                using var document = JsonDocument.Parse(jsonResponse);
                var root = document.RootElement;

                var newLead = new Lead
                {
                    Name = root.GetProperty("name").GetString() ?? "Unknown",
                    Email = root.GetProperty("email").GetString() ?? "Unknown",
                    Status = "New"
                };

                // 3. Save to database
                _dbContext.Leads.Add(newLead);

                // 4. Save the transcript as the first interaction for this lead
                var interaction = new Interaction
                {
                    Lead = newLead, // Link to the new lead
                    Message = transcriptMessage.Message,
                    Response = $"Lead automatically created. Intent: {root.GetProperty("intent").GetString()}",
                    Timestamp = DateTime.UtcNow
                };
                _dbContext.Interactions.Add(interaction);

                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    message = "Lead extracted and saved successfully!",
                    lead = newLead,
                    rawAiOutput = jsonResponse
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"AI failed to return valid JSON. Error: {ex.Message}. Raw output: {jsonResponse}");
            }
        }
    }
}