# Repository Guidelines

## Project Structure & Module Organization

CC Our Story is an ASP.NET Core Razor Pages application with SQLite. Preserve the dependency direction: `Web → Services → Data → Core`.

- `src/OurStory.Core/`: domain entities, models, configuration contracts, and utilities.
- `src/OurStory.Data/`: EF Core context, entity mappings, and `Migrations/`.
- `src/OurStory.Services/`: business features organized into service, interface, and model folders.
- `src/OurStory.Web/`: public `Pages/`, `Areas/Admin/Pages/`, API endpoints, and infrastructure. Static CSS/JavaScript lives in `wwwroot/assets/`.
- `tests/OurStory.Tests/`: xUnit tests and shared `TestDoubles.cs`.
- `.github/workflows/` and `docker/`: CI, releases, and container packaging.

## Build, Test, and Development Commands

Run from the repository root with the .NET 10 SDK selected by `global.json`:

- `dotnet restore OurStory.sln`: restore dependencies.
- `dotnet build OurStory.sln -c Release --no-restore`: compile with analyzers and warnings treated as errors.
- `dotnet test OurStory.sln -c Release --no-build`: run tests after building.
- `dotnet watch --project src/OurStory.Web`: start development with hot reload at `http://localhost:5080`.
- `dotnet publish src/OurStory.Web -c Release -o dist`: produce deployment output.
- `dotnet tool restore`, then `dotnet dotnet-ef migrations has-pending-model-changes --project src/OurStory.Data/OurStory.Data.csproj`: check migration consistency.
- `node --check <path-to-file.js>`: check changed JavaScript syntax, matching CI.

## Coding Style & Naming Conventions

Follow `.editorconfig`: four-space C# indentation, UTF-8, CRLF, file-scoped namespaces, and opening braces on the same line. Prefer explicit types per configuration. Use PascalCase for types and members, `I`-prefixed interfaces, and `Async` suffixes for asynchronous service methods. Preserve copyright headers. Shared build settings and package versions belong in `Directory.Build.props`.

## Testing Guidelines

Use xUnit `[Fact]`/`[Theory]` in `<Feature>Tests.cs`; existing tests use descriptive Chinese method names. Reuse test doubles and database harnesses; cover relevant behavior and regressions. No numeric coverage threshold is configured. Include EF migrations for model changes.

## Commit & Pull Request Guidelines

Recent commits use an emoji plus a concise Chinese summary, such as `🐛 修复…` or `✨ 新增…`; merged entries include PR numbers. Follow `.github/pull_request_template.md`: describe purpose, impact, performance, compatibility, and related issues. Include screenshots/GIFs for UI changes and report validation. Register new or changed persisted image URL references in `MediaLibraryService`.

## Security & Configuration

Keep `.env`, `ourstory.json`, databases, uploads, and keys out of commits. Runtime data defaults to `App_Data/`; `OURSTORY_DATA_DIR` overrides its location. Never include generated account passwords or service credentials in PRs.
