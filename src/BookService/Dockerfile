FROM microsoft/aspnetcore:2.0 AS base
RUN apt-get update && apt-get -q install -y curl
RUN apt-get -q install -y net-tools
RUN apt-get -q install -y dnsutils
RUN apt-get -q install -y procps
RUN apt-get -q install -y iputils-ping
RUN apt-get -q install -y inetutils-traceroute
RUN apt-get -q install -y nano

WORKDIR /app
EXPOSE 8080

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY BookService/BookService.csproj BookService/
RUN dotnet restore BookService/BookService.csproj
COPY . .
WORKDIR /src/BookService
RUN dotnet build BookService.csproj -c Release -o /app
RUN dotnet publish BookService.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "BookService.dll"]
