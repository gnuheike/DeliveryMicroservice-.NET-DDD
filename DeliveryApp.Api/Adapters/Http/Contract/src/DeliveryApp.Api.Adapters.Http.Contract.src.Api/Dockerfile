#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Container we use for final publish
FROM mcr.microsoft.com/dotnet/core/aspnet:8.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build container
FROM mcr.microsoft.com/dotnet/core/sdk:8.0-buster AS build

# Copy the code into the container
WORKDIR /src
COPY ["src/DeliveryApp.Api.Adapters.Http.Contract.src.Api/DeliveryApp.Api.Adapters.Http.Contract.src.Api.csproj", "DeliveryApp.Api.Adapters.Http.Contract.src.Api/"]

# NuGet restore
RUN dotnet restore "DeliveryApp.Api.Adapters.Http.Contract.src.Api/DeliveryApp.Api.Adapters.Http.Contract.src.Api.csproj"
COPY ["src/DeliveryApp.Api.Adapters.Http.Contract.src.Api/", "DeliveryApp.Api.Adapters.Http.Contract.src.Api/"]

# Build the API
WORKDIR "DeliveryApp.Api.Adapters.Http.Contract.src.Api"
RUN dotnet build "DeliveryApp.Api.Adapters.Http.Contract.src.Api.csproj" -c Release -o /app/build

# Publish it
FROM build AS publish
RUN dotnet publish "DeliveryApp.Api.Adapters.Http.Contract.src.Api.csproj" -c Release -o /app/publish

# Make the final image for publishing
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DeliveryApp.Api.Adapters.Http.Contract.src.Api.dll"]
