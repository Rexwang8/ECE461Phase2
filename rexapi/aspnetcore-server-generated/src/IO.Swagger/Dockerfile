FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

ENV DOTNET_CLI_TELEMETRY_OPTOUT 1

# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080


ENTRYPOINT ["dotnet", "IO.Swagger.dll"]
