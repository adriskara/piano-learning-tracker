$logPath = 'C:\Users\adrij\source\repos\piano-learning-tracker\.github\hooks\lab5\agent_log.txt'
$timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'

# Osiguraj da log direktorij postoji
$logDir = Split-Path -Parent $logPath
if (!(Test-Path -Path $logDir)) {
    New-Item -ItemType Directory -Path $logDir -Force | Out-Null
}

try {
    $raw = [Console]::In.ReadToEnd()
    $data = $raw | ConvertFrom-Json -ErrorAction Stop
} catch {
    Add-Content -Path $logPath -Value "[$timestamp] (parse error)" -Encoding UTF8
    exit 0
}

$event = $data.hook_event_name

function TrimText($text, $length = 120) {
    if (-not $text) { return '' }
    $text = $text -replace '\s+', ' '
    $text = $text.Trim()
    if ($text.Length -gt $length) { return $text.Substring(0, $length) + '...' }
    return $text
}

function FormatToolDetail($tool, $input) {
    switch ($tool) {
        'Bash'       { return TrimText($input.command, 80) }
        'PowerShell' { return TrimText($input.command, 80) }
        'Read'       { return TrimText($input.file_path -replace '.*\\repos\\', '~\\', 80) }
        'Write'      { return TrimText($input.file_path -replace '.*\\repos\\', '~\\', 80) }
        'Edit'       { return TrimText($input.file_path -replace '.*\\repos\\', '~\\', 80) }
        'Glob'       { return TrimText($input.pattern, 80) }
        'Grep'       { return '"' + (TrimText($input.pattern, 80)) + '"' }
        default      { return '' }
    }
}

function GetToolSource($data) {
    if ($data.data) { $data = $data.data }
    if ($data.tool_name) { return $data.tool_name }
    if ($data.tool)      { return $data.tool }
    if ($data.model)     { return $data.model }
    return 'unknown'
}

function GetPayloadValue($data, $field) {
    if ($data.$field) { return $data.$field }
    if ($data.data -and $data.data.$field) { return $data.data.$field }
    return $null
}

$line = switch ($event) {
    'UserPromptSubmit' {
        $source = GetToolSource $data
        $prompt = TrimText((GetPayloadValue $data 'prompt') -replace '<ide_opened_file>.*?</ide_opened_file>\s*', '')
        if (-not $prompt) { $prompt = '(empty prompt)' }
        "[PROMPT]    $timestamp  [$source]  $prompt"
    }
    'ResponseGenerated' {
        $source = GetToolSource $data
        $response = GetPayloadValue $data 'response'
        if (-not $response -and (GetPayloadValue $data 'content')) { $response = GetPayloadValue $data 'content' }
        if (-not $response) { $response = '(empty response)' }
        $short = TrimText($response, 100)
        "[RESPONSE]  $timestamp  [$source]  $short"
    }
    'PreToolUse' {
        $tool = $data.tool_name
        if (-not $tool) { $tool = $data.tool }
        $detail = FormatToolDetail $tool $data.tool_input
        if ($detail) { "[TOOL]      $timestamp  $tool  $detail" }
        else          { "[TOOL]      $timestamp  $tool" }
    }
    'PostToolUse' {
        $tool = $data.tool_name
        if (-not $tool) { $tool = $data.tool }
        $ms   = $data.duration_ms
        "[DONE]      $timestamp  $tool  (${ms}ms)"
    }
    'Stop' {
        "[STOP]      $timestamp"
    }
    default {
        "[${event}]  $timestamp"
    }
}

if ($line) {
    Add-Content -Path $logPath -Value $line -Encoding UTF8
}
