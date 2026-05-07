using Microsoft.AspNetCore.Mvc;
using PianoLearningTracker.Services;

namespace PianoLearningTracker.Controllers
{
    [Route("asistent")]
    public class AiController : Controller
    {
        private readonly IAnthropicService _anthropic;
        private readonly IAiLoggerService _aiLogger;

        public AiController(IAnthropicService anthropic, IAiLoggerService aiLogger)
        {
            _anthropic = anthropic;
            _aiLogger = aiLogger;
        }

        // URL: /asistent
        [Route("")]
        public IActionResult Index() => View();

        // URL: /asistent/pitaj
        [HttpPost]
        [Route("pitaj")]
        public async Task<IActionResult> Ask([FromBody] AskRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Prompt))
                return BadRequest(new { error = "Prompt ne smije biti prazan." });

            try
            {
                var sessionId = HttpContext.Session.Id;
                var result = await _anthropic.AskAsync(request.Prompt, sessionId);

                return Json(new
                {
                    reply = result.Content,
                    model = result.Model,
                    inputTokens = result.InputTokens,
                    outputTokens = result.OutputTokens
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // URL: /asistent/zapisi
        [Route("zapisi")]
        public async Task<IActionResult> Logs(string? sessionId)
        {
            var logs = await _aiLogger.GetLogsAsync(sessionId);
            return Json(logs);
        }

        // URL: /asistent/info
        [Route("info")]
        public async Task<IActionResult> LogInfo()
        {
            var logs = await _aiLogger.GetLogsAsync();
            var path = _aiLogger.LogPath;
            var exists = System.IO.File.Exists(path);
            long size = exists ? new System.IO.FileInfo(path).Length : 0;

            return Json(new
            {
                path,
                exists,
                sizeBytes = size,
                entryCount = logs.Count,
                lastEntryAt = logs.FirstOrDefault()?.Timestamp
            });
        }
    }

    public class AskRequest
    {
        public string Prompt { get; set; } = string.Empty;
    }
}
