using System.Text.Json;
using PianoLearningTracker.Models;

namespace PianoLearningTracker.Services
{
    public interface IAiLoggerService
    {
        Task LogAsync(AiLog entry);
        Task<List<AiLog>> GetLogsAsync(string? sessionId = null);
        string LogPath { get; }
    }

    public class AiLoggerService : IAiLoggerService
    {
        private readonly string _logPath;
        private readonly SemaphoreSlim _lock = new(1, 1);
        private readonly ILogger<AiLoggerService> _logger;

        public string LogPath => _logPath;

        public AiLoggerService(
            IConfiguration config,
            IWebHostEnvironment env,
            ILogger<AiLoggerService> logger)
        {
            _logger = logger;

            var folder = Path.Combine(
                env.ContentRootPath,
                config["AiLogger:Path"] ?? "App_Data/ai_logs"
            );
            Directory.CreateDirectory(folder);
            _logPath = Path.Combine(folder, "ai_usage.jsonl");

            // Osiguraj da datoteka postoji već pri pokretanju — olakšava provjeru
            if (!File.Exists(_logPath))
            {
                try
                {
                    using var _ = File.Create(_logPath);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ne mogu kreirati AI log datoteku na putanji {Path}", _logPath);
                }
            }

            _logger.LogInformation("AI logovi se zapisuju u {Path}", _logPath);
        }

        public async Task LogAsync(AiLog entry)
        {
            var line = JsonSerializer.Serialize(entry,
                new JsonSerializerOptions { WriteIndented = false })
                + Environment.NewLine;

            await _lock.WaitAsync();
            try
            {
                await File.AppendAllTextAsync(_logPath, line);
            }
            catch (Exception ex)
            {
                // Ne rušimo AI poziv ako log ne može zapisati na disk
                _logger.LogError(ex, "Neuspjelo zapisivanje AI loga u {Path}", _logPath);
            }
            finally
            {
                _lock.Release();
            }
        }

        public async Task<List<AiLog>> GetLogsAsync(string? sessionId = null)
        {
            if (!File.Exists(_logPath))
                return new List<AiLog>();

            var lines = await File.ReadAllLinesAsync(_logPath);

            var logs = lines
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Select(l =>
                {
                    try { return JsonSerializer.Deserialize<AiLog>(l); }
                    catch { return null; }
                })
                .Where(l => l != null)
                .Cast<AiLog>()
                .OrderByDescending(l => l.Timestamp)
                .ToList();

            if (sessionId != null)
                logs = logs.Where(l => l.SessionId == sessionId).ToList();

            return logs;
        }
    }
}
