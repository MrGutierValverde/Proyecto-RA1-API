using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MCPServer;
using APIrpoyecto.Models; // Si referencias proyecto que contiene MonsterDAOsql

var builder = WebApplication.CreateBuilder(args);

// Register your services
builder.Services.AddSingleton<MonsterMcpService>();
builder.Services.AddSingleton(provider =>
    new MonsterDAOsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Register McpWebSocketHandler as a singleton or transient (depending on your needs)
builder.Services.AddSingleton<McpWebSocketHandler>();

var app = builder.Build();

app.MapGet("/", () => "MCP WebSocket server running");

// WebSocket endpoint
app.UseWebSockets();
app.Map("/mcp/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var ws = await context.WebSockets.AcceptWebSocketAsync();
        var handler = context.RequestServices.GetRequiredService<McpWebSocketHandler>();
        await handler.HandleAsync(ws, context.RequestAborted);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.Run();
