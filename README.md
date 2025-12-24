# Backend Service for TCS Chat

A robust Backend-for-Frontend (BFF) service designed to orchestrate AI-driven chat interactions using **Microsoft Semantic Kernel** and **Supabase**.

Currently tailored for [TCS Paints](https://github.com/NikitaaOvramenko/TCS---Paints), this service manages chat sessions, processes user intent, and integrates with AI models to provide intelligent responses and utility functions (like estimation and image modification).

## 🚀 Tech Stack

- **Framework**: .NET 8.0 (Web API)
- **AI Orchestration**: Microsoft Semantic Kernel (v1.63.0)
- **Backend-as-a-Service**: Supabase (Database, Storage, Realtime)
- **LLM Provider**: OpenAI (GPT Models)
- **Documentation**: Swagger / OpenAPI

## 🏗 Architecture

The application follows a clean **Controller-Service** architecture:

- **`SessionController`**: Handles HTTP requests for creating sessions, sending messages, and ending chats.
- **`ChatManager`**: A Singleton service that manages active chat sessions and history in-memory.
- **`SemanticKernel`**: Wrapper around the Microsoft Semantic Kernel to manage AI services and plugin registration.

### Plugins & Capabilities

The system uses the Semantic Kernel plugin architecture to extend AI capabilities:

#### Native Functions (C#)

- **`Estimate`**: Calculates painting costs based on wall dimensions and paint coverage.
  - _Function_: `CalcWallPrice(width, height, numberOfCoats)`
- **`ImageSurfaceColor`**: Integrates with Supabase Storage to handle image uploads and modification requests.
  - _Function_: `EditImageAsync`

#### Semantic Functions (Prompts)

- **`ImageToImagePlugin`**:
  - _Goal_: "Color the Surface" - Interprets user requests to recolor specific surfaces in an uploaded image.

## 🔌 Integration & Usage

### Current Integration

This backend is currently the intelligence layer for:

- **[TCS Paints](https://github.com/NikitaaOvramenko/TCS---Paints)**: A painting service platform.

### Environment Setup

The application requires the following environment variables to start:

```bash
OPENAI_APIKEY=sk-...
MODEL_NAME=gpt-4o
SUPBASE_URL=https://your-project.supabase.co
SUPBASE_KEY=your-supabase-anon-key
PAGE_URL=http://localhost:3000 # React Frontend URL for CORS
```

## 🗺 Roadmap

The goal is to evolve this service into a general-purpose AI Gateway that can support multiple frontend applications with distinct contexts and requirements.

### Future Integrations

- **[TCS Junk Removal](https://github.com/NikitaaOvramenko/TCS-Junk-Removal)**: Booking and estimation for junk removal services.
- **[Appliance Repair Site](https://github.com/NikitaaOvramenko/applience-repair-site)**: Diagnostic and scheduling assistance.

### Planned Improvements

- **Persistence**: Move chat history from in-memory `ConcurrentDictionary` to Supabase Database to survive server restarts.
- **Multi-Tenancy**: Configure system prompts and active plugins dynamically based on the incoming `Client-ID`.
- **Streaming**: Implement proper IAsyncEnumerable streaming for real-time frontend feedback.
