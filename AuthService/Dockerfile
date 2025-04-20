FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY AuthService/AuthService.csproj AuthService/
COPY GameStore.Common/GameStore.Common.csproj GameStore.Common/
RUN dotnet restore AuthService/AuthService.csproj

COPY AuthService/ AuthService/
COPY /GameStore.Common/ GameStore.Common/
RUN dotnet publish AuthService/AuthService.csproj -c Realease -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "AuthService.dll"]