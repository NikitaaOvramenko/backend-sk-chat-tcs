# ---------- build .NET ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "backend-sk-chat-tcs/backend-sk-chat-tcs.csproj"
RUN dotnet publish "backend-sk-chat-tcs/backend-sk-chat-tcs.csproj" -c Release -o /app/publish

# ---------- runtime: ASP.NET + Python ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# OS deps then Python packages (split for clearer errors)
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    python3 python3-pip python3-dev \
    build-essential libffi-dev libssl-dev \
    libprotobuf-dev protobuf-compiler pkg-config \
    ca-certificates curl \
    && rm -rf /var/lib/apt/lists/*

# Optional: see versions in Render logs
RUN python3 --version && pip3 --version

# Python libs you use
RUN pip3 install --no-cache-dir --upgrade pip \
    && pip3 install --no-cache-dir requests pillow google-genai

# bring published app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "backend-sk-chat-tcs.dll"]
