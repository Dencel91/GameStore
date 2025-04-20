FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY ProductService/ProductService.csproj ProductService/
COPY GameStore.Common/GameStore.Common.csproj GameStore.Common/
RUN dotnet restore ProductService/ProductService.csproj

COPY ProductService/ ProductService/
COPY /GameStore.Common/ GameStore.Common/
RUN dotnet publish ProductService/ProductService.csproj -c Realease -o out

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "ProductService.dll"]