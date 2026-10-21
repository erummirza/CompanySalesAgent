# 🤖 AI-Powered Sales Agent CRM

An enterprise-grade, full-stack AI application that acts as a virtual sales assistant. It answers customer queries based on internal company documents using RAG (Retrieval-Augmented Generation) and automatically extracts customer data to save as leads in a SQL Server database.

## 🚀 Features
*   **Secure RAG Pipeline:** Answers questions strictly based on `company-info.txt` with built-in prompt guardrails to prevent hallucinations and prompt injection attacks.
*   **AI Data Extraction:** Automatically parses unstructured chat transcripts and generates structured JSON (Name, Email, Intent) to create new leads.
*   **Full-Stack Architecture:** React.js frontend connected to a .NET 10 Web API, backed by SQL Server.
*   **Local LLM Integration:** Uses Ollama (Llama 3.2) for zero-cost, privacy-first AI processing.

## 🛠️ Tech Stack
*   **Backend:** .NET 10, C#, ASP.NET Core Web API, Entity Framework Core
*   **Frontend:** React.js, Axios
*   **Database:** SQL Server
*   **AI/LLM:** Ollama (Llama 3.2), RAG, Prompt Engineering
*   **API Testing:** Scalar UI / OpenAPI

## ⚙️ How to Run Locally
1. Clone the repository.
2. Ensure you have **Ollama** installed and running `llama3.2:latest`.
3. Update the `appsettings.json` connection string to your SQL Server.
4. Run `Update-Database` in the Package Manager Console.
5. Run the .NET API (F5).
6. Navigate to the React folder, run `npm install` and `npm start`.
