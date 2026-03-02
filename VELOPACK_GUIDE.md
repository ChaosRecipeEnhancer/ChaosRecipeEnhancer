# Velopack Auto-Update Integration Guide

## Overview

ChaosRecipeEnhancer now uses [Velopack](https://docs.velopack.io/) for automatic updates, replacing the old manual MSI installer workflow. Users get seamless in-app updates pulled from GitHub Releases.

## Architecture

### How It Works

1. **On app startup**, `VelopackApp.Build().Run()` runs before any WPF code — this handles Velopack's internal install/uninstall/update hooks
2. **After the UI loads**, `SettingsViewModel.CheckForUpdateAsync()` silently queries your GitHub releases for a newer version
3. **If an update exists**, a button appears in the footer: _"vX.Y.Z available — click to update"_
4. **User clicks** → download happens in the background → app restarts on the new version

### Files Changed

| File | What Changed |
|---|---|
| `ChaosRecipeEnhancer.UI.csproj` | Added `Velopack` NuGet. Changed `App.xaml` from `ApplicationDefinition` → `Page` for custom `Main`. |
| `App.xaml.cs` | Added `[STAThread] Main()` with `VelopackApp.Build().Run()` as first line. |
| `Services/IUpdateManagerService.cs` | Interface: `CheckForUpdateAsync`, `DownloadUpdateAsync`, `ApplyUpdateAndRestart`. |
| `Services/UpdateManagerService.cs` | Velopack implementation using `GithubSource` pointed at your repo. |
| `Configuration/ServiceConfiguration.cs` | Registered `IUpdateManagerService` as singleton. |
| `Windows/SettingsViewModel.cs` | Replaced old update check with `IUpdateManagerService`. Added `DownloadAndApplyUpdateCommand`. |
| `Windows/SettingsWindow.xaml.cs` | Deleted 60 lines of manual GitHub API version checking. Uses service now. |
| `Windows/SettingsWindow.xaml` | Footer upgrade arrow is now a button with download status text. |

## Prerequisites

### Repository

The git remote should point to the current repo URL:

```
origin  https://github.com/ChaosRecipeEnhancer/ChaosRecipeEnhancer.git
```

If your local clone still points to `EnhancePoEApp`, update it:

```bash
git remote set-url origin https://github.com/ChaosRecipeEnhancer/ChaosRecipeEnhancer.git
```

### Install the `vpk` CLI (manual releases only)

```bash
dotnet tool install -g vpk
```

> **Tip:** Use the same version of `vpk` as the `Velopack` NuGet package in your project for compatibility. Not needed if you only release via GitHub Actions.
## Versioning

**Important:** Velopack requires **semver** format (`X.Y.Z`), not the 4-part version previously used (`3.27.1000.0`).

The version lives in 3 files with different formats:

| File | Format | Example |
|---|---|---|
| `CreAppConfig.cs` (`VersionText`) | `X.Y.Z` | `3.28.1000` |
| `AssemblyInfo.cs` (`AssemblyVersion` / `AssemblyFileVersion`) | `X.Y.Z.0` | `3.28.1000.0` |
| `.csproj` (`ApplicationVersion`) | `X.Y` | `3.28` |
| `.csproj` (`ApplicationRevision`) | Derived from patch | `10` |

`ApplicationRevision` = `floor(patch/1000)*10 + patch%1000`. Examples: `1000`→`10`, `1001`→`11`, `2000`→`20`.

The bump script handles all of this automatically.
### Bumping the version

Use the included script to update all 3 files at once:

```powershell
.\scripts\bump-version.ps1 3.28.1000
```

This updates all files, shows you what changed, and prints next steps. Run it on your release branch before merging to develop.
## Release Workflow (Automated via GitHub Actions)

Releases are **fully automated**. When you merge to `main`, the pipeline handles everything.

### Your flow

```
feature/* ──► release/vX.Y.Z ──► develop ──► main
                                              │
                                              ▼
                                    GitHub Actions runs:
                                    build → test → vpk pack → vpk upload
                                              │
                                              ▼
                                    GitHub Release created
                                    with Setup.exe + update packages
```

### What you do for each release

1. **Create release branch** from `develop` (e.g., `release/v3.28.1000`)
2. **Bump version**: `.\scripts\bump-version.ps1 3.28.1000`
3. **Commit**: `git commit -am 'chore: bump version to 3.28.1000'`
4. **Merge** release branch → `develop` → `main` (via PRs)
5. **Done.** The CI/CD pipeline creates the GitHub release with all Velopack artifacts

### What the pipeline does automatically

1. Extracts version from `CreAppConfig.VersionText`
2. Checks if that version was already released (idempotent — safe to re-merge)
3. Runs `dotnet test` to validate the build
4. Runs `dotnet publish` for self-contained `win-x64`
5. Downloads the previous GitHub release (for delta package generation)
6. Runs `vpk pack` to produce installer + update packages
7. Runs `vpk upload github` to create the GitHub release with all assets

### Workflow files

| File | Trigger | Purpose |
|---|---|---|
| `.github/workflows/ci.yml` | PRs to `develop`/`main`, pushes to `develop` | Build + test (catches failures early) |
| `.github/workflows/release.yml` | Push to `main` | Build + package + publish GitHub release |

### Manual release (fallback)

If you ever need to release manually:

```bash
dotnet publish src\App\ChaosRecipeEnhancer.UI\ChaosRecipeEnhancer.UI.csproj ^
  -c Release --self-contained -r win-x64 -o .\publish

vpk pack ^
  --packId ChaosRecipeEnhancer ^
  --packVersion X.Y.Z ^
  --packDir .\publish ^
  --mainExe ChaosRecipeEnhancer.exe ^
  --icon src\App\ChaosRecipeEnhancer.UI\Assets\Icons\CREIcon.ico

vpk upload github ^
  --repoUrl https://github.com/ChaosRecipeEnhancer/ChaosRecipeEnhancer ^
  --tag vX.Y.Z ^
  --token YOUR_GITHUB_TOKEN ^
  --releaseName "vX.Y.Z" ^
  --publish
```

### Velopack output files

```
Releases/
├── ChaosRecipeEnhancer-X.Y.Z-full.nupkg    # Full update package
├── ChaosRecipeEnhancer-X.Y.Z-delta.nupkg   # Delta (if previous version exists)
├── ChaosRecipeEnhancer-Setup.exe            # First-time installer
├── ChaosRecipeEnhancer-Portable.zip         # Portable version
├── releases.win.json                        # Release feed (used by UpdateManager)
└── assets.win.json                          # Build asset manifest
```
## Testing Locally (End-to-End)

### Test 1: Verify the app starts correctly

Just build and run in Debug. The update check will run but won't find anything (no Velopack install context). This is expected — you'll see a warning in the Serilog logs, which is harmless.

### Test 2: Full update cycle

1. **Create v1 (the "installed" version):**

```bash
dotnet publish src\App\ChaosRecipeEnhancer.UI\ChaosRecipeEnhancer.UI.csproj -c Release --self-contained -r win-x64 -o .\publish

vpk pack --packId ChaosRecipeEnhancer --packVersion 3.27.0 --packDir .\publish --mainExe ChaosRecipeEnhancer.exe --icon src\App\ChaosRecipeEnhancer.UI\Assets\Icons\CREIcon.ico
```

2. **Install v1:** Run `Releases\ChaosRecipeEnhancer-Setup.exe`

3. **Bump version** in `CreAppConfig.cs` and `AssemblyInfo.cs` to `3.28.0`

4. **Create v2:**

```bash
dotnet publish src\App\ChaosRecipeEnhancer.UI\ChaosRecipeEnhancer.UI.csproj -c Release --self-contained -r win-x64 -o .\publish

vpk pack --packId ChaosRecipeEnhancer --packVersion 3.28.0 --packDir .\publish --mainExe ChaosRecipeEnhancer.exe --icon src\App\ChaosRecipeEnhancer.UI\Assets\Icons\CREIcon.ico
```

5. **Upload v2** to GitHub (or use a local test server)

6. **Relaunch v1** → should show update button → click → downloads → restarts on v2

## Migration Note for Existing Users

The first Velopack-based release requires existing `.msi` users to **manually install** `ChaosRecipeEnhancer-Setup.exe` one time. After that, all future updates are automatic. Mention this prominently in your release notes.

## Future Improvements

- [x] ~~Add a GitHub Actions workflow to automate releases~~ (done)
- [ ] Add download progress percentage to the update button
- [ ] Consider adding a `--channel` for beta/stable release tracks
- [ ] Code-sign the Velopack packages for Windows SmartScreen trust
- [ ] Add release notes auto-generation from commit messages
