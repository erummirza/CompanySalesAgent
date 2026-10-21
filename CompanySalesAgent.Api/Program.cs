using CompanySalesAgent.Api.Data;
using CompanySalesAgent.Api.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore; // Required for the new UI

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy => policy.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});
// 1. Add Database Connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// 1. Add Controllers
builder.Services.AddControllers();

// 2. Register RagService (Assuming it uses HttpClient)
builder.Services.AddHttpClient<RagService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:11434/");
    client.Timeout = TimeSpan.FromMinutes(3);
});

// 3. Register OllamaService
builder.Services.AddHttpClient<OllamaService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:11434/");
    client.Timeout = TimeSpan.FromMinutes(10); // 10 mins for slow local LLMs
});

// 4. Register .NET 10 Native OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// 5. Map the OpenAPI JSON and the Scalar UI
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();               // Generates the JSON at /openapi/v1.json
    app.MapScalarApiReference();    // Generates the UI at /scalar/v1
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.UseAuthorization();
app.MapControllers();

app.Run();