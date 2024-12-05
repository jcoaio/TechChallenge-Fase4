FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /app
COPY ["./src/TechChallenge.Fase3.Consumer/TechChallenge.Fase3.Consumer.csproj", "TechChallenge.Fase3.Consumer/"]
COPY . .
WORKDIR "/app/src/TechChallenge.Fase3.Consumer"


RUN dotnet build "./TechChallenge.Fase3.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TechChallenge.Fase3.Consumer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechChallenge.Fase3.Consumer.dll"]
