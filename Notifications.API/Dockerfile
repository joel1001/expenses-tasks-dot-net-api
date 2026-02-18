FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["Notifications.API.csproj", "./"]
RUN dotnet restore "Notifications.API.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "Notifications.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Notifications.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notifications.API.dll"]
