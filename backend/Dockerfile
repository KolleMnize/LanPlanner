FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Erst nur die Projektdateien kopieren, damit der Restore im Cache bleibt
COPY LanPlanner.slnx ./
COPY src/LanPlanner.Api/LanPlanner.Api.csproj src/LanPlanner.Api/
COPY src/LanPlanner.Application/LanPlanner.Application.csproj src/LanPlanner.Application/
COPY src/LanPlanner.Domain/LanPlanner.Domain.csproj src/LanPlanner.Domain/
COPY src/LanPlanner.Infrastructure/LanPlanner.Infrastructure.csproj src/LanPlanner.Infrastructure/
RUN dotnet restore src/LanPlanner.Api/LanPlanner.Api.csproj

COPY src/ src/
RUN dotnet publish src/LanPlanner.Api/LanPlanner.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "LanPlanner.Api.dll"]