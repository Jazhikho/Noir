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
Music Assets:
I Knew a Guy by Kevin MacLeod
Shady Business by David Renda 
Dances and Dames by Kevin MacLeod
Cool Vibes by Kevin MacLeod 
Closed Curtains by David Renda
Untapped Well by Ashley Haberberger and Joshua Hammond

---

## Code / Repo Notes (for team)

- **RefreshLockedDoors** (`AdventureHUDController`): Public method is intentionally left in place but is not currently called from anywhere. Call it when doors are unlocked at runtime so the HUD arrow and lock state stay correct. See XML doc on the method.
- **Folder naming**: `Assets/Scripts/ScriptDocumentation/Demo-Specific/` uses a hyphen; CONTRIBUTING prefers PascalCase (e.g. `DemoSpecific`). Rename only when confirmed no scene/prefab/asset references depend on the path.
- **Demo scripts** (`FadeGroup`, `DemoEndSequence`): Not referenced in MainMenu or OfficeFloor; kept for possible demo/Bolt flows. Remove or relocate if the team decides they are unused.
- **Ternaries**: CONTRIBUTING disallows ternary operators. One instance was fixed in `AdventureHUDController`. Other scripts (e.g. `RoomManager`, `UIController`, `ClickController2D`, `ButtonMashPromptUI`, `AdventureCameraController`) still contain ternaries; consider refactoring to explicit `if`/`else` in a follow-up.

---

AI Use Statement:
This project used AI-assisted tools during development. Claude Sonnet 4.5 (Anthropic) was used for portions of code authoring and implementation. Cursor (Cursor, Inc.) was used for code authoring support and repository management. All AI-generated or AI-assisted content was reviewed, edited, and approved by the team; design and creative decisions remain human-led.
