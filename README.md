# Private Eye Pierce & The Ink Spill

A 2D point-and-click sidescroller adventure game built in Unity, set in a black-and-white comic world.

## About

**Private Eye Pierce & The Ink Spill** follows P.I. Pierce, a washed-up detective locked in his office building. As a colorful ink spill spreads through the city, Pierce must explore different rooms, interact with surreal characters, and solve puzzles to escape.

**Elevator Pitch:** *"A washed-up detective escapes the monotony of his grayscale office as color invades the outside world."*

## Project Structure

All Unity assets are located in the `Private Eye Pierce & The Ink Spill` folder:

```
/Private Eye Pierce & The Ink Spill
  /Assets
    /Scenes            # Unity Scenes
    /Scripts           # Gameplay scripts
    /Runtime           # Unity runtime scripts (assemblies as needed)
    /Editor            # Unity editor scripts (separate assembly)
    /Resources         # Unity ScriptableObjects/Data
    /Shaders           # Unity HLSL/ShaderGraph assets
    /Audio             # Audio assets
    /Art               # Models/Textures/Materials
    /Tests             # Tests
  /Packages            # Unity package manifests
  /ProjectSettings     # Unity project settings
/Docs                  # Design docs (at root level)
```

### Current Implementation (Graybox)

The **OfficeFloor** scene (`Assets/Scenes/OfficeFloor.unity`) is the main playable graybox. It uses a single-scene room system:

- **Rooms** are GameObjects under `Game/Rooms`; only one room’s `RoomRoot` is active at a time. Switching rooms enables the new room, teleports Pierce, snaps the camera, and updates walk bounds.
- **Scripts** in `Assets/Scripts/`: `RoomManager` (transitions, spawn rules), `RoomDefinition` (per-room data), `ClickController2D` (interact vs move, door cursor), `DoorInteractable`, `PlayerMover2D`, `PageTurnTransition` (optional page-turn effect), `IInteractable`.
- **Spawn rules**: Game start uses the initial room’s Spawn_Left. When entering via a door, spawn side follows “left door exit → spawn right” (with exceptions for Room1 and Hallway-from-Room1). Pierce’s Y position is never overridden.
- **Squawk** `CursorController` (in `Assets/Squawk/Scripts/UI/`) is used for door-hover cursor; it initialises in `Awake`.

See in-scene setup (RoomManager, ClickController, room definitions, floor colliders) for required references.

## Development Guidelines

### Code Standards
- **Files and Folders**: PascalCase
- **C# Methods**: PascalCase
- **C# Properties**: PascalCase
- **Private Fields**: `_camelCase`
- **Explicit typing**: Avoid `var` for non-obvious types
- **No ternary operators** in committed code
- **Function doc blocks** required for all functions

### Unity Best Practices
- Use **SerializedFields** for Inspector tuning
- Cache component references in `Awake`/`Start`, avoid `GetComponent` in `Update`
- Prefer **ScriptableObjects** for configs/registries
- Use **Events** over singletons when possible
- Configure layers/tags in Project Settings
- Use **Input System** for input handling

### Script Organization
- Target ~10 functions per script
- Keep behavior with its owning entity
- Use **single source of truth** for IDs and shared data
- Prefer **Resources** (ScriptableObjects) for tunables and registries

### Testing
- Use **EditMode/PlayMode** tests with NUnit
- Test math/state logic; avoid fragile frame-by-frame visual assertions

## Getting Started

### For New Contributors

**Never used GitHub before?** Start here:
- Read [Docs/GitHubGuide.md](Docs/GitHubGuide.md) - A complete beginner's guide to GitHub and Git
- This guide covers everything from creating an account to daily workflows

### Setup Steps

1. **Install Git and Git LFS** (see [GitHubGuide.md](Docs/GitHubGuide.md) for detailed instructions)
2. **Clone the repository** to your computer
3. **Open the project in Unity** (Unity 6.0.3 or later - see `ProjectSettings/ProjectVersion.txt` for exact version)
4. **Wait for packages to import** - Unity will automatically download required packages from `Packages/manifest.json`
5. **If you see compilation errors:**
   - Wait for Unity Package Manager to finish downloading packages (check the Package Manager window)
   - If errors persist, go to **Window → Package Manager** and click "Refresh" or "Reimport All"
   - Close and reopen Unity if packages still haven't imported
6. Check the `/Docs` folder for design documentation

### Important: Git LFS Required

This project uses **Git LFS (Large File Storage)** for large binary files (textures, audio, models, etc.). 

**Before cloning:**
- Install Git LFS: [git-lfs.github.com](https://git-lfs.github.com)
- Run `git lfs install` after installation

The `.gitattributes` file is already configured to handle large files automatically.

## Troubleshooting

### Package Import Errors

If you see errors like "The type or namespace name 'Bolt' could not be found" or missing package errors:

1. **Wait for packages to download** - Unity automatically imports packages listed in `Packages/manifest.json`, but this can take several minutes on first open
2. **Check Package Manager** - Open **Window → Package Manager** and verify all packages are installed
3. **Force reimport** - Right-click the `Packages` folder in Project window → **Reimport**
4. **Restart Unity** - Close and reopen Unity to ensure all packages are properly loaded
5. **Check Unity version** - Ensure you're using Unity 6.0.3 or later (check `ProjectSettings/ProjectVersion.txt`)

### Missing Scripts or Pink Materials

- **Never move/rename files outside Unity** - Always use Unity's Project window
- If this happened, use Git to restore: `git checkout -- Assets/`
- Then move/rename files inside Unity

## Contributing

- **New to GitHub?** Start with [Docs/GitHubGuide.md](Docs/GitHubGuide.md) - Complete beginner's guide
- **Ready to code?** See [CONTRIBUTING.md](CONTRIBUTING.md) for development guidelines, code standards, and Git workflow

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
