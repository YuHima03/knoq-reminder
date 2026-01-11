# デバッグ コンテナーをカスタマイズする方法と、Visual Studio がこの Dockerfile を使用してより高速なデバッグのためにイメージをビルドする方法については、https://aka.ms/customizecontainer をご覧ください。

# このステージは、VS から高速モードで実行するときに使用されます (デバッグ構成の既定値)
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble-chiseled-extra AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# このステージは、サービス プロジェクトのビルドに使用されます
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

# このステージは、最終ステージにコピーするサービス プロジェクトを公開するために使用されます
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./KnoqReminder.App.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# このステージは、運用環境または VS から通常モードで実行している場合に使用されます (デバッグ構成を使用しない場合の既定)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KnoqReminder.App.dll"]
