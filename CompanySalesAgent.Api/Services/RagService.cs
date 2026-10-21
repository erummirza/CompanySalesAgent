
using System.Net.Http.Json;
using System.Text.Json;

namespace CompanySalesAgent.Api.Services
{
    public class RagService
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment _environment;

        public RagService(
            HttpClient httpClient,
            IWebHostEnvironment environment)
        {
            _httpClient = httpClient;
            _environment = environment;
        }

        public async Task<List<string>> RetrieveRelevantChunksAsync(
            string question)
        {
            var filePath = Path.Combine(
                _environment.ContentRootPath,
                "Knowledge",
                "company-info.txt");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(
                    "Company knowledge file was not found.",
                    filePath);
            }

            var document = await File.ReadAllTextAsync(filePath);

            var chunks = document
                .Split(
                    new[] { "\r\n\r\n", "\n\n" },
                    StringSplitOptions.RemoveEmptyEntries)
                .Select(chunk => chunk.Trim())
                .Where(chunk => !string.IsNullOrWhiteSpace(chunk))
                .ToList();

            if (chunks.Count == 0)
            {
                return new List<string>();
            }

            // Generate an embedding for the customer's question.
            var questionEmbedding = await GetEmbeddingAsync(question);

            // Generate embeddings for company knowledge chunks.
            var rankedChunks = new List<(string Text, double Score)>();

            foreach (var chunk in chunks)
            {
                var chunkEmbedding = await GetEmbeddingAsync(chunk);

                var similarity = CalculateCosineSimilarity(
                    questionEmbedding,
                    chunkEmbedding);

                rankedChunks.Add((chunk, similarity));
            }

            // Return the three chunks most similar in meaning.
            const double similarityThreshold = 0.45;

            return rankedChunks
                .Where(item => item.Score >= similarityThreshold)
                .OrderByDescending(item => item.Score)
                .Take(3)
                .Select(item => item.Text)
                .ToList();
        }

        private async Task<List<double>> GetEmbeddingAsync(string text)
        {
            var request = new
            {
                model = "nomic-embed-text",
                input = text
            };

            var response = await _httpClient.PostAsJsonAsync(
                "api/embed",
                request);

            response.EnsureSuccessStatusCode();

            using var result = await response.Content
                .ReadFromJsonAsync<JsonDocument>();

            if (result == null ||
                !result.RootElement.TryGetProperty(
                    "embeddings", out var embeddings) ||
                embeddings.GetArrayLength() == 0)
            {
                throw new InvalidOperationException(
                    "Ollama did not return an embedding.");
            }

            return embeddings[0]
                .EnumerateArray()
                .Select(value => value.GetDouble())
                .ToList();
        }

        private static double CalculateCosineSimilarity(
            List<double> vectorA,
            List<double> vectorB)
        {
            if (vectorA.Count != vectorB.Count || vectorA.Count == 0)
            {
                return 0;
            }

            double dotProduct = 0;
            double magnitudeA = 0;
            double magnitudeB = 0;

            for (int i = 0; i < vectorA.Count; i++)
            {
                dotProduct += vectorA[i] * vectorB[i];
                magnitudeA += vectorA[i] * vectorA[i];
                magnitudeB += vectorB[i] * vectorB[i];
            }

            if (magnitudeA == 0 || magnitudeB == 0)
            {
                return 0;
            }

            return dotProduct /
                (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
        }
    }
}