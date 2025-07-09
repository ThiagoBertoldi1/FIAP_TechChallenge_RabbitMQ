FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Queue.Delete/Queue.Delete.csproj", "Queue.Delete/"]
COPY ["Queue.Domain/Queue.Domain.csproj", "Queue.Domain/"]
COPY ["Queue.Repository/Queue.Repository.csproj", "Queue.Repository/"]
RUN dotnet restore "./Queue.Delete/Queue.Delete.csproj"
COPY . .
WORKDIR "/src/Queue.Delete"
RUN dotnet build "./Queue.Delete.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Queue.Delete.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Queue.Delete.dll"]