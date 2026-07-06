FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ProjectManager.Api/ProjectManager.Api.csproj", "ProjectManager.Api/"]
RUN dotnet restore "ProjectManager.Api/ProjectManager.Api.csproj"
COPY . .
WORKDIR "/src/ProjectManager.Api"
RUN dotnet build "./ProjectManager.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ProjectManager.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
VOLUME /app/Logs
ENTRYPOINT ["dotnet", "ProjectManager.Api.dll"]
