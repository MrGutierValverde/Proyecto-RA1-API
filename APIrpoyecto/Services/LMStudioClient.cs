using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace APIrpoyecto.Services
{
    public class LMStudioClient
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _cfg;

        public LMStudioClient(HttpClient http, IConfiguration cfg)
        {
            _http = http;
            _cfg = cfg;

            var url = _cfg["LMStudio:Url"] ?? "http://localhost:1234";
            _http.BaseAddress = new Uri(url);
        }

        /// <summary>
        /// Envía una solicitud al endpoint /v1/responses de LM Studio
        /// devolviendo el texto generado por el modelo.
        /// </summary>
        public async Task<string> RequestToolDecisionAsync(string userQuery, string toolsDescription)
        {
            var prompt = $@"
Eres un sistema que decide qué herramienta del MCP usar.

Herramientas disponibles:
{toolsDescription}

Instrucciones:
Devuelve SOLO un JSON con el formato:
{{
  ""tool"": ""<tool_name o null>"",
  ""arguments"": {{ ... }},
  ""explain"": ""breve explicación""
}}

Usuario: {userQuery}
";

            var payload = new
            {
                model = _cfg["LMStudio:Model"] ?? "meta-llama-3.1-8b-instruct",
                input = new[]
                {
                    new { role = "system", content = "Eres un experto en selección de herramientas MCP." },
                    new { role = "user", content = prompt }
                }
            };

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var resp = await _http.PostAsync("/v1/responses", content);
            resp.EnsureSuccessStatusCode();

            var txt = await resp.Content.ReadAsStringAsync();

            /*
             LM Studio devuelve formato OpenAI-like:

             {
               "id": "...",
               "object": "chat.completion",
               "choices": [
                 {
                   "message": { "role": "assistant", "content": "texto..." }
                 }
               ]
             }
            */

            using var doc = JsonDocument.Parse(txt);
            string output = doc
                .RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return output;
        }
    }
}
