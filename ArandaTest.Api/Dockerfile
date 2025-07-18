
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ArandaTest.Api/ArandaTest.Api.csproj", "ArandaTest.Api/"]
COPY ["ArandaTest.Domain/ArandaTest.Domain.csproj", "ArandaTest.Domain/"]
COPY ["ArandaTest.Infrastructure/ArandaTest.Infrastructure.csproj", "ArandaTest.Infrastructure/"]
COPY ["ArandaTest.Application/ArandaTest.Application.csproj", "ArandaTest.Application/"]
RUN dotnet restore "./ArandaTest.Api/ArandaTest.Api.csproj"
COPY . .
WORKDIR "/src/ArandaTest.Api"
RUN dotnet build "./ArandaTest.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ArandaTest.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ArandaTest.Api.dll"]