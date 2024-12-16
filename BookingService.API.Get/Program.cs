var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:4940");

builder.Services
    .AddCors(options =>
        options.AddPolicy(
            "Booking",
            policy =>
                policy
                    .WithOrigins()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
        )
    );

var app = builder.Build();

app.UseCors("Booking");

// 🟢 GET endpoint to fetch bookings from the repository service
app.MapGet("/bookings", async (HttpContext context) =>
{
    var repoUri = Environment.GetEnvironmentVariable("REPO_URI") ?? "http://repo:5030";
    using var httpClient = new HttpClient
    {
        BaseAddress = new Uri(repoUri)
    };

    // Forward the request to the repository service
    var response = await httpClient.GetAsync("/bookings");
    if (!response.IsSuccessStatusCode)
    {
        return Results.StatusCode((int)response.StatusCode);
    }

    // Return the data from the repository service
    var data = await response.Content.ReadAsStringAsync();
    return Results.Content(data, "application/json");
});

app.Run();
