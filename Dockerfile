FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
COPY ./ /App/
WORKDIR /App/
RUN dotnet test
RUN dotnet publish -c Release -o release

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS deploy
WORKDIR /App
COPY --from=build /App/release .
ENV ASPNETCORE_URLS "http://*:80" 
ENTRYPOINT ["dotnet", "Notification.Api.dll"]
