# Noir

A Unity game project.

## Project Structure

```
/Scenes            # Unity Scenes
/Scripts           # Gameplay scripts
/Runtime           # Unity runtime scripts (assemblies as needed)
/Editor            # Unity editor scripts (separate assembly)
/Resources         # Unity ScriptableObjects/Data
/Shaders           # Unity HLSL/ShaderGraph assets
/Audio             # Audio assets
/Art               # Models/Textures/Materials
/Tests             # Tests
/Docs              # Design docs, cheat sheets
```

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

1. Open the project in Unity (recommended version: 2022.3 LTS or later)
2. Ensure all packages are imported
3. Check the `/Docs` folder for design documentation

## License

See LICENSE file for details.
