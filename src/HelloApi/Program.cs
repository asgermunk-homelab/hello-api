using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// OpenTelemetry: logs, traces and metrics. The service name and the environment
// come from OTEL_SERVICE_NAME and OTEL_RESOURCE_ATTRIBUTES (set in the gitops overlays).
var otel = builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        // The probes call /healthz every few seconds. Do not trace them.
        .AddAspNetCoreInstrumentation(options =>
            options.Filter = context => !context.Request.Path.StartsWithSegments("/healthz"))
        .AddHttpClientInstrumentation())
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation())
    .WithLogging();

// Export only when an endpoint is set, like a connection string. In the cluster
// this is Alloy. A local "dotnet run" without the variable exports nothing.
if (!string.IsNullOrWhiteSpace(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]))
{
    otel.UseOtlpExporter();
}

var app = builder.Build();

app.MapGet("/", (ILogger<Program> logger) =>
{
    logger.LogInformation("Hello requested on {Host}", Environment.MachineName);
    return Results.Ok(new
    {
        message = "Hello from the homelab, deployed by Flux!",
        environment = app.Environment.EnvironmentName,
        host = Environment.MachineName,
    });
});

// Kubernetes liveness and readiness probes call this endpoint.
app.MapGet("/healthz", () => Results.Ok("healthy"));

app.Run();
