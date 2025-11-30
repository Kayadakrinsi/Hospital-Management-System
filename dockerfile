# 1. Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj files
COPY API/HMSAPI/*.csproj API/HMSAPI/
COPY API/HMSBAL/*.csproj API/HMSBAL/
COPY API/HMSDAL/*.csproj API/HMSDAL/
COPY API/HMSMAL/*.csproj API/HMSMAL/

# Restore
RUN dotnet restore API/HMSAPI/HMSAPI.csproj

# Copy everything
COPY . .

# Build and publish
RUN dotnet publish API/HMSAPI/HMSAPI.csproj -c Release -o /app/out

# 2. Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 5000
ENTRYPOINT ["dotnet", "HMSAPI.dll"]
