FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY Discord_bot/Discord_bot.csproj Discord_bot/
COPY Discord_bot.SelectTable/Discord_bot.SelectTable.csproj Discord_bot.SelectTable/
RUN dotnet restore Discord_bot/Discord_bot.csproj
COPY . .
WORKDIR /src/Discord_bot
RUN dotnet build Discord_bot.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Discord_bot.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY **/*.json ./
COPY **/*.png ./
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Discord_bot.dll"]
