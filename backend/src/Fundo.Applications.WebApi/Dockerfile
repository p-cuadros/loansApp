# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy project files
COPY Fundo.Domain/Fundo.Domain.csproj Fundo.Domain/
COPY Fundo.Infrastructure/Fundo.Infrastructure.csproj Fundo.Infrastructure/
COPY Fundo.Applications.WebApi/Fundo.Applications.WebApi.csproj Fundo.Applications.WebApi/

# Restore
RUN dotnet restore Fundo.Applications.WebApi/Fundo.Applications.WebApi.csproj

# Copy source
COPY . .

# Publish
RUN dotnet publish Fundo.Applications.WebApi/Fundo.Applications.WebApi.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Fundo.Applications.WebApi.dll"]
