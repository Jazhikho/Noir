# Team Noir

Ashley Haberberger - Audio, Player Design
Bella Howe - Concept, Art Style, Influences and References
Christopher Del Gesso - UI, Technical Details (Coding, Repo Management)
Kaya Wilson - Enemy and Obstacle Design
Kevin - MVP, Elevator pitch, Genre and target audience
Matthew Nichols - User Experience, Core Gameplay Loop, Game Objective

# Elevator Pitch 

“A washed-up detective escapes the monotony of his grayscale office as color invades the outside world.”

## Concept
Private Eye Pierce & The Ink Spill is a point-and-click 2D sidescroller taking place in a black-and-white comic world. The protagonist, P.I Pierce, has been investigating the colorful ink spill spreading through the city and now finds himself locked in his office building. He’ll explore the different rooms, talking to other surreal characters and solving puzzles as works towards his escape.

---

## MatthewBranch merge review (Feb 2025)

**Branch:** `MatthewBranch` (Matt's branch)  
**Target:** `main` (reverted to `c31eb0d`; borked merge `cc6d0d0` was reverted.)

### Commits that would merge in (main..MatthewBranch)
- `671b2f4` C
- `676ac30` Merge branch 'main' into UI
- `319d74f` Create .vsconfig

### Summary of changes (diff main..MatthewBranch)
- **Scenes:** Adds `Main Menu.unity`; removes `Test.unity`; updates `OfficeFloor.unity`.
- **Scripts:** Adds `MenuController.cs`; updates `PlayerController.cs`, `PlayerMover.cs`; removes `AdventureCameraController.cs`, `TutorialPuzzle.cs`, `WalkToInteractable.cs`.
- **Art:** Replaces/removes several placeholders and Carmen Evangeline assets; adds `PierceOfficeConcept.png`; removes pierce sprites (pierceHallway, pierceIdle, pierceLook, pierceWalk) and placeholder art.
- **Project:** `.gitattributes` and `.gitignore` changes; `EditorBuildSettings`, `PackageManagerSettings`, `ProjectSettings`, `TagManager` tweaks; `Docs/GitHubGuide.md`, `README.md` edits.

### TMP (TextMesh Pro) assets to remove before or after merge
Do **not** add or keep a full **TextMesh Pro** package under `Assets/TextMesh Pro/` (if it appears as untracked or gets added, exclude it from the merge or delete it after).

On MatthewBranch, TMP is also used in:
- **Assets/Squawk/Materials/TMP materials/** — folder containing `LiberationSans SDF Squawk.mat` (and `.meta`). Remove this folder and its `.meta` if you want to drop all TMP assets.
- **Main Menu.unity** — uses `Text (TMP)` / `TMPro.TextMeshProUGUI` components. If you remove TMP, these need to be replaced with Unity UI Text or another solution.
- **Squawk:** `CutsceneConversation.asset`, cutscene scenes, and scripts (`DialogueUI.cs`, `Inventory.cs`, `Tooltip.cs`, `CutsceneConversationSO.cs`) reference TMPro. Removing TMP will require updating those to use a non-TMP text solution.

**Action:** Remove `Private Eye Pierce & The Ink Spill/Assets/Squawk/Materials/TMP materials/` (and `TMP materials.meta`) before merging, and either (a) add the package via Package Manager only (no Assets/TextMesh Pro folder), or (b) migrate Main Menu and Squawk UI to non-TMP text and then remove all TMP usage.

---

## Recent merges (Feb 2026)

**main** currently includes:

- **Bella-Branch:** Art assets (Backgrounds: PierceBreakroom, PierceOfficeBackr, PierceOfficeFront; PuzzleAssets: BoxChecked, BoxUnchecked, DoorBroken, FridgeChecked, PierceBat, PierceHand, TrashChecked, TrashUnchecked; Art root: PierceIdle, PierceLook, PierceWalk, PierceHallway; Backgrounds/PierceHallway). All art file names use **PascalCase** per project standard.
- **NewMatthewBranch (aligned):** UI art (cursors, dialogue bubbles, buttons under `Assets/Art/UI/`), Audio (Music and SFX under `Assets/Audio/`), `UIController.cs`, `SceneFader.cs`, and updated MainMenu and OfficeFloor scenes. No TextMesh Pro package in repo; scenes may have placeholder TMP references to clean up in Unity.

**Folder naming:** `Assets/Art`, `Assets/Scenes`, and `Assets/Scripts` are the canonical PascalCase folders for game content.