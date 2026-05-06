# Claude Code AI Logging Setup

Ovaj dokument opisuje kako aktivirati automatsko logiranje AI interakcija u vašem projektu.

## Trenutno stanje

✅ **Web aplikacija**: Automatsko logiranje je već aktivno za AI interakcije kroz web interface
   - Logovi se spremaju u: `PianoLearningTracker/App_Data/ai_logs/ai_usage.jsonl`
   - Sadrži: timestamp, role, content, model, token usage, session ID

✅ **Claude Code hooks**: Konfigurirani su hookovi u `.claude/settings.json`
   - Logira: User prompts, Tool usage, AI responses
   - Logovi se spremaju u: `.github/hooks/lab3/agent_log.txt`

## Kako aktivirati logging

### 1. Za Claude Code / Claude Desktop:

Hookovi su već konfigurirani u `.claude/settings.json`. Trebate osigurati da:

1. Koristite Claude Code ili Claude Desktop u ovom direktoriju
2. Hookovi će se automatski aktivirati kada koristite AI

### 2. Za GitHub Copilot u VS Code:

Nažalost, GitHub Copilot nema native hook sustav. Za logging trebate:

1. Ručno bilježiti sesije u zasebnu datoteku
2. Ili koristiti ekstenzije koje prate AI usage

## Testiranje hookova

Za testiranje da li hookovi rade, možete pokrenuti:

```batch
# Test user prompt logging
echo {"prompt": "Test prompt"} | powershell.exe -ExecutionPolicy Bypass -Command "...[inline command]..."

# Ili koristiti batch file
.github\hooks\test_logging.bat
```

## Format logova

```
[EventType] YYYY-MM-DD HH:mm:ss - detail
```

Primjeri:
```
[UserPrompt] 2026-04-24 10:30:15 - prompt: Create a new controller for students
[ToolUse] 2026-04-24 10:30:16 - tool: run_in_terminal
[ToolResult] 2026-04-24 10:30:17 - tool: run_in_terminal
[Response] 2026-04-24 10:30:18 - response: 245 chars
```

## Troubleshooting

Ako hookovi ne rade:
1. Provjerite da li je `.claude/settings.json` ispravno formatiran
2. Testirajte PowerShell komande ručno
3. Provjerite dozvole za pisanje u log direktorij
4. Restartujte Claude Code/ Desktop