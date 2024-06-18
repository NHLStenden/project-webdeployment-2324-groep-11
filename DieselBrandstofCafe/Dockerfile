# See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY DieselBrandstofCafe/DieselBrandstofCafe.csproj DieselBrandstofCafe/
RUN dotnet restore DieselBrandstofCafe/DieselBrandstofCafe.csproj

COPY . .
WORKDIR /src/DieselBrandstofCafe
RUN dotnet build DieselBrandstofCafe.csproj -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish DieselBrandstofCafe.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY wait-for-it.sh /wait-for-it.sh
RUN chmod +x /wait-for-it.sh

ENTRYPOINT ["/wait-for-it.sh", "mysqldb:3306", "-t", "15", "--", "dotnet", "DieselBrandstofCafe.dll"]
