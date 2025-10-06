# =========================
# 1. BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the source code
COPY . ./

# Build and publish the app
RUN dotnet publish -c Release -o out

# =========================
# 2. RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published files from the build stage
COPY --from=build /app/out ./

# Expose port 8080 (Render expects your app to listen on this port)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Start the app
ENTRYPOINT ["dotnet", "ToxicFitnessAPI.dll"]
