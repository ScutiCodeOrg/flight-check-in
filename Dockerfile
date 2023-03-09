FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app


# Copy csproj and restore as distinct layers
COPY *.sln .
COPY  ["/Britannica.Domain/*.csproj", "./Britannica.Domain/"]
COPY  ["/Britannica.Application/*.csproj", "./Britannica.Application/"]
COPY  ["/Britannica.Infrastructure/*.csproj", "./Britannica.Infrastructure/"]
COPY  ["/Britannica.Host/*.csproj", "./Britannica.Host/"]
#COPY  ["/Britannica.UnitTest/*.csproj", "./Britannica.UnitTest/"]

COPY ./nuget.config ./
RUN sed -i "s|localhost:5555|host.docker.internal:5555|g" ./nuget.config
RUN dotnet restore

# run tests on docker build
# RUN dotnet test -c Release --logger "trx;LogFileName=testresults.trx"; 

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final

# update + upgrade system
RUN apt-get update -y && apt-get upgrade -y

LABEL maintainer="avin@scuticode.com"
WORKDIR /app
COPY --from=build-env /app/out .

ENV ASPNETCORE_URLS "http://+:10000"

ENTRYPOINT ["dotnet", "Britannica.Host.dll"]