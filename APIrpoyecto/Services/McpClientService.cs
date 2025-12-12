using APIrpoyecto.Models;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Channels;

namespace APIrpoyecto.Services
{
    public class McpClientService
    {
        private readonly IConfiguration _cfg;
        private readonly ILogger<McpClientService> _logger;
        private readonly LMStudioClient _lmClient;
        private readonly HttpClient _http;
        private readonly string _mcpServerWsUrl;

        public McpClientService(IConfiguration cfg, ILogger<McpClientService> logger, LMStudioClient lmStudioClient, IHttpClientFactory httpFactory)
        {
            _cfg = cfg;
            _logger = logger;
            _lmClient = lmStudioClient;
            _http = httpFactory.CreateClient();
            _mcpServerWsUrl = _cfg["MCPServer:WsUrl"] ?? "ws://localhost:5101/mcp/ws";
        }

        // Router público: recibe consulta en lenguaje natural
        public async Task<string> RouteQueryAsync(string userQuery, CancellationToken ct = default)
        {
            // 1) Intent detection via manual rules (simple)
            var manual = TryManualRouting(userQuery);
            if (manual != null)
            {
                _logger.LogInformation("Manual router matched: {Name}", manual?.Name);
                return await CallMcpToolAsync(manual?.ToolName, manual?.ArgumentsJson, ct);
            }

            // 2) No manual match -> use LLM (LMStudio) to decide tool and args
            // We will ask LMStudio what tool and arguments to use.
            // Send the list of tools metadata so LM can choose. For simplicity we hardcode the tool descriptions.
            var toolsDescription = @"
- monsters/list : devuelve lista completa (sin argumentos)
- monsters/getById : args { ""id"": <int> }
- monsters/searchByName : args { ""name"": ""<string>"" }
- monsters/query : args { ""sql"": ""<sql string>"" } (use with caution)
";
            var lmRespText = await _lmClient.RequestToolDecisionAsync(userQuery, toolsDescription);
            _logger.LogDebug("LMStudio response: {resp}", lmRespText);

            // Try to extract JSON from LM response
            JsonDocument doc = null;
            try
            {
                doc = JsonDocument.Parse(lmRespText);
            }
            catch
            {
                // Attempt to find first JSON object substring
                var start = lmRespText.IndexOf('{');
                var end = lmRespText.LastIndexOf('}');
                if (start >= 0 && end >= start)
                {
                    var candidate = lmRespText.Substring(start, end - start + 1);
                    doc = JsonDocument.Parse(candidate);
                }
            }

            if (doc == null)
                return "No pude entender la respuesta del LLM para enrutamiento.";

            string tool = null;
            string argumentsJson = "{}";
            if (doc.RootElement.TryGetProperty("tool", out var tEl))
                tool = tEl.ValueKind == JsonValueKind.Null ? null : tEl.GetString();

            if (tool == null)
            {
                return "El LLM determinó que no es necesario invocar ninguna herramienta. Respuesta (raw): " + lmRespText;
            }

            if (doc.RootElement.TryGetProperty("arguments", out var argsEl))
                argumentsJson = argsEl.GetRawText();

            // Call MCP server tool
            var result = await CallMcpToolAsync(tool, argumentsJson, ct);
            return result;
        }

        private (string Name, string ToolName, string ArgumentsJson)? TryManualRouting(string q)
        {
            // Regla ejemplo: si pregunta contiene "todos los monstruos" o "listar monstruos"
            var qLow = q?.ToLowerInvariant() ?? "";
            if (qLow.Contains("todos los monstruos") || qLow.Contains("listar monstruos") || qLow.Contains("mostrar monstruos"))
            {
                return ("ListAll", "monsters/list", "{}");
            }
            if (qLow.Contains("monstruo por id") || qLow.Contains("monstruo con id") || qLow.Contains("monstruo id"))
            {
                // Intentamos extraer el id después de "por id"
                var idPattern = @"\d+";  // Expresión regular para buscar un número
                var match = Regex.Match(q, idPattern);
                if (match.Success)
                {
                    // Convertimos el ID extraído a un número entero
                    if (int.TryParse(match.Value, out int id))
                    {
                        var args = JsonSerializer.Serialize(new { id = id });
                        return ("SearchById", "monsters/getById", args);
                    }
                    else
                    {
                        // Si no se puede convertir a entero, retornamos null o manejamos el error de otra forma
                        return null; // O puedes agregar un mensaje de error si lo prefieres
                    }
                }
            }
                // Si contiene "buscar monstruo" seguido de nombre
                if (qLow.Contains("buscar monstruo") || qLow.Contains("buscar a") || qLow.Contains("monstruo llamado") || qLow.Contains("monstruo"))
            {
                // intenta extraer nombre simple (very naive)
                // ejemplo: "Buscar monstruo Pikachu" -> name = Pikachu
                var parts = q.Split(' ');
                if (parts.Length >= 2)
                {
                    // tomar última palabra como nombre (mejorable)
                    var name = parts.Last().Trim();
                    var args = JsonSerializer.Serialize(new { name = name });
                    return ("SearchByName", "monsters/searchByName", args);
                }
            }
            

            // No match
            return null;
        }

        // Lógica para hablar con servidor MCP por WebSocket -> simple JSON-RPC client
        private async Task<string> CallMcpToolAsync(string toolName, string argumentsJson, CancellationToken ct)
        {
            using var ws = new ClientWebSocket();
            var uri = new Uri(_mcpServerWsUrl);
            await ws.ConnectAsync(uri, ct);

            int id = 1;
            var req = new
            {
                jsonrpc = "2.0",
                id = id,
                method = "tools/call",
                @params = new
                {
                    name = toolName,
                    arguments = JsonSerializer.Deserialize<object>(argumentsJson)
                }
            };
            var msg = JsonSerializer.Serialize(req);
            var bytes = Encoding.UTF8.GetBytes(msg + "\n");
            await ws.SendAsync(bytes, WebSocketMessageType.Text, true, ct);

            // read response (simple)
            var buffer = new ArraySegment<byte>(new byte[16 * 1024]);
            var sb = new StringBuilder();
            WebSocketReceiveResult r;
            do
            {
                r = await ws.ReceiveAsync(buffer, ct);
                var chunk = Encoding.UTF8.GetString(buffer.Array!, 0, r.Count);
                sb.Append(chunk);
            } while (!r.EndOfMessage && !r.CloseStatus.HasValue);

            if (r.CloseStatus.HasValue)
            {
                await ws.CloseAsync(ws.CloseStatus.Value, ws.CloseStatusDescription, ct);
            }
            var respText = sb.ToString();
            // parse result.result.content or result
            try
            {
                using var doc = JsonDocument.Parse(respText);
                if (doc.RootElement.TryGetProperty("result", out var res) && res.TryGetProperty("content", out var content))
                {
                    // content is array; return first text block if present
                    if (content.ValueKind == JsonValueKind.Array && content.GetArrayLength() > 0)
                    {
                        foreach (var el in content.EnumerateArray())
                        {
                            if (el.TryGetProperty("type", out var t) && t.GetString() == "text" && el.TryGetProperty("text", out var txt))
                                return txt.GetString();
                        }
                    }
                }
                // fallback: return raw result
                return respText;
            }
            catch
            {
                return respText;
            }
        }
    }
}
