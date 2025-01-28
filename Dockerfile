#Base Image for Build - dotnetcore SDK Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8082

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# copy all the layers' csproj files into respective folders
COPY "DeliveryApp.Api/DeliveryApp.Api.csproj" "DeliveryApp.Api/DeliveryApp.Api.csproj"
COPY "DeliveryApp.Core/DeliveryApp.Core.csproj" "DeliveryApp.Core/DeliveryApp.Core.csproj"
COPY "DeliveryApp.Infrastructure/DeliveryApp.Infrastructure.csproj" "DeliveryApp.Infrastructure/DeliveryApp.Infrastructure.csproj"
COPY "Tests/DeliveryApp.ComponentTests/DeliveryApp.ComponentTests.csproj" "Tests/DeliveryApp.ComponentTests/DeliveryApp.ComponentTests.csproj"
COPY "Tests/DeliveryApp.IntegrationTests/DeliveryApp.IntegrationTests.csproj" "Tests/DeliveryApp.IntegrationTests/DeliveryApp.IntegrationTests.csproj"
COPY "Tests/DeliveryApp.UnitTests/DeliveryApp.UnitTests.csproj" "Tests/DeliveryApp.UnitTests/DeliveryApp.UnitTests.csproj"
COPY "Utils/Primitives/Primitives.csproj" "Utils/Primitives/Primitives.csproj"
COPY "NuGet.config" "NuGet.config"

# run restore over API project - this pulls restore over the dependent projects as well
RUN dotnet restore "DeliveryApp.Api/DeliveryApp.Api.csproj"

#Copy all the source code into the Build Container
COPY . .

# Run dotnet publish in the Build Container
# Generates output available in /app/build
# Since the current directory is /app
WORKDIR /src/DeliveryApp.Api
RUN dotnet build -c Release -o /app/build

FROM build as unittest
WORKDIR /src/DeliveryApp.UnitTests

# run publish over the API project
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# Step 2 - Continuing from the End of Step 1 #
# Second Stage - Pick an Image with only dotnetcore Runtime
# base is defined at the top of the script - mcr.microsoft.com/dotnet/aspnet:6.0
FROM base AS runtime

# Set the Directory as /app
# All consecutive operations happen under /app
WORKDIR /app

# Copy the dlls generated under /app/out of the previous step
# With alias build onto the current directory
# Which is /app in runtime
COPY --from=publish /app/publish .

# Set the Entrypoint for the Container
# Entrypoint is for executables (such as exe, dll)
# Which cannot be overriden by run command
# or docker-compose
ENTRYPOINT ["dotnet", "DeliveryApp.Api.dll"]