# Stage 1: Build (for .NET)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything
COPY . .

# Restore dependencies
RUN dotnet restore "backend-sk-chat-tcs/backend-sk-chat-tcs.csproj"

# Build & publish
RUN dotnet publish "backend-sk-chat-tcs/backend-sk-chat-tcs.csproj" -c Release -o /app/publish


# Stage 2: Runtime (ASP.NET + Python)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install Python + pip + required libs in final runtime
RUN apt-get update && apt-get install -y python3 python3-pip python3-dev build-essential \
    libffi-dev libssl-dev \
    && pip3 install --no-cache-dir --upgrade pip \
    && pip3 install --no-cache-dir requests pillow google-genai

# Copy published app from build stage
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Run your app
ENTRYPOINT ["dotnet", "backend-sk-chat-tcs.dll"]
