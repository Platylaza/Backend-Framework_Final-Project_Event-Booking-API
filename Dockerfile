# -------- Build stage --------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore first for better layer caching
COPY EventBookingApi.csproj ./
RUN dotnet restore

# Copy the rest of the source
COPY . ./

# Publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# -------- Runtime stage --------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Bind Kestrel to port 8080 inside the container
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

COPY --from=build /app/publish ./
ENTRYPOINT ["dotnet", "EventBookingApi.dll"]