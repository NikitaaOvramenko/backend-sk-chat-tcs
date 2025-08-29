# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

RUN apt-get update && apt-get install -y python3 python3-pip \
    && pip3 install --no-cache-dir requests pillow google-generativeai


WORKDIR /src

# Copy everything
COPY . .

# Restore dependencies
RUN dotnet restore "backend-sk-chat-tcs/backend-sk-chat-tcs.csproj"

# Build & publish
RUN dotnet publish "backend-sk-chat-tcs/backend-sk-chat-tcs.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port your app runs on
EXPOSE 8080

# Run your app
ENTRYPOINT ["dotnet", "backend-sk-chat-tcs.dll"]
