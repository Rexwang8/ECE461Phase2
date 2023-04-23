FROM mcr.microsoft.com/dotnet/core/sdk:3.1
WORKDIR /app

ENV DOTNET_CLI_TELEMETRY_OPTOUT 1

# Download pip and python
RUN apt-get update && apt-get install -y python3 python3-pip



# copy everything else and build
COPY rexapi/aspnetcore-server-generated/ ./
RUN dotnet publish -c Release -o out
RUN ls -la


#C# Dependencies
RUN dotnet add package Google.Api.Gax --version 4.3.1
RUN dotnet add package Google.Apis.Auth --version 1.60.0
RUN dotnet add package Google.Apis.Bigquery.v2 --version 1.60.0.2998
RUN dotnet add package Google.Cloud.BigQuery.V2                --version 3.2.0
RUN dotnet add package Google.Cloud.SecretManager.V1           --version 2.1.0
RUN dotnet add package Google.Protobuf                         --version 3.22.3
RUN dotnet add package Swashbuckle.AspNetCore                  --version 5.5.1
RUN dotnet add package Swashbuckle.AspNetCore.Annotations      --version 5.5.1
RUN dotnet add package Swashbuckle.AspNetCore.Newtonsoft       --version 5.5.1
RUN dotnet add package Swashbuckle.AspNetCore.SwaggerGen       --version 5.5.1
RUN dotnet add package Swashbuckle.AspNetCore.SwaggerUI        --version 5.5.1 
RUN dotnet add package GraphQL.Client --version 5.1.1
RUN dotnet add package GraphQL.Client.Serializer.Newtonsoft


# build runtime image

ENTRYPOINT ["dotnet", "run --project IO.Swagger/IO.Swagger.csproj"]
