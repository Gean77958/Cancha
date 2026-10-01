FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore

RUN dotnet publish -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish .

# No fijamos ASPNETCORE_URLS aquí: Program.cs ya arranca explícitamente
# con app.Run() leyendo la variable PORT que Render asigna en runtime.
# Fijarla en el Dockerfile sería redundante (y en Cafeteria quedó un
# valor que el código ignoraba).
EXPOSE 10000

ENTRYPOINT ["dotnet", "CanchaApi.dll"]
