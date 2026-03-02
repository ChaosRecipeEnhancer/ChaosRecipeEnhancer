# Agent Instructions — ChaosRecipeEnhancer

## Project Overview

ChaosRecipeEnhancer (CRE) is a WPF desktop tool for Path of Exile players. It overlays in-game to track chaos/regal/exalted/chance recipe progress, manipulates loot filters, and talks to the PoE API via OAuth.

- **Framework**: .NET 8, WPF (`net8.0-windows`)
- **Language**: C#
- **Repo**: `https://github.com/ChaosRecipeEnhancer/ChaosRecipeEnhancer`
- **Solution**: `ChaosRecipeEnhancer.sln`

## Project Structure

```
src/
  App/
    ChaosRecipeEnhancer.UI/           # Main WPF application
      App.xaml / App.xaml.cs           # Entry point (custom Main for Velopack)
      Configuration/                   # DI setup (ServiceConfiguration.cs)
      Models/                          # Domain models, enums, config
        Config/CreAppConfig.cs         # App constants, VERSION LIVES HERE
      Services/                        # Business logic (API, auth, updates, hotkeys)
      UserControls/                    # MVVM user controls + ViewModels
      Windows/                         # Window code-behind + ViewModels
      Properties/                      # AssemblyInfo.cs, Settings.settings, Resources
      Native/                          # P/Invoke, keyboard hooks
      Assets/                          # Icons, images, themes, fonts
    ChaosRecipeEnhancer.UI.Setup/      # Legacy .vdproj installer (being replaced by Velopack)
    ChaosRecipeEnhancer.UI.Tests/      # Unit tests (xUnit)
  Backend/                             # Backend test app (not part of main build)
scripts/
  bump-version.ps1                     # Version bump script (updates all 3 version files)
.github/workflows/
  ci.yml                               # Build + test on PRs
  release.yml                          # Automated release on push to main
```

## Architecture Patterns

- **MVVM** with CommunityToolkit.Mvvm (`ObservableObject`, `RelayCommand`, `AsyncRelayCommand`)
- **DI** via `Microsoft.Extensions.DependencyInjection` + `CommunityToolkit.Mvvm.DependencyInjection.Ioc`
- **Services** registered in `ServiceConfiguration.cs` — singletons for stateful services, transient for ViewModels
- **Base ViewModel**: `CreViewModelBase` extends `ObservableObject`, exposes `Settings.Default` as `GlobalUserSettings`
- **User settings**: `Properties/Settings.settings` (WPF Settings infrastructure), accessed via `Settings.Default`
- **Logging**: Serilog with file + debug sinks
- **HTTP**: `IHttpClientFactory` with Polly retry policies
- **Auto-updates**: Velopack with `GithubSource` — see `VELOPACK_GUIDE.md`

## Branching Model

```
feature/*  ──► release/vX.Y.Z ──► develop ──► main
hotfix/*   ──►                     develop ──► main
```

- `main` — production. Pushing here triggers the release pipeline.
- `develop` — integration branch. PRs from release branches land here first.
- `release/vX.Y.Z` — release prep branches. Version bump happens here.
- `feature/*`, `fix/*`, `hotfix/*`, `bugfix/*` — topic branches off develop.

## Versioning

Version is stored in **3 files** with different formats:

| File | Pattern | Example |
|---|---|---|
| `src/.../Models/Config/CreAppConfig.cs` | `VersionText = "X.Y.Z"` | `"3.28.1000"` |
| `src/.../Properties/AssemblyInfo.cs` | `AssemblyVersion("X.Y.Z.0")` | `"3.28.1000.0"` |
| `src/.../ChaosRecipeEnhancer.UI.csproj` | `<ApplicationVersion>X.Y</ApplicationVersion>` | `3.28` |
| `src/.../ChaosRecipeEnhancer.UI.csproj` | `<ApplicationRevision>N</ApplicationRevision>` | `10` |

The `ApplicationRevision` is derived from the patch number: `floor(patch/1000)*10 + patch%1000`.
Examples: `1000`→`10`, `1001`→`11`, `2000`→`20`.

**NEVER edit these manually.** Use the bump script:

```powershell
.\scripts\bump-version.ps1 3.28.1000
```

The CI pipeline (`release.yml`) extracts the version from `CreAppConfig.VersionText` to tag the GitHub release.

---

## Release Preparation Procedure

When the user asks to "prep a release", "bump version", or "get ready for release", follow these steps exactly:

### Step 1: Confirm the version number

Ask the user what the new version should be if they didn't specify. The format is `X.Y.Z` (e.g., `3.28.1000`). Reference recent release branches for naming conventions:
- Patch: `3.27.1000` → `3.27.1001`
- Minor: `3.27.1000` → `3.28.1000`

### Step 2: Run the bump script

```powershell
.\scripts\bump-version.ps1 <VERSION>
```

Verify the output shows all 3 files were updated.

### Step 3: Build and test

```bash
dotnet build ChaosRecipeEnhancer.sln -c Release
dotnet test ChaosRecipeEnhancer.sln -c Release
```

Both must pass. Fix any failures before proceeding.

### Step 4: Summarize for the user

Tell them:
1. What version was set
2. Which files were modified
3. Suggest the commit message: `chore: bump version to X.Y.Z`
4. Remind them of the merge flow: release branch → develop → main
5. Confirm that GitHub Actions will handle the rest after merge to main

### DO NOT:
- Manually edit version files (use the script)
- Create git tags (the CI pipeline does this)
- Run `vpk pack` or `vpk upload` (the CI pipeline does this)
- Push to main directly (always via PR)

---

## Common Tasks Reference

### Adding a new service
1. Create `IMyService.cs` interface in `Services/`
2. Create `MyService.cs` implementation in `Services/`
3. Register in `Configuration/ServiceConfiguration.cs`
4. Inject via constructor in the ViewModel that needs it

### Adding a new settings form
1. Create UserControl + ViewModel in `UserControls/SettingsForms/`
2. Register ViewModel in `ServiceConfiguration.ConfigureViewModels()`
3. Add the form to the appropriate tab in `SettingsWindow.xaml`

### Build commands
```bash
dotnet build ChaosRecipeEnhancer.sln           # Debug build
dotnet build ChaosRecipeEnhancer.sln -c Release # Release build
dotnet test ChaosRecipeEnhancer.sln             # Run tests
```

### Key files to know
- `CreAppConfig.cs` — app-wide constants (version, instance name, GitHub org/repo)
- `SiteUrls.cs` — external URLs (GitHub releases, Discord, etc.)
- `ServiceConfiguration.cs` — all DI registrations
- `App.xaml.cs` — entry point, Velopack bootstrap, single-instance logic, DI init
- `SettingsWindow.xaml.cs` — main window, tray icon, hotkeys
- `SettingsViewModel.cs` — main window ViewModel, update checking

### External docs
- `VELOPACK_GUIDE.md` — full auto-update and release pipeline documentation
- `CONTRIBUTING.md` — contributor guidelines
- `CHANGELOG.md` — release history
