FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["FinaTech.Web.Blazor/FinaTech.Web.Blazor.csproj", "FinaTech.Web.Blazor/"]
RUN dotnet restore "FinaTech.Web.Blazor/FinaTech.Web.Blazor.csproj"
COPY . .
WORKDIR "/src/FinaTech.Web.Blazor"
RUN dotnet build "./FinaTech.Web.Blazor.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FinaTech.Web.Blazor.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FinaTech.Web.Blazor.dll"]
