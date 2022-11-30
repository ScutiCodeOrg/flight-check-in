FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app


# Copy csproj and restore as distinct layers
COPY *.sln .
COPY  ["/Britannica.Domain/*.csproj", "./Britannica.Domain/"]
COPY  ["/Britannica.Application/*.csproj", "./Britannica.Application/"]
COPY  ["/Britannica.Infrastructure/*.csproj", "./Britannica.Infrastructure/"]
COPY  ["/Britannica.Host/*.csproj", "./Britannica.Host/"]
#COPY  ["/Britannica.UnitTest/*.csproj", "./Britannica.UnitTest/"]
COPY ./nuget.config ./
RUN dotnet restore

# run tests on docker build
# RUN dotnet test -c Release --logger "trx;LogFileName=testresults.trx"; 

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM unicorngroupnamespace/aspnet-base:develop AS final
LABEL maintainer="avin@scuticode.com"
ENV ASPNETCORE_URLS "https://+:10000"
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "Britannica.Host.dll"]