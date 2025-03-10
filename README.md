# Real Estate Agent Statistics API

A .NET API solution that retrieves, processes, and provides statistics about real estate agents based on different criteria.

## 📋 Overview

This solution focuses on simplicity while demonstrating good architecture with scalability potential. It processes data from a partner API to calculate which real estate agents have the most property listings in Amsterdam.

## 🏗️ Architecture

The solution uses **vertical slice architecture** ([more info here](https://codeopinion.com/organizing-code-by-feature-using-vertical-slices/)) to organize code by business features rather than technical layers:

```
Features/
├── Shared/
└── Statistics/
    ├── ObjectsForSaleDataProvider.cs
    ├── StatisticsBackgroundService.cs
    ├── StatisticsService.cs
    └── StatisticsStore.cs
```

The codebase follows these principles:
- High cohesion (related classes grouped together, sometimes in the same file)
- Separation of concerns (HTTP handling, data processing, persistence)
- Feature-based organization for better discoverability

## 🔑 Key Components

- **ObjectsForSaleDataProvider**: Handles pagination with the partner API
- **StatisticsBackgroundService**: Fetches statistics data periodically
- **StatisticsService**: The main "handler" class
- **StatisticsStore**: Caches aggregated data for different queries

## 🔍 Technical Details

- Uses memory cache for simplicity
- Background service updates statistics periodically
- Endpoints return HTTP 204 (No Content) until background service completes first data fetch
- Handles API rate limiting through retries

## 🚀 Getting Started

### Prerequisites
- [.NET 9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

### Running the Application
```bash
cd Assignment.Api
dotnet run --launch-profile https
```
Go to https://localhost:5001/scalar
> There is an open [issue](https://github.com/scalar/scalar/issues/4916) with the Scalar UI where the response body is sometimes missing. In case Scalar UI shows no body for HTTP 200 responses, please refresh your browser. If that doesn't help, any other API client can be used, including web browser.

### Available Endpoints
- `GET /api/statistics/amsterdam/` - Top agents in Amsterdam
- `GET /api/statistics/amsterdam/tuin/` - Top agents for properties with gardens
- `GET /api/health` - Health check endpoint

## 🧪 Testing

The solution uses integration tests with WireMock to simulate the partner API:
- Tests real HTTP requests to API endpoints
- Simulates rate limiting and error scenarios
- Verifies correct aggregation of results

## 🔮 Future Enhancements

While intentionally kept simple, the solution could be enhanced with:
- Distributed caching for multi-instance deployment
- Relational database for more query capabilities
- More sophisticated error handling and resilience patterns
- Metrics collection and monitoring
