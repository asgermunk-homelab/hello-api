var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new
{
    message = "Hello from the homelab, deployed by Flux!",
    environment = app.Environment.EnvironmentName,
    host = Environment.MachineName,
}));

// Kubernetes liveness and readiness probes call this endpoint.
app.MapGet("/healthz", () => Results.Ok("healthy"));

app.Run();
