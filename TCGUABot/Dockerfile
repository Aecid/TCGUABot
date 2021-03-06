FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 443

# install System.Drawing native dependencies
RUN apt-get update \
    && apt-get install -y --allow-unauthenticated \
        libc6-dev \
        libgdiplus \
        libx11-dev \
     && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["TCGUABot/TCGUABot.csproj", "TCGUABot/"]
RUN dotnet restore "TCGUABot/TCGUABot.csproj"
COPY . .
WORKDIR "/src/TCGUABot"
RUN dotnet build "TCGUABot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TCGUABot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TCGUABot.dll"]