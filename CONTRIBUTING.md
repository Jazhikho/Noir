# Contributing Guidelines

Welcome to the project! This document outlines our coding standards and Git workflow to keep the codebase clean, consistent, and maintainable.

---

## Quick Start Summary (For Non-Coders)

If you're working with art, audio, or design assets and aren't writing code, here's what you need to know:

### Where to Put Your Work
- **All Unity assets** (art, audio, scenes) go inside the `Private Eye Pierce & The Ink Spill` folder
- **Design documents** go in the root `/Docs` folder
- Never move or rename files outside Unity - always use Unity's Project window

### Daily Workflow (5 Simple Steps)
1. **Pull** latest changes: `git pull` (get what teammates worked on)
2. **Do your work** (create art, edit scenes, add audio, etc.)
3. **Test** your changes in Unity
4. **Commit** your work: `git add .` then `git commit -m "Brief description"` (save your work)
5. **Push** your changes: `git push` (share with team)

### Before Starting Work
- Always `git pull` first to get the latest files
- If working on scenes or prefabs, coordinate with teammates to avoid conflicts

### Making Changes
- **Small changes** are better - commit frequently
- **Test everything** before committing
- **Write clear commit messages**: "Add office background art" not "stuff"

### Need Help?
- If something breaks, ask a coder before trying to fix it
- When in doubt, coordinate with teammates
- See the [Git Workflow](#git-workflow) section below for more details

**For detailed technical guidelines, see the sections below.**

---

## Table of Contents

1. [Project Structure](#project-structure)
2. [Naming Conventions](#naming-conventions)
3. [Code Style](#code-style)
4. [Documentation](#documentation)
5. [Error Handling](#error-handling)
6. [Performance](#performance)
7. [Unity-Specific Guidelines](#unity-specific-guidelines)
8. [Testing](#testing)
9. [Git Workflow](#git-workflow)
10. [PR Checklist](#pr-checklist)

---

## Project Structure

Our project follows this folder layout:

```
/Private Eye Pierce & The Ink Spill
  /Assets
    /Scenes              # Unity scene files
    /Scripts             # Gameplay scripts
    /Runtime             # Runtime scripts (assemblies as needed)
    /Editor              # Editor scripts (separate assembly)
    /Resources           # ScriptableObjects and data assets
    /Shaders             # HLSL/ShaderGraph shader assets
    /Audio               # Audio clips, mixers, and audio assets
    /Art                 # Models, textures, materials, animations
      /Models            # 3D models
      /Textures          # Texture files
      /Materials         # Material files
      /Animations        # Animation clips and controllers
    /Tests               # Unit and integration tests
  /Packages              # Unity package manifests
  /ProjectSettings       # Unity project settings
/Docs                    # Design documents (at root level, outside Unity project)
```

**Important:** 
- All Unity assets must be inside the `Private Eye Pierce & The Ink Spill` folder
- Keep third-party assets in their own folders at the root of `Private Eye Pierce & The Ink Spill/Assets`
- Design documentation lives in the root `/Docs` folder (outside the Unity project)

---

## Naming Conventions

### Files and Folders
- **PascalCase** for all files and folders
- One class per file
- File name must match class name

**Examples:**
- ✅ `PlayerController.cs`
- ✅ `HealthSystem.cs`
- ✅ `EnemySpawner.cs`
- ❌ `player_controller.cs`
- ❌ `health.cs`

### Functions and Methods
- **C# methods:** PascalCase, verb-first
- **Unity lifecycle methods:** Exact casing (`Start`, `Update`, `Awake`, etc.)

**Examples:**
- ✅ `public void SpawnEnemy()`
- ✅ `private bool HasItemInInventory(string itemId)`
- ✅ `void Start()`
- ❌ `public void spawnEnemy()`
- ❌ `private bool hasItem()`

### Variables and Properties
- **C# properties:** PascalCase
- **Private fields:** `_camelCase` (with underscore prefix)
- **Local variables:** camelCase
- **Constants:** UPPER_SNAKE_CASE or PascalCase

**Examples:**
```csharp
public class Player
{
    // Properties
    public int MaxHealth { get; private set; }
    
    // Private fields
    private int _currentHealth;
    private bool _isAlive;
    
    // Constants
    private const int DEFAULT_HEALTH = 100;
    
    // Local variables in methods
    public void TakeDamage(int damageAmount)
    {
        int remainingHealth = _currentHealth - damageAmount;
        // ...
    }
}
```

**Don't shadow Unity/engine concepts:**
- ❌ Avoid naming variables: `transform`, `position`, `gameObject`

---

## Code Style

### Explicit Typing
- **Always use explicit types** - no `var` unless type is obvious
- **Type all parameters and return values**

**Examples:**
```csharp
// ✅ Good
public int CalculateDamage(int baseDamage, float multiplier)
{
    int finalDamage = Mathf.RoundToInt(baseDamage * multiplier);
    return finalDamage;
}

// ❌ Bad
public var CalculateDamage(var baseDamage, var multiplier)
{
    var finalDamage = Mathf.RoundToInt(baseDamage * multiplier);
    return finalDamage;
}
```

### No Ternary Operators
Use explicit `if` statements for clarity and debuggability.

```csharp
// ❌ Bad
int health = isDead ? 0 : currentHealth;

// ✅ Good
int health;
if (isDead)
{
    health = 0;
}
else
{
    health = currentHealth;
}
```

### Script Size
- Target **~10 functions per script** (soft guideline)
- If you exceed 12-15 functions, consider:
  - Splitting by responsibility
  - Moving configuration to ScriptableObjects
  - Extracting helper classes

### Code Organization (C#)
**Order within a class:**
1. Serialized fields
2. Private fields
3. Properties
4. Unity lifecycle methods (Awake, Start, Update, etc.)
5. Public methods
6. Internal/protected methods
7. Private helper methods

---

## Documentation

### Function Documentation
**Every public function/method must have XML documentation:**

```csharp
/// <summary>
/// Calculates damage after applying armor reduction and multipliers.
/// </summary>
/// <param name="baseDamage">The raw damage before modifications</param>
/// <param name="armor">Target's armor value</param>
/// <returns>The final damage value to apply</returns>
public int CalculateDamage(int baseDamage, int armor)
{
    // Implementation
}
```

### Field Documentation
**For Unity [SerializeField] attributes, use comments above:**

```csharp
/// <summary>
/// Maximum health the player can have
/// </summary>
[SerializeField] private int _maxHealth = 100;

/// <summary>
/// Speed at which the player moves (units per second)
/// </summary>
[SerializeField] private float _moveSpeed = 5.0f;
```

### Inline Comments
- Comment **why**, not **what**
- No "TODO" or "FIXME" comments - create issues instead
- No commented-out code in commits

```csharp
// ✅ Good - explains why
// Use square root for natural-feeling deceleration
float deceleration = Mathf.Sqrt(speed);

// ❌ Bad - obvious what it does
// Set health to 100
health = 100;
```

---

## Error Handling

### Fail Loudly
**Never hide errors** - fail with descriptive messages.

```csharp
// ✅ Good
public void LoadPlayer(string playerId)
{
    if (string.IsNullOrEmpty(playerId))
    {
        Debug.LogError($"LoadPlayer failed: playerId is null or empty");
        return;
    }
    
    PlayerData data = Database.GetPlayer(playerId);
    if (data == null)
    {
        Debug.LogError($"LoadPlayer failed: No player found with ID '{playerId}'");
        return;
    }
    
    // Continue...
}

// ❌ Bad - silent failure
public void LoadPlayer(string playerId)
{
    PlayerData data = Database.GetPlayer(playerId);
    if (data != null)
    {
        // Continue...
    }
}
```

### Log Context
Always log enough information to debug:
- Which object/component
- Parameter values
- Expected vs actual

```csharp
Debug.LogError($"[EnemySpawner] Failed to spawn enemy at position {spawnPosition}. " +
               $"Expected prefab '{enemyPrefab.name}' but prefab is null", this);
```

### Treat Warnings as Errors
- Code must compile cleanly with zero warnings
- Fix warnings immediately

---

## Performance

### Cache References
**Don't call `GetComponent` or expensive lookups in Update:**

```csharp
// ❌ Bad - called every frame
void Update()
{
    Rigidbody rb = GetComponent<Rigidbody>();
    rb.AddForce(Vector3.up);
}

// ✅ Good - cached in Awake/Start
private Rigidbody _rigidbody;

void Awake()
{
    _rigidbody = GetComponent<Rigidbody>();
}

void Update()
{
    _rigidbody.AddForce(Vector3.up);
}
```

### Avoid Allocations in Hot Paths
- Reuse lists/arrays instead of creating new ones
- Use `StringBuilder` for string concatenation in loops
- Pool frequently spawned objects

### Use Fixed Update for Physics
```csharp
void FixedUpdate() // NOT Update()
{
    _rigidbody.AddForce(moveDirection * moveSpeed);
}
```

---

## Unity-Specific Guidelines

### Inspector-First Configuration
**If Unity can do it in the Inspector, configure it there** instead of code:
- Collision layers/masks → Project Settings
- Input → Input System asset
- Audio routing → Audio Mixer
- Post-processing → Volume Profiles

### Use ScriptableObjects for Data
**Don't hard-code configuration:**

```csharp
// ❌ Bad
public class Enemy : MonoBehaviour
{
    private int health = 100;
    private float speed = 3.5f;
    private int damage = 25;
}

// ✅ Good
[CreateAssetMenu(fileName = "EnemyData", menuName = "Game/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public int Health;
    public float Speed;
    public int Damage;
}

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyData _data;
}
```

### Events Over Polling
Use C# events or UnityEvents instead of checking conditions every frame:

```csharp
// ❌ Bad - polling
void Update()
{
    if (health <= 0 && !isDead)
    {
        OnDeath();
    }
}

// ✅ Good - event-driven
public event Action OnDeath;

public void TakeDamage(int damage)
{
    health -= damage;
    if (health <= 0)
    {
        OnDeath?.Invoke();
    }
}
```

### Avoid GetComponent in Update
Cache component references in `Awake` or `Start`.

---

## Testing

### Unit Tests
- Use **Unity Test Framework** (NUnit)
- Test critical game logic, math, and state management
- Avoid fragile frame-by-frame assertions

```csharp
[TestFixture]
public class HealthSystemTests
{
    [Test]
    public void TakeDamage_ReducesHealth()
    {
        // Arrange
        HealthSystem health = new HealthSystem(100);
        
        // Act
        health.TakeDamage(25);
        
        // Assert
        Assert.AreEqual(75, health.CurrentHealth);
    }
}
```

### Test Location
- Place tests in `Private Eye Pierce & The Ink Spill/Assets/Tests/`
- Use EditMode tests for logic testing
- Use PlayMode tests for integration testing

---

## Git Workflow

### Daily Routine (5 Steps)

1. **Pull latest changes**: `git pull` to sync with team's work
2. **Work, test, and fix errors**
3. **Stage your changes**: `git add` for files you've modified
4. **Commit with clear message**: `git commit -m "Clear message"`
5. **Push your changes**: `git push` to share with team

Do steps 1 and 4 often.

### When to Commit vs Stash vs Branch

* **Commit** (push to repository) when:
  * It builds and plays without new errors
  * One small theme per commit: "Fix enemy patrol bug," "Add laser SFX"
  * Avoid dumping a whole day of unrelated changes in one go

* **Stash** (private, temporary save) when:
  * The work isn't ready for the team, and you need to: switch branches, pull big updates, or try something risky
  * Restoring later: `git stash pop` → continue

* **Branch** (work in isolation) when:
  * A feature will take more than a day or will disrupt others
  * Name like: `feature/enemy-ai` or `fix/ui-overlap`

### Essential Git Commands

* **Pull teammates' work**: `git pull`
* **See what changed**: `git status`, `git diff`
* **Stage changes**: `git add <file>` or `git add .` (be careful)
* **Commit**: `git commit -m "Clear descriptive message"`
* **Push**: `git push`
* **Create branch**: `git checkout -b feature/your-feature-name`
* **Switch branch**: `git checkout <branch-name>`
* **Stash current work**: `git stash`
* **Restore stashed work**: `git stash pop`
* **See commit history**: `git log`
* **Revert a file**: `git checkout -- <file>`
* **See file history**: `git log <file-path>`

### Good Commits: How Small, How Often, How to Write Messages

* **Small and frequent.** Aim for a single task ("Add sfx for laser", "Fix typos in dialogue for Scene X").
* **Build passes.** No red Console on commit.
* **Message template:**
  * Title (imperative, ~60 chars): `Fix patrol idle loop when route ends`
  * Body (optional, bullet points):
    * Why change was needed
    * Risk/impact
    * Follow-ups or TODOs

**Examples:**
```
Fix enemy patrol path when reaching end of route

The patrol AI would enter an idle state indefinitely when
reaching the final waypoint. Now correctly loops back to
the start or reverses direction based on patrol type.
```

### Simple Branching Model

* `main` (stable) → always shippable/playable
* `feature/*` branches per task. Merge back to `main` when done
* Never push half-broken work to `main`. Use a feature branch or a stash

### Minimal Conflict-Free Workflow for Scenes/Prefabs

* **Before** opening or changing a scene/prefab, **pull latest changes** (`git pull`)
* Make your change, test it, **commit** with clear message
* **Push** your changes promptly

If there's a merge conflict:
* **Don't panic.** Read the conflict markers carefully
* For Unity scene/prefab conflicts, prefer:
  * **Theirs** if you want teammate's version (and haven't made changes yet)
  * **Yours** if you want yours (only if you're sure)
  * **Manual merge** - carefully combine both changes, test thoroughly
* For script conflicts, use Git's merge tool to resolve line by line
* After resolving, test the merged result before committing

### Before You Start Coding

1. **Survey:** Search the project for existing implementations
2. **Plan:** Draft a function list with signatures and purposes
3. **Discuss:** Get approval on the approach before implementing

### While Coding

1. **Implement:** Follow all rules in this document
2. **Document:** Add XML docs for all public methods
3. **Test:** Write/update tests for critical logic

### Before Committing

1. **Pull latest**: `git pull` to get any remote changes
2. **Remove:** Delete any debug logs, commented code
3. **Verify:** Zero warnings, zero errors
4. **Test:** Run the game and verify your changes work
5. **Stage:** `git add` only relevant files
6. **Commit:** Write clear message and commit
7. **Push:** `git push` to share with team

### Troubleshooting Quick Fixes

* **I can't see my partner's changes**
  * You're on the right branch? `git branch` to check current
  * `git pull` to fetch and merge remote changes
  * They actually **pushed** to the repository? Ask them to confirm

* **My references are missing / pink materials**
  * You moved/renamed files outside Unity. Undo that and move inside Unity so .meta files follow
  * Always use Unity's Project window or file browser for moving/renaming assets

* **Endless conflicts on a scene**
  * Coordinate scene editing with teammates - avoid simultaneous edits
  * Consider splitting into **additive scenes**: one scene per feature/area
  * Communicate before editing shared assets

* **Project suddenly has huge diffs**
  * Someone changed Unity version or reimported assets. Coordinate upgrades and avoid random reimports
  * Check `.gitignore` is properly configured

* **I messed up badly**
  * `git log` to find the last good commit
  * `git checkout <commit-hash> -- <file>` to restore specific files
  * `git reset --hard HEAD~1` to undo last commit (use carefully)
  * `git revert <commit-hash>` to create a new commit that undoes changes

---

## PR Checklist

Before submitting your changes, verify:

- [ ] File names and folders are PascalCase
- [ ] C# methods are PascalCase, Unity messages use exact casing
- [ ] Variables are descriptive; no shadowing engine fields
- [ ] All parameters and variables are explicitly typed
- [ ] No ternary operators; no ambiguous `var`
- [ ] Engine-first configuration used where applicable
- [ ] Single source of truth for IDs and shared data
- [ ] Function XML docs are present and accurate
- [ ] Script size is around 10 functions or split is justified
- [ ] Zero compiler warnings
- [ ] Error messages are descriptive
- [ ] Tests updated/added for critical logic
- [ ] No debug logs or commented-out code
- [ ] GetComponent calls are cached outside Update
- [ ] Configuration uses ScriptableObjects where appropriate
- [ ] Pulled latest changes before committing
- [ ] Commit message is clear and descriptive
- [ ] Unity .meta files are included in commits
- [ ] Scene/prefab changes are tested before committing

---

## Exceptions

### Unity Lifecycle Methods
Unity lifecycle methods must use exact signatures and casing:
- ✅ `void Start()` (not `void start()`)
- ✅ `void OnTriggerEnter(Collider other)` (exact signature)

### Third-Party APIs
When interacting with third-party APIs, you may need to match their naming conventions. Wrap them to preserve internal consistency where possible.

---

## Checklist Card (Print This)

* Pull latest changes before starting
* Coordinate scene/prefab editing with teammates
* Keep changes small; test before commit
* Include .meta files automatically (Unity handles this)
* Stash if not ready; branch if it's big
* Pull again before you push, then push
* Write clear, descriptive commit messages

---

## Questions?

If you're unsure about something:
1. Check this document first
2. Look for similar code in the project
3. Ask the team lead
4. When in doubt, favor clarity over cleverness

Thank you for contributing! 🎮
