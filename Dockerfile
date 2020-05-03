FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY *.sln .

# Copy the main source project files
RUN find . -name "*.csproj"
COPY . .

RUN dotnet restore

WORKDIR /src/WebApi
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /src/WebApi/out ./
ENTRYPOINT ["dotnet", "WebApi.dll"]