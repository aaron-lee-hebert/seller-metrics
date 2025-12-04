# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy Directory.Build.props and Directory.Packages.props first (Central Package Management)
COPY Directory.Build.props .
COPY Directory.Packages.props .

# Copy solution and project files for layer caching
COPY SellerMetrics.sln .
COPY SellerMetrics.Domain/SellerMetrics.Domain.csproj SellerMetrics.Domain/
COPY SellerMetrics.Application/SellerMetrics.Application.csproj SellerMetrics.Application/
COPY SellerMetrics.Infrastructure/SellerMetrics.Infrastructure.csproj SellerMetrics.Infrastructure/
COPY SellerMetrics.Web/SellerMetrics.Web.csproj SellerMetrics.Web/
COPY SellerMetrics.Tests/SellerMetrics.Tests.csproj SellerMetrics.Tests/

# Restore dependencies for entire solution
RUN dotnet restore SellerMetrics.sln

# Copy all source code
COPY . .

# Build and publish
RUN dotnet publish SellerMetrics.Web/SellerMetrics.Web.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Create non-root user for security
RUN groupadd -r appgroup && useradd -r -g appgroup appuser

# Copy published app
COPY --from=build /app/publish .

# Set ownership
RUN chown -R appuser:appgroup /app

# Switch to non-root user
USER appuser

# Expose port (ASP.NET Core 8+ defaults to port 8080)
EXPOSE 8080

# Health check using wget (curl not available in aspnet image)
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "SellerMetrics.Web.dll"]
