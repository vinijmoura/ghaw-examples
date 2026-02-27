var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

// GET /hello - simple greeting endpoint
app.MapGet("/hello", () => Results.Ok(new { Message = "Hello, World!" }));

// GET /fetch - makes an outbound HTTP GET request to a public API
app.MapGet("/fetch", async (IHttpClientFactory httpClientFactory, IConfiguration configuration) =>
{
    var todoUrl = configuration["ExternalApis:TodoUrl"]
        ?? "https://jsonplaceholder.typicode.com/todos/1";
    var client = httpClientFactory.CreateClient();
    var response = await client.GetAsync(todoUrl);

    if (!response.IsSuccessStatusCode)
        return Results.StatusCode((int)response.StatusCode);

    var content = await response.Content.ReadAsStringAsync();
    return Results.Content(content, "application/json");
});

// POST /echo - echoes back the request body
app.MapPost("/echo", async (HttpContext context) =>
{
    using var reader = new StreamReader(context.Request.Body);
    var body = await reader.ReadToEndAsync();
    return Results.Ok(new { Echo = body });
});

app.Run();
