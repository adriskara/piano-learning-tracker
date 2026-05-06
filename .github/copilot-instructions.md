# GitHub Copilot Instructions — Piano Learning Tracker

This is an ASP.NET Core MVC application for tracking piano student progress. Use the guidance below when generating code suggestions for this project.

## Project overview
- Framework: ASP.NET Core MVC (.NET)
- Views: Razor (.cshtml) with strongly typed view models
- Styling: Custom CSS in `wwwroot/css/site.css` — avoid plain Bootstrap defaults
- Data: Static mock data (no database); read-only views only (no Create/Edit forms)
- Users: Piano students (children, teens, adults) and piano teachers

## UX / UI guidelines

When generating Razor views or CSS:

- Use a **left sidebar layout** defined in `_Layout.cshtml` with `<aside>` for navigation and a main content wrapper
- Main grid: `display: grid; grid-template-columns: 260px 1fr; gap: 24px`
- Use **CSS variables** for the palette: `--bg`, `--surface`, `--text`, `--accent`, `--accent-soft`, `--success`, `--warning`
- Cards: `border-radius: 24px`, soft `box-shadow`, `background: var(--surface)`
- Hover states: `transform: translateY(-2px)` with increased shadow
- Transitions: `transition: transform 180ms ease, box-shadow 180ms ease`
- Progress bars: `height: 12px`, rounded pill shape
- Color palette: soft blue/purple or warm accents; green/gold for progress highlights
- Add subtle music-themed cues (piano key separators, note icons) sparingly

## Page patterns

- **Index/list views**: card grid layout, each card links to Details via `asp-action` and `asp-route-id`
- **Details views**: summary card at top, grouped detail cards below, breadcrumb header
- **Dashboard**: metric summary cards, today's focus, practice streaks, teacher activity panel
- Do **not** generate Create/Edit forms — read-only views only

## Razor conventions

- Use `@model IEnumerable<T>` for list views
- Use `@model T` for detail views
- Use `asp-controller`, `asp-action`, `asp-route-id` tag helpers for all links
- Import common namespaces in `_ViewImports.cshtml`

## CSS conventions

- All custom styles go in `wwwroot/css/site.css`
- Use utility classes: `.app-shell`, `.sidebar-panel`, `.content-panel`, `.dashboard-card`, `.metric-badge`, `.progress-line`
- Keep the sidebar responsive: collapse to top drawer on mobile

## Code quality

- Use strongly typed view models; avoid `ViewBag` for complex data
- Keep Razor views maintainable: header, summary cards, section cards, footer
- Name controllers and views consistently with existing project structure
- Design should be explainable during an oral code review

## AI Agents

For UI/UX design and layout tasks, use the **ux-designer** agent:

- **When to use**: Designing new Razor views, creating or modifying CSS styles, improving layouts, styling components, navigation structure, or anything related to visual design
- **How to use**: Simply mention "ux-designer" in your request or use `@ux-designer` if your client supports agent mentions
- **Example prompts**:
  - "Use ux-designer to create a new Details view for lessons with improved card layout"
  - "ux-designer: improve the Teacher Index page styling and add visual hierarchy"
  - "@ux-designer doradi Student dashboard s boljim metrikom kartama"
- **Agent capabilities**: Reads/writes Razor views, modifies CSS, understands the project's custom design system, ensures consistency with existing UI patterns
