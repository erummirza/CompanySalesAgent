using System.Text.Json.Serialization; // Add this

namespace CompanySalesAgent.Api.Models
{
    public class Interaction
    {
        public int Id { get; set; }
        public int? LeadId { get; set; } // Made nullable earlier

        [JsonIgnore] // <--- ADD THIS HERE
        public Lead? Lead { get; set; }

        public string Message { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}