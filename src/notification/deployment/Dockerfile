FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /App
COPY ../../ /App/
RUN dotnet restore notification-service.sln
RUN dotnet publish -c Release -o release

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build /App/release .
ENTRYPOINT ["dotnet", "Notification.Api.dll"]
