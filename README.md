# ConnectorRepo – NOVA Advanced Demo

A small ASP.NET Core minimal API (C#, .NET 8) used in the NOVA Advanced session **"Teaching Copilot to Learn."** It's a pipeline maintenance work-order tracker.

## Run it

```bash
dotnet run --project src/WorkOrders.Api
```

Then try:

```bash
curl http://localhost:5229/workorders
curl -X POST http://localhost:5229/workorders -H "Content-Type: application/json" \
  -d '{"pipelineSegment":"SEG-101","description":"Replace corroded valve","priority":"High"}'
curl -X PUT http://localhost:5229/workorders/1/status -H "Content-Type: application/json" \
  -d '{"status":"Completed"}'
```

## Endpoints

| Method | Route | Purpose |
|---|---|---|
| GET | `/workorders?status=Open` | List work orders, optional status filter |
| GET | `/workorders/{id}` | Get one work order |
| POST | `/workorders` | Create a work order |
| PUT | `/workorders/{id}/status` | Update status (Open, InProgress, Completed, Cancelled) |

## Demo phases

Each phase is tagged so you can step through it live.

1. **`phase-1-baseline`** – Built with no Copilot instructions, using default choices (classes for DTOs, synchronous repository, interpolated log messages, plain-text validation errors, endpoints inline in `Program.cs`). *← you are here*
2. **`phase-2-rules-learned`** – Refactor to the team's house rules and write them into `.github/copilot-instructions.md`.
3. **`phase-3-new-feature`** – In a fresh chat, add `GET /workorders/overdue` and an `Assignee` field; Copilot follows the rules unprompted.
4. **`phase-4-self-improving`** – Add a rule telling Copilot to update its own instructions whenever it's corrected.
