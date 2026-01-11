# cf. https://aka.ms/customizecontainer

FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled-composite-extra AS base
# -composite: composite version of runtime (higher performance and smaller image size)
# -extra: includes tzdata and ICU data
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0-noble AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY ["src/KnoqReminder.App/KnoqReminder.App/KnoqReminder.App.csproj", "src/KnoqReminder.App/KnoqReminder.App/"]
COPY ["src/KnoqReminder.Domain/KnoqReminder.Domain.csproj", "src/KnoqReminder.Domain/"]
COPY ["src/KnoqReminder.Infrastructure/KnoqReminder.Infrastructure.csproj", "src/KnoqReminder.Infrastructure/"]
COPY ["src/KnoqReminder.Utilities/KnoqReminder.Utilities.csproj", "src/KnoqReminder.Utilities/"]
COPY ["src/KnoqReminder.App/KnoqReminder.App.Client/KnoqReminder.App.Client.csproj", "src/KnoqReminder.App/KnoqReminder.App.Client/"]
RUN dotnet restore "./src/KnoqReminder.App/KnoqReminder.App/KnoqReminder.App.csproj"
COPY . .
WORKDIR "/src/src/KnoqReminder.App/KnoqReminder.App"
RUN dotnet build "./KnoqReminder.App.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./KnoqReminder.App.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KnoqReminder.App.dll"]
