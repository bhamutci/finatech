FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FinaTech.Web.Host/FinaTech.Web.Host.csproj", "FinaTech.Web.Host/"]
RUN dotnet restore "FinaTech.Web.Host/FinaTech.Web.Host.csproj"
COPY . .
WORKDIR "/src/FinaTech.Web.Host"
RUN dotnet build "./FinaTech.Web.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FinaTech.Web.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinaTech.Web.Host.dll"]
