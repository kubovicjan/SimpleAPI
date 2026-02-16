# SimpleAPI

A .NET 10 Web API that provides card information by proxying requests to an external card data provider. The API is secured with API key authentication and includes health check support.

## Prerequisites

- To build and run the application install [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- To run application install [ASP.NET Core Runtime](https://dotnet.microsoft.com/download/dotnet/10.0)

## Configuration

The API is configured via `appsettings.json` (or environment variables). The required configuration section is:

| Setting              | Description                                      |
|----------------------|--------------------------------------------------|
| `RemoteServiceBaseUrl` | Base URL of the external card data provider.     |
| `ApiKey`               | API key used to authenticate incoming requests.  |

You can override these values using environment variables:

## Running the API

### From the command line
```
dotnet run --launch-profile https --project SimpleAPI\SimpleAPI
```

### From Visual Studio

1. Open the solution in Visual Studio.
2. Set **SimpleAPI** as the startup project.
3. Press **F5** (or **Ctrl+F5** to run without debugging).

The API will start on `http://localhost:5201` and on `https://localhost:7201` by default.

## API Endpoints

### Get Card Info
**Endpoint:** `GET /api/cardinfo/{cardNumber}`

**Headers:**

| Header      | Required | Description            |
|-------------|----------|------------------------|
| `X-API-KEY` | Yes      | Your configured API key |

**Example request using Curl:**

```
curl -X GET "http://localhost:5201/api/cardinfo/1234567890123456" -H "X-API-KEY: your_api_key"
```
**Example response:**
```json
{
  "cardId": "1234567890123456",
  "stateDescripiton": "Aktivní v držení klienta",
  "validityEnd": "12.08.2020"
}
```

### Health Check

The API includes a health check endpoint to verify its status.

**Endpoint:** `GET /health`

**Response:**

```
 Healthy
```

### OpenAPI

The OpenAPI specification is available when running in the `Development` environment.

**Endpoint:** `GET /openapi/v1.json`

This returns the OpenAPI v3.1.1 document describing available endpoint, request/response schemas, and authentication requirements.

### Unit tests
Unit tests are located in the `SimpleAPI.Tests` project. To run the tests, use the following command:
```
dotnet test SimpleAPI\SimpleAPI.Tests

```


