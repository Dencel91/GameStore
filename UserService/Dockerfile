FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY UserService/*.csproj UserService/
COPY GameStore.Common/GameStore.Common.csproj GameStore.Common/
RUN dotnet restore UserService/UserService.csproj


COPY UserService/ UserService/
COPY /GameStore.Common/ GameStore.Common/
RUN dotnet publish UserService/UserService.csproj  -c Realease -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "UserService.dll"]