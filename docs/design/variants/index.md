# Application Hosts

Momentum's hosted browser application (`apps/web` and `Momentum.Service`) uses Azure Storage as the system of record. The browser signs in through the configured identity provider and uses the HTTP API.

The service also retains its MCP capability surface for agent clients, with backend authorization shared across entry points. Shared UI, SDK, and domain packages remain independent of persistence.

Additional hosts require an explicit design decision and must preserve lifecycle, authorization, and visibility rules. See `docs/design/platform/frontend/applications.md` and `docs/design/platform/mcp/index.md`.
