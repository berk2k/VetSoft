using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

// Create service collection
var services = new ServiceCollection();

// Add logging
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.AddDebug();
    builder.SetMinimumLevel(LogLevel.Information);
});

// Build the service provider
var serviceProvider = services.BuildServiceProvider();

// Get logger factory
var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

// Create a logger
var logger = loggerFactory.CreateLogger<Program>();

// Log messages
logger.LogInformation("Application started");