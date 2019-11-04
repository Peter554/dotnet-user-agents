FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /app

COPY *.sln .
COPY WebAPI/*.csproj ./WebAPI/
RUN dotnet restore

copy WebAPI/. ./WebAPI/
WORKDIR /app/WebAPI
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 as runtime
WORKDIR /app
COPY --from=build /app/WebAPI/out .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet WebAPI.dll