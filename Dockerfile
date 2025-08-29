# ---------- Stage 1: build .NET ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "backend-sk-chat-tcs/backend-sk-chat-tcs.csproj"
RUN dotnet publish "backend-sk-chat-tcs/backend-sk-chat-tcs.csproj" -c Release -o /app/publish

# ---------- Stage 2: runtime (ASP.NET + Python venv) ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# OS deps needed for pip builds + SSL
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    python3 python3-pip python3-venv python3-dev \
    build-essential libffi-dev libssl-dev \
    libprotobuf-dev protobuf-compiler pkg-config \
    ca-certificates curl \
    && rm -rf /var/lib/apt/lists/*

# Create a dedicated virtualenv and put it on PATH
RUN python3 -m venv /opt/pyenv
ENV PATH="/opt/pyenv/bin:${PATH}"

# (Optional) show versions in build logs
RUN python --version && pip --version

# Install your Python libs into the venv
RUN pip install --no-cache-dir --upgrade pip \
    && pip install --no-cache-dir requests pillow google-genai

# bring the published .NET app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "backend-sk-chat-tcs.dll"]
