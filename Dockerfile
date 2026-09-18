FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY src/PersonApp/PersonApp.sln src/PersonApp/
COPY src/PersonApp/PersonApp.Core/PersonApp.Core.csproj src/PersonApp/PersonApp.Core/
COPY src/PersonApp/PersonApp.Application/PersonApp.Application.csproj src/PersonApp/PersonApp.Application/
COPY src/PersonApp/PersonApp.DataAccess/PersonApp.DataAccess.csproj src/PersonApp/PersonApp.DataAccess/
COPY src/PersonApp/PersonApp.Server/PersonApp.Server.csproj src/PersonApp/PersonApp.Server/
COPY src/PersonApp/PersonApp.Tests/PersonApp.Tests.csproj src/PersonApp/PersonApp.Tests/

RUN dotnet restore src/PersonApp/PersonApp.sln

COPY . .

RUN dotnet publish src/PersonApp/PersonApp.Server/PersonApp.Server.csproj \
    --configuration Release \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production
ENV PORT=8080

EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PersonApp.Server.dll"]
