# ghaw-examples

Example repository with standard C# code featuring HTTP request functions.

## HttpRequestFunction

An ASP.NET Core minimal API project demonstrating HTTP request handling in C#.

### Endpoints

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/hello` | Returns a simple greeting message |
| GET | `/fetch` | Makes an outbound HTTP GET request to a public API |
| POST | `/echo` | Echoes back the request body |

### Running locally

```bash
cd HttpRequestFunction
dotnet run
```