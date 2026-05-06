using System.Text;
using System.Text.Json;
using PianoLearningTracker.Models;

namespace PianoLearningTracker.Services
{
    public class AnthropicRequest
    {
        public string model { get; set; } = "claude-sonnet-4-6";
        public int max_tokens { get; set; } = 1024;
        public List<AnthropicMessage> messages { get; set; } = new();
    }

    public class AnthropicMessage
    {
        public string role { get; set; } = string.Empty;
        public string content { get; set; } = string.Empty;
    }

    public class AnthropicResponse
    {
        public string id { get; set; } = string.Empty;
        public string model { get; set; } = string.Empty;
        public List<AnthropicContent>? content { get; set; }
        public AnthropicUsage? usage { get; set; }
    }

    public class AnthropicContent
    {
        public string type { get; set; } = string.Empty;
        public string text { get; set; } = string.Empty;
    }

    public class AnthropicUsage
    {
        public int input_tokens { get; set; }
        public int output_tokens { get; set; }
    }

    public class AiResult
    {
        public string Content { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int InputTokens { get; set; }
        public int OutputTokens { get; set; }
    }

    public interface IAnthropicService
    {
        Task<AiResult> AskAsync(string prompt, string? sessionId = null);
    }

    public class AnthropicService : IAnthropicService
    {
        private readonly HttpClient _http;
        private readonly IAiLoggerService _logger;
        private readonly string _apiKey;
        private const string Model = "claude-sonnet-4-6";

        public AnthropicService(
            IHttpClientFactory factory,
            IAiLoggerService logger,
            IConfiguration config)
        {
            _http = factory.CreateClient();
            _logger = logger;
            _apiKey = config["Anthropic:ApiKey"]
                ?? throw new InvalidOperationException(
                    "Nedostaje Anthropic:ApiKey u konfiguraciji. Postavi ga pomoću User Secrets.");
        }

        public async Task<AiResult> AskAsync(string prompt, string? sessionId = null)
        {
            await _logger.LogAsync(new AiLog
            {
                Role = "user",
                Content = prompt,
                SessionId = sessionId
            });

            var errorLogged = false;
            try
            {
                var body = new AnthropicRequest
                {
                    model = Model,
                    max_tokens = 1024,
                    messages = new List<AnthropicMessage>
                    {
                        new() { role = "user", content = prompt }
                    }
                };

                var json = JsonSerializer.Serialize(body);
                var httpReq = new HttpRequestMessage(HttpMethod.Post, "https://api.anthropic.com/v1/messages");
                httpReq.Headers.Add("x-api-key", _apiKey);
                httpReq.Headers.Add("anthropic-version", "2023-06-01");
                httpReq.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var httpResp = await _http.SendAsync(httpReq);
                var respJson = await httpResp.Content.ReadAsStringAsync();

                if (!httpResp.IsSuccessStatusCode)
                {
                    await _logger.LogAsync(new AiLog
                    {
                        Role = "error",
                        Content = $"Anthropic API greška {(int)httpResp.StatusCode} {httpResp.StatusCode}: {respJson}",
                        Model = Model,
                        SessionId = sessionId,
                        Metadata = new Dictionary<string, string>
                        {
                            ["statusCode"] = ((int)httpResp.StatusCode).ToString()
                        }
                    });
                    errorLogged = true;
                    throw new Exception($"Anthropic API greška {httpResp.StatusCode}: {respJson}");
                }

                var parsed = JsonSerializer.Deserialize<AnthropicResponse>(respJson)
                    ?? throw new Exception("Neispravan odgovor od Anthropic API-ja");

                var replyText = parsed.content?.FirstOrDefault()?.text ?? string.Empty;

                await _logger.LogAsync(new AiLog
                {
                    Role = "assistant",
                    Content = replyText,
                    Model = parsed.model,
                    InputTokens = parsed.usage?.input_tokens,
                    OutputTokens = parsed.usage?.output_tokens,
                    SessionId = sessionId
                });

                return new AiResult
                {
                    Content = replyText,
                    Model = parsed.model,
                    InputTokens = parsed.usage?.input_tokens ?? 0,
                    OutputTokens = parsed.usage?.output_tokens ?? 0
                };
            }
            catch (Exception ex)
            {
                if (!errorLogged)
                {
                    await _logger.LogAsync(new AiLog
                    {
                        Role = "error",
                        Content = $"Iznimka prilikom poziva Anthropic API-ja: {ex.GetType().Name}: {ex.Message}",
                        Model = Model,
                        SessionId = sessionId
                    });
                }
                throw;
            }
        }
    }
}
