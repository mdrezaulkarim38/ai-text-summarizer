# 🤖 AI Text Summarizer

> A production-grade web application that summarizes long articles, documents, emails, and reports into concise, readable summaries — powered by a local LLM running on your own machine via **Ollama**, orchestrated with **Semantic Kernel** in ASP.NET Core.

![Stack](https://img.shields.io/badge/.NET%2010-512BD4?logo=dotnet&logoColor=white) ![Stack](https://img.shields.io/badge/React%2019-61DAFB?logo=react&logoColor=black) ![Stack](https://img.shields.io/badge/Semantic%20Kernel-7C4DFF) ![Stack](https://img.shields.io/badge/Ollama-000000?logo=ollama) ![Stack](https://img.shields.io/badge/TypeScript-3178C6?logo=typescript) ![Stack](https://img.shields.io/badge/Bootstrap%205.3-7952B3?logo=bootstrap)

---

## 📌 What is this project?

People are drowning in information. Long articles, research papers, meeting notes, and reports take too long to read. **AI Text Summarizer** solves this by condensing any text into a clear, structured summary in seconds — without sending your data to the cloud.

Blazing fast, private, and free — the entire AI runs **locally** on your machine using **qwen3:8b** (an open-source LLM managed by Ollama). Nothing ever leaves your computer.

---

## ✨ Key Features

- **Instant Summaries** — Paste any text, click "Summarize", and get a concise result in seconds.
- **Live Streaming Output** — Watch the summary being generated word-by-word in real time (Server-Sent Events).
- **Private & Secure** — Runs 100% locally. Your documents never leave your machine. Ideal for confidential business data.
- **Customizable Output** — Length control, bullet-point vs. paragraph format, and example texts to try.
- **Copy & Download** — One-click copy to clipboard or download the summary as `.txt` / `.md`.
- **Dark Mode** — Easy on the eyes for long working sessions.
- **Production-Ready** — Retry policies, circuit breaker, caching, rate limiting, health checks, and structured logging.

---

## 🏗️ Architecture

```
┌──────────────────┐   REST + SSE    ┌─────────────────────────┐   Semantic Kernel   ┌──────────────┐
│    React UI      │ ◄──────────────► │   ASP.NET Core 10 API   │ ◄─────────────────► │    Ollama    │
│ (Vite + Bootstrap)│    JSON stream   │ (Controllers)           │  streaming/prompts │  qwen3:8b    │
└──────────────────┘                  └─────────────────────────┘                     └──────────────┘
```

| Layer | Technology | Responsibility |
|-------|-----------|----------------|
| **Frontend** | React 19, TypeScript, Vite, Bootstrap 5.3 | User interface, streaming display |
| **API** | ASP.NET Core 10 (Controller-based) | REST endpoints, validation, SSE streaming, resilience |
| **AI Orchestration** | Semantic Kernel + Microsoft.Extensions.AI | Prompt templates, model invocation |
| **AI Model** | Ollama with qwen3:8b (local) | Text generation & summarization |

### Solution Structure

```
ai-text-summarizer/
├── server/
│   ├── AITextSummarizer.Api/           # ASP.NET Core 10 Web API (Controllers)
│   ├── AITextSummarizer.Core/          # Domain models & interfaces
│   └── AITextSummarizer.Infrastructure/# Semantic Kernel + Ollama implementation
├── client/                             # React 19 + TypeScript + Vite frontend
├── tests/
│   └── AITextSummarizer.Tests/         # Unit & integration tests
├── docker-compose.yml                  # One-command local setup
└── README.md
```

---

## 🧰 Tech Stack

| Area | Technology |
|------|-----------|
| Backend | ASP.NET Core 10, C# |
| AI Framework | Semantic Kernel, Microsoft.Extensions.AI |
| Local LLM | Ollama (qwen3:8b) |
| Frontend | React 19, TypeScript, Vite |
| Styling | Bootstrap 5.3 |
| Resilience | Polly (retry, circuit breaker, timeout) |
| Observability | Serilog structured logging, ASP.NET Health Checks |
| API Quality | FluentValidation, rate limiting, output caching |
| Deployment | Docker, Docker Compose |

---

## 🚀 Quick Start

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/)
- [Ollama](https://ollama.com/) with qwen3:8b model

```bash
# 1. Clone the repository
git clone https://github.com/mdrezaulkarim38/ai-text-summarizer.git
cd ai-text-summarizer

# 2. Pull the model (one-time)
ollama pull qwen3:8b

# 3. Start backend
cd server/AITextSummarizer.Api
dotnet run

# 4. In another terminal, start frontend
cd ../../client
npm install
npm run dev
```

**Or run everything with Docker:**
```bash
docker-compose up --build
```

Open http://localhost:3000 → paste text → get your summary.

---

## 📡 API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/summarization` | Summarize text (non-streaming, returns JSON) |
| `POST` | `/api/summarization/stream` | Summarize text (Server-Sent Events streaming) |
| `GET`  | `/health` | Liveness check |
| `GET`  | `/health/ready` | Readiness check (Ollama connected?) |

---

## 🛡️ Production Patterns Included

These are the patterns that separate junior from senior engineering:

- **Resilience (Polly)** — retry with exponential backoff, circuit breaker, timeout handling
- **Caching** — repeated summaries served from the output cache
- **Rate Limiting** — protects the API from abuse
- **Health Checks** — liveness + readiness endpoints for orchestration
- **Structured Logging** — Serilog with request/response context
- **Validation** — FluentValidation for clean request validation
- **Security** — inputs validated, secrets managed via appsettings, local-by-design privacy

---

## 🗺️ Roadmap

- [x] Solution structure & backend setup (.NET 10, server/ + tests/)
- [x] Ollama + Semantic Kernel summarization service (qwen3:8b)
- [x] REST + SSE streaming endpoints (tested via Scalar UI)
- [x] React 19 + TypeScript + Vite + Bootstrap 5 frontend
- [x] UI: input → summarize → output with copy/download, streaming toggle, error handling
- [ ] FluentValidation request validation
- [ ] Serilog structured logging + health checks
- [ ] Example texts + dark mode + keyboard shortcuts
- [ ] Resilience (Polly), caching, rate limiting
- [ ] Unit + integration tests (70%+ coverage)
- [ ] Docker Compose deployment
- [ ] GitHub Actions CI/CD
- [ ] Live deployment (Azure / Railway)

---

## 👨‍💻 About the Author

Built by **Md. Rezaul Karim** — a .NET engineer (ASP.NET Core, ERP/CRM/Accounting systems) expanding into AI Engineering. This is the first project in a journey from *"I can call an LLM API"* to *"I can architect production AI systems."*

- 💼 3+ years professional .NET experience
- 🧠 Learning path: Semantic Kernel → RAG → ML.NET + LLM pipelines
- 🔗 [GitHub](https://github.com/mdrezaulkarim38)

---

## 📄 License

[MIT](LICENSE)