using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var appName = Assembly.GetEntryAssembly()!.GetName().Name!;

builder.Logging.AddOpenTelemetry(config =>
{
    config.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(appName));
    config.IncludeScopes = true;
    config.IncludeFormattedMessage = true;
});

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(appName))
    .WithMetrics(config =>
    {
        config.AddRuntimeInstrumentation()
            .AddMeter(
                "Microsoft.AspNetCore.Hosting",
                "Microsoft.AspNetCore.Server.Kestrel",
                "System.Net.Http");
    })
    .WithTracing(config =>
    {
        config.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
    });

builder.Services.ConfigureOpenTelemetryLoggerProvider(logging => logging.AddOtlpExporter(config =>
{
    config.Endpoint = new Uri("http://aspiredashboard:18889/");
}));
builder.Services.ConfigureOpenTelemetryMeterProvider(metrics => metrics.AddOtlpExporter((config, metricReaderConfig) =>
{
    metricReaderConfig.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 10_000;
    config.Endpoint = new Uri("http://aspiredashboard:18889/");
}));
builder.Services.ConfigureOpenTelemetryTracerProvider(tracing => tracing.AddOtlpExporter(config =>
{
    config.Endpoint = new Uri("http://aspiredashboard:18889/");
}));


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
