using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using APIrpoyecto.Models;

namespace MCPServer
{
    public class McpWebSocketHandler
    {
        private readonly ILogger<McpWebSocketHandler> _logger;
        private readonly MonsterMcpService _monsterService;

        public McpWebSocketHandler(ILogger<McpWebSocketHandler> logger, MonsterMcpService monsterService)
        {
            _logger = logger;
            _monsterService = monsterService;
        }

        public async Task HandleAsync(WebSocket ws, CancellationToken ct)
        {
            var buffer = new byte[16 * 1024];
            while (!ct.IsCancellationRequested && ws.State == WebSocketState.Open)
            {
                var sb = new StringBuilder();
                WebSocketReceiveResult result;
                do
                {
                    result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), ct);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", ct);
                        return;
                    }
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                } while (!result.EndOfMessage);

                var text = sb.ToString().Trim();
                if (string.IsNullOrEmpty(text)) continue;

                _logger.LogDebug("MCPServer received: {t}", text);
                try
                {
                    using var doc = JsonDocument.Parse(text);
                    var root = doc.RootElement;
                    var id = root.TryGetProperty("id", out var idEl) ? idEl.GetInt32() : 0;
                    var method = root.GetProperty("method").GetString();

                    if (method == "initialize")
                    {
                        var resp = new
                        {
                            jsonrpc = "2.0",
                            id = id,
                            result = new
                            {
                                protocolVersion = "2024-11-05",
                                serverInfo = new { name = "Monster-MCP-Server", version = "1.0.0" },
                                capabilities = new
                                {
                                    tools = new { listChanged = true },
                                    resources = new { listChanged = true },
                                    prompts = new { listChanged = true }
                                }
                            }
                        };
                        await SendAsync(ws, resp, ct);
                        continue;
                    }

                    if (method == "tools/list")
                    {
                        var resp = new
                        {
                            jsonrpc = "2.0",
                            id = id,
                            result = new
                            {
                                tools = new object[] {
                                    new {
                                        name = "monsters/list",
                                        title = "List all monsters",
                                        description = "Return all monsters in DB",
                                        inputSchema = new { type = "object", properties = new { }, required = Array.Empty<string>() }
                                    },
                                    new {
                                        name = "monsters/getById",
                                        title = "Get monster by id",
                                        description = "Return monster for given id",
                                        inputSchema = new {
                                            type = "object",
                                            properties = new {
                                                id = new { type = "integer", description = "Id of monster" }
                                            },
                                            required = new [] { "id" }
                                        }
                                    },
                                    new {
                                        name = "monsters/searchByName",
                                        title = "Search monsters by name",
                                       description = "Search monsters by name (like)",
                                        inputSchema = new {
                                            type = "object",
                                            properties = new {
                                                name = new { type = "string", description = "name substring" }
                                            },
                                            required = new [] { "name" }
                                        }
                                    }
                                }
                            }
                        };

                        await SendAsync(ws, resp, ct);
                        continue;
                    }


                    if (method == "tools/call")
                    {
                        var paramsEl = root.GetProperty("params");
                        var name = paramsEl.GetProperty("name").GetString();
                        var argsEl = paramsEl.GetProperty("arguments");
                        // Dispatch tool
                        string contentText = "{}";
                        if (name == "monsters/list")
                        {
                            var list = _monsterService.GetAll();
                            contentText = JsonSerializer.Serialize(list);
                        }
                        else if (name == "monsters/getById")
                        {
                            var idArg = argsEl.GetProperty("id").GetInt32();
                            var monster = _monsterService.GetById(idArg);
                            contentText = JsonSerializer.Serialize(monster);
                        }
                        else if (name == "monsters/searchByName")
                        {
                            var nameArg = argsEl.GetProperty("name").GetString();
                            var list = _monsterService.SearchByName(nameArg);
                            contentText = JsonSerializer.Serialize(list);
                        }
                        else
                        {
                            contentText = JsonSerializer.Serialize(new { error = "Tool not found" });
                        }

                        var resp = new
                        {
                            jsonrpc = "2.0",
                            id = id,
                            result = new
                            {
                                content = new object[] {
                                    new { type = "text", text = contentText }
                                }
                            }
                        };
                        await SendAsync(ws, resp, ct);
                        continue;
                    }

                    // default: method not supported
                    var err = new
                    {
                        jsonrpc = "2.0",
                        id = id,
                        error = new { code = -32601, message = "Method not found" }
                    };
                    await SendAsync(ws, err, ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error handling message");
                }
            }
        }

        private async Task SendAsync(WebSocket ws, object obj, CancellationToken ct)
        {
            var s = JsonSerializer.Serialize(obj);
            var bytes = Encoding.UTF8.GetBytes(s + "\n");
            await ws.SendAsync(bytes, WebSocketMessageType.Text, true, ct);
        }
    }
}
