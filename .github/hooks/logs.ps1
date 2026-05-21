param(
    [string]$EventType = "Unknown"
)

$logPath = "C:\Users\adrij\source\repos\piano-learning-tracker\.github\hooks\lab4\agent_log.txt"
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

$data = [Console]::In.ReadToEnd()
$json = $null
try {
    if ($data -and $data.Trim() -ne "") {
        $json = $data | ConvertFrom-Json -ErrorAction SilentlyContinue
    }
} catch {}

$detail = switch ($EventType) {
    "UserPromptSubmit" {
        $prompt = if ($json?.prompt) { $json.prompt -replace "`n", " " -replace "`r", "" } else { "(no content)" }
        if ($prompt.Length -gt 150) { $prompt = $prompt.Substring(0, 150) + "..." }
        "prompt: $prompt"
    }
    "PreToolUse" {
        $tool = if ($json?.tool_name) { $json.tool_name } else { "unknown" }
        $file = if ($json?.tool_input?.path) { " | file: $($json.tool_input.path)" } `
           elseif ($json?.tool_input?.file_path) { " | file: $($json.tool_input.file_path)" } `
           else { "" }
        "tool: $tool$file"
    }
    "PostToolUse" {
        $tool = if ($json?.tool_name) { $json.tool_name } else { "unknown" }
        $status = if ($json?.tool_response?.is_error -eq $true) { "error" } else { "success" }
        "tool: $tool | status: $status"
    }
    default { $data.Trim() }
}

$line = "[$EventType] $timestamp - $detail"
Add-Content -Path $logPath -Value $line -Encoding UTF8
