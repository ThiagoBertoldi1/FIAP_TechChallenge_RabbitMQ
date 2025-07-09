FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Queue.Update/Queue.Update.csproj", "Queue.Update/"]
COPY ["Queue.Domain/Queue.Domain.csproj", "Queue.Domain/"]
COPY ["Queue.Repository/Queue.Repository.csproj", "Queue.Repository/"]
RUN dotnet restore "./Queue.Update/Queue.Update.csproj"
COPY . .
WORKDIR "/src/Queue.Update"
RUN dotnet build "./Queue.Update.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Queue.Update.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Queue.Update.dll"]