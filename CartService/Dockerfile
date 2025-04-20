FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY CartService/CartService.csproj CartService/
COPY GameStore.Common/GameStore.Common.csproj GameStore.Common/
RUN dotnet restore CartService/CartService.csproj

COPY CartService/ CartService/
COPY /GameStore.Common/ GameStore.Common/
RUN dotnet publish CartService/CartService.csproj -c Realease -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "CartService.dll"]