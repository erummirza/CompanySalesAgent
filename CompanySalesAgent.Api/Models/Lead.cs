using Microsoft.VisualBasic;

namespace CompanySalesAgent.Api.Models
{
    public class Lead
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Status { get; set; } = "New"; // New, Contacted, Qualified, Won, Lost
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public List<Interaction> Interactions { get; set; } = new();
    }
}