# Kevin Test Scripts - Documentation

**Complete Documentation & Hookup Guide**  
Guide for Unity 6000.3f1  
Private Eye Pierce Demo No. 1

---

## Table of Contents

1. [Introduction and File Structure](#1-introduction-and-file-structure)
2. [Before You Begin: Creating Event Flags](#2-before-you-begin-creating-event-flags)
3. [Dialogue Folder Scripts](#3-dialogue-folder-scripts)
4. [Flags Folder Scripts](#4-flags-folder-scripts)
5. [HUD Folder Scripts](#5-hud-folder-scripts)
6. [Interaction Folder Scripts](#6-interaction-folder-scripts)
7. [Player Folder Scripts](#7-player-folder-scripts)
8. [Puzzles Folder Scripts](#8-puzzles-folder-scripts)
9. [Rooms Folder Scripts](#9-rooms-folder-scripts)
10. [Demo-Specific Folder Scripts](#10-demo-specific-folder-scripts)
11. [Audio](#11-audio)
12. [Complete Demo Walkthrough](#12-complete-demo-walkthrough)
13. [Troubleshooting](#13-troubleshooting)

---

## 1. Introduction and File Structure

Welcome! This guide teaches you how to use the Kevin Test scripts to build stuff in Private Eye Pierce.

### Your Folder Structure

All the scripts live inside: **Assets > Scripts**

Inside that folder, you'll find these subfolders:

| Folder | What's Inside |
|--------|---------------|
| **Dialogue** | Scripts that make characters talk |
| **Flags** | Scripts that remember yes/no information |
| **HUD** | Scripts for on-screen displays (cursor, highlights) |
| **Interaction** | Scripts for clicking on things |
| **Player** | Scripts that control the player character |
| **Puzzles** | Scripts for game puzzles |
| **Rooms** | Scripts for different areas/rooms |
| **ScriptDocumentation/Demo-Specific** | Scripts only used for the demo ending |
| **UI** | UI helpers (SceneFader, PageTurnTransition, UIController) |
| **Audio** | Jukebox and audio helpers |

For dialogue snippets and audio, see the audio folder in the project (**Assets > Audio**). Any dialogue references in this doc are hypotheticals for showing how the scripts work.

### Animations

All animations should live inside: **Assets > Art > Animations**

---

## 2. Before You Begin: Creating Event Flags

**IMPORTANT:** Before setting up most scripts, you need to create Event Flags. These are like sticky notes that the game uses to remember things.

### What is an Event Flag?

Think of an Event Flag like a light switch. It's either ON or OFF. The game uses these to remember things like:

- Did the player pick up the keys? (ON = yes, OFF = no)
- Is the break room door unlocked? (ON = yes, OFF = no)
- Did the player complete the tutorial? (ON = yes, OFF = no)

### The 3 Event Flags You Need to Create

For the demo, you need exactly 3 Event Flags:

| Flag Name | What It Remembers |
|-----------|-------------------|
| TutorialComplete | Did Pierce break the office door window? |
| BreakRoomUnlocked | Did Pierce complete the vending machine puzzle? |
| HasKeys | Did Pierce find the janitor's keys? |

### How to Create an Event Flag (Step by Step)

1. **Open Your Project Window** – Look at the bottom of Unity. You should see a panel called "Project".

2. **Navigate to Where You Want to Save Flags** – Go to: **Assets > ScriptableObjects > Flags**  
   (If this folder doesn't exist, create it: right-click > Create > Folder)

3. **Right-Click in the Empty Space** – Right-click anywhere in the empty area of the Project window (not on a file).

4. **Create the Event Flag** – Click: **Create > Game > Event Flag**

5. **Name Your Flag** – Type one of these names exactly:
   - TutorialComplete
   - BreakRoomUnlocked
   - HasKeys

6. **Configure the Flag** – Click on the flag. In the Inspector:
   - **Is Active**: Leave UNCHECKED (the flag starts OFF)
   - **Reset On Play**: CHECK this box (so the flag resets when you test the game)
   - **Default Value**: Leave UNCHECKED (the flag should start as OFF)

7. **Repeat** – Do steps 3–6 three times, once for each flag name.

---

## 3. Dialogue Folder Scripts

**Location:** Assets > Scripts > Dialogue

### DialogueAsset.cs

**What Does It Do?**  
Holds all the words that characters say during a conversation. Create one for each conversation.

**How to Create a Dialogue Asset**

1. Right-click in your Project window
2. Click: **Create > Dialogue > Dialogue Asset**
3. Name it something like "Carmen_Dialogue" or "Janitor_Greeting"
4. Click on it and fill in the Inspector:
   - **Dialogue Name**: A friendly name like "Carmen First Meeting"
   - **Default Player Portrait**: Drag in Pierce's face picture
   - **Default Other Portrait**: Drag in the NPC's face picture
   - **Player Name**: Type "Pierce"
   - **Other Name**: Type the NPC's name (e.g. "Carmen")
5. In the **Lines** section, click + to add dialogue lines:
   - **Speaker**: Player or Other
   - **Text**: What they say

### DialogueRunner.cs

**What Does It Do?**  
Starts conversations. When it's time to talk, this tells DialogueUI to show the conversation.

**How to Hook It Up**

1. Create empty object: Right-click in Hierarchy > Create Empty
2. Name it "DialogueSystem"
3. Add Component > type "DialogueRunner" and add it
4. Unity automatically adds **DialogueUI** to the same GameObject (required by DialogueRunner)

**Note:** You only need ONE DialogueRunner in your entire scene. Assign the dialogue panel and images on the **DialogueUI** component (step 5 under DialogueUI).

### DialogueTypes.cs

**What Does It Do?**  
Defines DialogueLine, Speaker, DialogueChoice. A behind-the-scenes helper.

**Hookup:** None needed. Just make sure the file is in your project.

### DialogueUI.cs

**What Does It Do?**  
Shows the dialogue box, character portraits, and types out text letter by letter. Advances on left-click automatically (no separate PlayerInput action needed).

**How to Hook It Up**

1. Create Canvas: Right-click in Hierarchy > UI > Canvas
2. Inside the Canvas, create Panel: Right-click Canvas > UI > Panel
3. Name it "DialoguePanel" and design your dialogue box
4. Add inside DialoguePanel:
   - Image for player portrait (name it "PlayerImage")
   - Image for NPC portrait (name it "NPCImage")
   - TextMeshPro text for dialogue (name it "DialogueText")
   - TextMeshPro text for speaker name (name it "SpeakerNameText")
5. On the same GameObject (DialogueSystem), select the **DialogueUI** component (added automatically when you add DialogueRunner) and assign:
   - **Dialogue Panel**: your DialoguePanel
   - **Player Image**: your PlayerImage
   - **Npc Image**: your NPCImage
   - **Dialogue Text**: your DialogueText
   - **Speaker Name Text**: your SpeakerNameText

---

## 4. Flags Folder Scripts

**Location:** Assets > Scripts > Flags

### EventFlag.cs

**What Does It Do?**  
This is the light switch. A piece of data that says "yes" or "no"; other scripts can check it.

**How to Hook It Up**  
See Section 2 for creating Event Flags. Then drag the flag into other scripts that need it.

---

## 5. HUD Folder Scripts

**Location:** Assets > Scripts > HUD

### AdventureHUDController.cs

**What Does It Do?**  
Handles:
- Custom mouse cursor (can switch to Interactable or Door sprite when hovering)
- **Door cursor direction**: doors leading right show a right arrow, doors leading left a left arrow, doors on the wall show an up arrow (assign the three sprites below)
- Highlight when hovering over clickable things
- Arrow that points to exits (LockedDoor)

**How to Hook It Up**

1. Create Canvas if needed: Right-click Hierarchy > UI > Canvas
2. Name it "HUDCanvas"
3. Add AdventureHUDController to HUDCanvas
4. Create **Cursor Image**:
   - Right-click HUDCanvas > UI > Image
   - Name it "CursorImage"
   - Set Source Image to your cursor sprite
   - Set Pivot to X: 0, Y: 1 (top-left)
   - Uncheck "Raycast Target"
5. Create **Highlight Image**:
   - Right-click HUDCanvas > UI > Image
   - Name it "HighlightImage"
   - Set Source Image
   - Uncheck "Raycast Target"
   - DISABLE the GameObject
6. Create **Arrow Image** (same process as Highlight)
7. On AdventureHUDController, assign:
   - **Cursor Image**
   - **Highlight Image**
   - **Arrow Image**
   - **Interactable Cursor Sprite** (optional – used when hovering interactables)
   - **Door Cursor Sprite** (fallback arrow when direction sprites are unset)
   - **Door Cursor Left Sprite** (optional – left arrow for doors leading left)
   - **Door Cursor Right Sprite** (optional – right arrow for doors leading right)
   - **Door Cursor Up Sprite** (optional – up arrow for doors on the wall)
   - **Interactable Layer**: Check "Interactable"
   - **Hide System Cursor**: Check this box

### ButtonMashPromptUI.cs

**What Does It Do?**  
Shows text like "Click to escape!" during the vending machine puzzle. Text bobs and scales on click.

**How to Hook It Up**

1. Inside HUDCanvas, create Panel: Right-click > UI > Panel
2. Name it "MashPromptPanel"
3. Inside that panel, create TextMeshPro text
4. Name it "MashPromptText" and type "Click to escape!"
5. Add ButtonMashPromptUI to MashPromptPanel
6. Assign **Tmp Text** and **Panel Root**
7. Set Bob Speed to 2, Bob Amount to 10, Punch Scale to 1.2
8. DISABLE MashPromptPanel (the puzzle enables it when needed)

**Progress Texts**: List of threshold + text. Lets you show different messages at different progress values (e.g. "Almost there!" when close to winning). Each entry has a **threshold** (0–1) and **text**. The UI shows the text for the highest threshold the current progress has reached. **VendingMachinePuzzle** wires this up automatically via SetProgress01 and the prompt's progress text list.

### HoverHighlightTarget.cs

**What Does It Do?**  
Optional. Add to objects that should show a highlight when the mouse hovers over them.

**How to Hook It Up**  
Add Component > HoverHighlightTarget. Defaults work fine.

### HoverFeedback.cs

**What Does It Do?**  
When an interactable is highlighted (cursor hovers over it), this can trigger: a Pierce animation (e.g. "interested" look), a highlighted sprite or animation on the object, and a sound cue. Add to the same GameObject (or a child) as an IInteractable (e.g. Interactable, DoorInteractable). ClickController2D calls it automatically when hover starts and ends.

**How to Hook It Up**

1. Click on the interactable object (door, bat, vending machine, etc.)
2. Add Component > HoverFeedback
3. Optional – **Pierce Animation**: Assign **Pierce Animation Driver** (or leave unset to auto-find). In Pierce's Animator, add a trigger (e.g. "HighlightReact") and assign **Highlight React Trigger** on PierceAnimationDriver
4. Optional – **Object Highlight**: Assign **Sprite Renderer** and **Highlight Sprite** to swap to when hovered; the original sprite is restored when the cursor leaves
5. Optional – **Object Animation**: Assign **Object Animator** and **Object Hover Trigger** to fire an animation on the object when hovered
6. Optional – **Sound**: Assign **Hover Sound** (AudioClip). Assign **Audio Source** if you want a specific source; otherwise the sound plays at the camera position

### SearchPromptUI.cs

**What Does It Do?**  
Shows "Searching..." when Pierce searches a trash can or desk. Singleton (`SearchPromptUI.Instance`).

**How to Hook It Up**

1. Inside HUDCanvas, create Panel named "SearchPanel"
2. Inside that panel, create TextMeshPro text named "SearchingText"
3. Type "Searching..." in the text
4. Add SearchPromptUI to SearchPanel
5. Assign **Panel Root** and **Tmp Text**
6. DISABLE SearchPanel (SearchableProp calls Show/Hide at runtime)

---

## 6. Interaction Folder Scripts

**Location:** Assets > Scripts > Interaction

### DialogueInteraction.cs

**What Does It Do?**  
Put on NPCs. When Pierce clicks and walks over, a conversation starts. Can show different conversations based on an EventFlag (e.g. HasKeys).

**How to Hook It Up**

1. Click on your NPC sprite
2. Add Box Collider 2D
3. Set Layer to "Interactable"
4. Add Interactable component
5. Add DialogueInteraction component
6. Assign **Dialogue Runner** and **Dialogue Asset**
7. For conditional dialogue: assign **Conditional Flag** and **Conditional Dialogue Asset**
8. On Interactable's **On Interact**, click +, drag this NPC in, choose DialogueInteraction > StartDialogue

### Interactable.cs

**What Does It Do?**  
Makes objects clickable. Pierce walks to them, then On Interact fires. Use for doors, items, NPCs, puzzles.

**How to Hook It Up**

1. Click on the object
2. Add Collider2D (Box Collider 2D)
3. Set Layer to "Interactable"
4. Add Interactable component
5. Set **Default Offset** to 1
6. In **On Interact**, click + and connect what should happen

**Note:** For interactables where Pierce should move again after (e.g. search), use PierceAnimationDriver.PlayInspectAutoEnd. For puzzles that manage movement themselves, use PlayInspect.

### LockedDoor.cs

**What Does It Do?**  
A door locked by an EventFlag. When unlocked, teleports the player to the target room. Uses `RoomManager.Instance` (no manual Room Manager assignment). Used by TutorialPuzzle for the office door after the window is broken.

**How to Hook It Up**

1. Click on your door sprite
2. Add Box Collider 2D (and optionally a trigger for glow)
3. Set Layer to "Interactable"
4. Add Interactable component
5. Add LockedDoor component
6. Fill in:
   - **Target Room**: Drag the RoomDefinition this door leads to
   - **Target Entry Point Name**: "Left" or "Right"
   - **Unlock Flag**: Drag the EventFlag that unlocks this door (e.g. BreakRoomUnlocked)
   - **Require Flag True**: CHECK
7. Other fields (optional):
   - **Is Locked**: Manual lock toggle; when true, the door refuses use regardless of the unlock flag. No EventFlag needed for this.
   - **Glow Effect**: GameObject to show when the player is nearby. The door (or a child) must have a **trigger collider** (e.g. Box Collider 2D with Is Trigger checked); LockedDoor shows/hides this on trigger enter/exit.
   - **Show Exit Arrow**: When true, the HUD can show an arrow pointing to this door.
8. On Interactable's **On Interact**:
   - For break room door: connect to LockedDoor > UseDoor
   - For tutorial door: connect to TutorialPuzzle > InteractWithDoor (TutorialPuzzle calls LockedDoor.UseDoor() internally after the window is broken)

### DoorInteractable.cs

**What Does It Do?**  
Click-based door. When Pierce clicks, he walks to it and RoomManager transitions to the target room. Implements IInteractable for ClickController2D. The mouse cursor shows a direction-specific arrow when hovering: right arrow for doors leading right, left arrow for left, up arrow for doors on the wall (set **Door Direction** and assign the corresponding cursor sprites on AdventureHUDController).

**How to Hook It Up**

1. Add to door GameObject
2. Add Box Collider 2D, set Layer to "Interactable"
3. Assign **Target Room Id** (must match a RoomDefinition.roomId)
4. Assign **Target Spawn**: "Left" or "Right"
5. Set **Door Direction**: **Left** (cursor shows left arrow), **Right** (right arrow), or **Wall** (up arrow for doors on the wall)
6. Optionally assign **Glow Effect** (GameObject to show when hovering)
7. RoomManager handles the transition when the player reaches the door

---

## 7. Player Folder Scripts

**Location:** Assets > Scripts > Player

### PointClickController.cs

**What Does It Do?**  
Makes Pierce walk when you click. Click on the floor → Pierce walks there. Click on an Interactable → Pierce walks to it and interacts. When **ClickController2D** is in the scene, it drives PointClickController (`inputHandledExternally` is set automatically); otherwise PointClickController handles input itself.

**Setting Up Layers First**

1. Edit > Project Settings > Tags and Layers
2. Find empty slot, type "Walkable"
3. Find another, type "Interactable"

**Setting Up a Walkable Area**

1. Right-click Hierarchy > 2D Object > Sprites > Square
2. Name it "WalkableArea"
3. Scale to cover your floor
4. Set Layer to "Walkable"
5. Add Box Collider 2D
6. Make invisible: SpriteRenderer Color alpha = 0

**How to Hook It Up**

1. Click on Pierce
2. Ensure SpriteRenderer and Collider2D
3. Set Tag to "Player"
4. Add PointClickController
5. Set **Move Speed** to 3, **Stopping Distance** to 0.1
6. Set **Floor Y** to your floor Y
7. **Walkable Layer**: Check "Walkable"
8. **Interactable Layer**: Check "Interactable"

### ClickController2D.cs

**What Does It Do?**  
Handles click-to-move and interaction. Converts screen clicks to world position, clamps to walk bounds, and calls IInteractable.OnClick() when hovering interactables. ClickController2D works with any component that implements IInteractable (e.g. Interactable, DoorInteractable). Drives PointClickController (sets `inputHandledExternally = true`).

**How to Hook It Up**

1. Add to a GameObject in the scene (e.g. on the same object as RoomManager or Camera)
2. Assign **Interactable Mask**: Check "Interactable"
3. **Walk Bounds**: RoomManager sets this automatically from the current room's floor collider when you enter a room. You can leave unset; assign only if you need a fallback (e.g. testing without RoomManager).
4. **Walk Bounds Inset X**: World units to inset from each side of walk bounds (default 0.5). Tune if movement feels tight or Pierce clips past props.
5. Assign **Pierce Controller** (PointClickController)
6. Optionally assign **Adventure HUD Controller** for cursor type switching

### AdventureCameraController.cs

**What Does It Do?**  
Controls the camera: follow Pierce, use room bounds, optional film look. **Picture Window** mode draws the game in a smaller rect (e.g. half the screen) with black bars on the sides and top/bottom.

**How to Hook It Up**

1. Click on Main Camera
2. Add AdventureCameraController
3. For film look: leave Target Aspect Width at 1.37
4. For small rooms: Check "Use Room Mode" and set Room Position
5. For big rooms: Uncheck "Use Room Mode", assign Target
6. For **Picture Window** (half-size view with black bars): Check **Picture Window Mode**, set **Picture Window Scale** (e.g. 0.5 = half width and height, centered). A full-screen clear camera runs first so the bar regions are black. **Main menu and other scenes**: If your Canvas is **Screen Space - Overlay**, it would draw full screen and hide the bars. AdventureCameraController automatically switches Overlay Canvases to **Screen Space - Camera** and assigns the Main Camera when Picture Window Mode is on, so the UI (e.g. main menu) only draws in the center and the black bars stay visible. For manual setup: use **Screen Space - Camera** and assign **Main Camera** (like OfficeFloor).

**SnapToTarget()**: Teleports the camera instantly to its target position (room position or follow target) instead of lerping. Call this after the player teleports (e.g. room change) to avoid camera lag; the script already uses it when switching room mode.

### FootstepLoop.cs

**What Does It Do?**  
Plays footstep sounds when Pierce walks. Starts when he moves, stops when he stops.

**How to Hook It Up**

1. Click on Pierce
2. Add Audio Source
3. Add FootstepLoop
4. Assign **Audio Source** and **Footstep Clip**
5. Set Volume to 0.5

### PierceAnimationDriver.cs

**What Does It Do?**  
Controls Pierce's animations: look up when mouse is above him, inspect animation when examining things, and an optional **highlight-react** animation when the cursor hovers an interactable (if HoverFeedback is used). Use **PlayInspect** for one-shot; use **PlayInspectAutoEnd** for interactions that should re-enable movement automatically (e.g. SearchableProp).

**How to Hook It Up**

1. Click on Pierce
2. Add PierceAnimationDriver
3. Assign **Animator**
4. In Animator Controller, add:
   - **Inspect** (Trigger)
   - **IsLookingUp** (Bool) – optional
   - **HighlightReact** (Trigger) – optional; fired when hovering an interactable that has HoverFeedback (set **Highlight React Trigger** on PierceAnimationDriver to match)

---

## 8. Puzzles Folder Scripts

**Location:** Assets > Scripts > Puzzles

### KeyHuntManager.cs

**What Does It Do?**  
Optional. Randomly picks which searchable object has the keys.

**How to Hook It Up**

1. Create Empty "KeyHuntManager"
2. Add KeyHuntManager
3. Drag all searchable props into **Searchable Props**
4. Assign **Has Keys Flag**
5. Check **Randomize Key Location** for random placement; if unchecked, assign **Fixed Key Prop** to the one SearchableProp that should contain the keys.

### SearchableProp.cs

**What Does It Do?**  
Put on things Pierce can search (trash cans, desks). When clicked, Pierce searches and might find keys. Uses SearchPromptUI.Instance for "Searching..." text.

**How to Hook It Up**

1. Click on searchable object
2. Add Box Collider 2D, set Layer to "Interactable"
3. Add Interactable
4. Add SearchableProp
5. Set **Search Duration** to 1.5
6. For the object with keys: check **Contains Keys**
7. Assign **Has Keys Flag**
8. On Interactable **On Interact**, connect to SearchableProp > Search

### TutorialPuzzle.cs

**What Does It Do?**  
First puzzle: Pierce is trapped in his office. He finds a bat and uses it to break the door window. Uses LockedDoor for the room transition after the window is broken.

**How to Hook It Up**

1. Create Empty "TutorialController"
2. Add TutorialPuzzle
3. **Bat setup:**
   - Create bat sprite, add Box Collider 2D, Layer Interactable, Interactable
   - Drag bat into TutorialPuzzle **Bat**
   - On bat's Interactable **On Interact**, connect to TutorialPuzzle > InteractWithBat
4. **Door setup:**
   - Create door sprite, add Box Collider 2D, Layer Interactable, Interactable
   - Add LockedDoor to the door (for room transition after window breaks)
   - Drag door into TutorialPuzzle **Door**
   - Assign **Broken Door Sprite**, **Background Renderer**, **Broken Window Background**
   - Assign **Locked Door** (the LockedDoor on the same door)
   - On door's Interactable **On Interact**, connect to TutorialPuzzle > InteractWithDoor
5. Create 4 DialogueAssets and assign:
   - Door Locked Dialogue, Need Bat Dialogue, Bat Picked Up Dialogue, Door Opened Dialogue
6. Assign **Dialogue Runner** and **Tutorial Complete Flag**

**For window-break sound:** Use TutorialPuzzle's **On Door Opened** event; connect to AudioSource > PlayOneShot with "Breaking Window" clip.

### VendingMachinePuzzle.cs

**What Does It Do?**  
Pierce's hand gets stuck. Fullscreen arm image shakes. Click fast to escape. The puzzle can only be played **once**; after it is completed, clicking the vending machine does nothing.

**How to Hook It Up**

1. Create vending machine sprite
2. Add Box Collider 2D, Layer Interactable, Interactable
3. Add VendingMachinePuzzle
4. Create fullscreen arm:
   - In HUDCanvas, create UI > Image "FullscreenArmImage"
   - Set Source Image to arm sprite
   - Stretch to fill (Alt + bottom-right anchor), set L/R/T/B to 0
   - DISABLE it
5. Create black background image (optional, for fade)
6. Create hand reveal image (optional, shows after escape)
7. Assign:
   - **Fullscreen Arm Image**
   - **Black Background Image** (optional)
   - **Hand Reveal Image** (optional)
   - **Prompt UI** (ButtonMashPromptUI)
   - **Break Room Unlocked Flag**
   - **Max Shake Intensity**: 20
8. On Interactable **On Interact**, connect to VendingMachinePuzzle > StartPuzzle

---

## 9. Rooms Folder Scripts

**Location:** Assets > Scripts > Rooms

### RoomDefinition.cs

**What Does It Do?**  
Defines a room: ID, spawn points, camera anchor, room root, floor collider, bounds, and optional **room angle**. Used by RoomManager and LockedDoor. When you enter a room, RoomManager can set the camera position/rotation from the anchor and angle.

**How to Hook It Up**

1. Create empty "Room_OfficeName"
2. Put all room objects as children (background, props, NPCs, doors)
3. Add RoomDefinition
4. Set **Room Id** (e.g. "Office", "Hallway")
5. Assign **Room Root** (root GameObject for this room)
6. Assign **Spawn Left** and **Spawn Right** (Transforms where Pierce appears)
7. Assign **Floor Collider** (BoxCollider2D for walk bounds)
8. Optionally assign **Camera Anchor**, **Room Bounds**, **Background**
9. **Room angle:** Set **Camera Angle Degrees** (0–360, e.g. 0, 90, 180, 270) to rotate the camera when entering this room. This is applied only when **Camera Anchor** is also assigned; RoomManager sets the camera’s Z rotation to this value when entering the room.

### RoomManager.cs

**What Does It Do?**  
Singleton that manages room transitions. Activates/deactivates rooms, moves the player, updates camera and click bounds. Use DoorInteractable or LockedDoor to trigger transitions.

**How to Hook It Up**

1. Create Empty "RoomManager"
2. Add RoomManager
3. Assign **Player** transform
4. Assign **Main Camera**
5. Assign **Click Controller** (ClickController2D)
6. Assign **Player Mover** (PlayerMover2D) or leave unset to use PointClickController
7. Set **Initial Room Id** to the roomId of your starting room (e.g. "Office" for the demo). Must match a **Room Id** on one of the RoomDefinitions in your **Rooms** list.
8. Add all RoomDefinitions to **Rooms** list
9. Optionally assign **Page Turn Transition** for room-change animation (captures the screen, peels it away to reveal the new room)

**PageTurnTransition setup:** Assign a RawImage (fullscreen under a Canvas) to **Capture Display**. The script captures the current view, switches rooms, then peels the captured image away. Optionally assign **Target Canvas**, **Capture Camera**, **Page Peel Material** (using the UI/PagePeel shader in Assets/Shaders), and **Transition Sound** (AudioClip played once when each transition starts; uses an AudioSource on the same GameObject, adding one if missing).

**Note:** Only ONE RoomManager per scene. RoomManager.Instance is used by LockedDoor and DoorInteractable.

---

## 10. Demo-Specific Folder Scripts

**Location:** Assets > Scripts > ScriptDocumentation > Demo-Specific

### DemoEndSequence.cs

**What Does It Do?**  
Plays the demo ending: fade to black, "To be continued...", return to main menu. Uses SceneFader if present, otherwise fades manually.

**How to Hook It Up**

1. In HUDCanvas, create UI > Image "FadeImage"
   - Color BLACK, alpha 0, stretch to fill, DISABLE
2. Create UI > Text - TextMeshPro "ToBeContinuedText"
   - Type "To be continued...", color white, alpha 0, center, DISABLE
3. Create Empty "DemoEndManager"
4. Add DemoEndSequence
5. Assign **Fade Image** (or **Fade Canvas Group**), **Tmp Text** (or **Legacy Text**)
6. Set **Fade Duration**, **Text Display Duration**, **Main Menu Scene Name**
7. On the janitor's DialogueInteraction **On Dialogue Ended**, connect to DemoEndSequence > PlayEndSequence

### FadeGroup.cs

**What Does It Do?**  
Fades sprites in/out. Useful for objects appearing or disappearing.

**How to Hook It Up**  
Add FadeGroup to any object with sprites. Assign SpriteRenderers or leave empty to auto-find. Set Fade Duration. Call FadeOut() / FadeIn().

---

## 11. Audio

### Overview

| Type | Description | Where |
|------|-------------|-------|
| Music | Background music | Main Menu, In-Game |
| SFX | Event sounds | Various gameplay moments |

### Event Flags for Window Break Sound

Use TutorialPuzzle's **On Door Opened** event. Add AudioSource.PlayOneShot with "Breaking Window" clip.

### Vending Machine Shake Sound

Use VendingMachinePuzzle's **On Puzzle Started** (AudioSource.Play) and **On Puzzle Completed** (AudioSource.Stop) with "Shaking Machine" clip.

### Footsteps

See FootstepLoop in Section 7. Assign "Dress Shoes Walk" to Footstep Clip.

### Room Transition (Page Turn)

Assign **Transition Sound** on PageTurnTransition to play an audio clip once when each room transition starts (e.g. page-turn or paper rustle SFX).

### Jukebox.cs

**What Does It Do?**  
Background music system. Picks a random track from a list, plays it, then when it ends picks another (with optional gap and avoid-repeat). AudioSource is required and is added automatically by the component.

**How to Hook It Up**

1. Create an empty GameObject named e.g. "Jukebox"
2. Add the **Jukebox** component (AudioSource is added automatically)
3. Drag music clips into **Tracks**
4. **Volume**: 0–1 (default 0.7)
5. **Gap Between Tracks**: Seconds of silence between songs (0 = no gap)
6. **Avoid Repeat**: If true, the same track won't play back-to-back

**Other behavior / API**

- **PlayNext()** – Plays a random track (respects Avoid Repeat)
- **Stop()** – Stops playback and clears state
- **FadeOut(duration)** – Smoothly fades volume to zero over the given seconds, then stops. **UIController** already uses this when returning to the main menu

---

## 12. Complete Demo Walkthrough

### Part 1: Tutorial Room (Pierce's Office)

1. Pierce starts trapped. Door is locked.
2. Click door → "It's locked"
3. Click door again → "I need something to break this"
4. Click bat → Pierce picks it up (bat disappears)
5. Click door → Pierce breaks the window
6. TutorialComplete flag turns ON
7. Click door again → LockedDoor.UseDoor() transitions to hallway

### Part 2: Hallway

- Talk to Carmen, Janitor
- Click vending machine → button mash puzzle
- After puzzle: BreakRoomUnlocked ON, break room door works

### Part 3: Break Room

- Search trash can, desk, etc.
- One has keys → HasKeys ON

### Part 4: Ending

- Return to Janitor; HasKeys ON → new dialogue
- On Dialogue Ended → DemoEndSequence plays
- Fade to black → "To be continued..." → Main menu

---

## 13. Troubleshooting

### Pierce Won't Move

- Is PointClickController on Pierce?
- Is ClickController2D present and driving Pierce? (inputHandledExternally = true)
- Did you create the "Walkable" layer?
- Is WalkableArea on "Walkable" with a Collider2D?
- Is RoomManager assigning walkBounds to ClickController2D?

### Can't Click on Objects

- Does the object have a Collider2D?
- Is the object's Layer "Interactable"?
- Is "Interactable" in ClickController2D's Interactable Mask (or PointClickController's Interactable Layer)?

### Nothing Happens When Clicking

- Does the object have Interactable (or DoorInteractable)?
- Is **On Interact** connected to something?

### Dialogue Doesn't Start

- Is there a DialogueRunner in the scene?
- Is DialogueRunner assigned in DialogueInteraction?
- Is a DialogueAsset assigned?

### Door Doesn't Work

- Does the target RoomDefinition exist in RoomManager.rooms?
- Does DoorInteractable **Target Room Id** (or LockedDoor **Target Room**) match RoomDefinition **Room Id**?
- Do spawn point names match ("Left" / "Right")? LockedDoor uses **Target Entry Point Name**.

### Locked Door Always Locked

- Is the EventFlag turning ON?
- Is **Require Flag True** checked?
- Is **Reset On Play** checked on the flag?

### Event Flag Not Working

- Does the EventFlag asset exist?
- Is it assigned in the correct slot?
- Are you testing in Play mode?

### Wrong Starting Room or Pierce Spawns in Wrong Place

- In RoomManager, **Initial Room Id** must exactly match the **Room Id** of one of the RoomDefinitions in the **Rooms** list (e.g. "Office", not "Pierce" unless you have a room with that id).

### Door Cursor Wrong or Missing Arrow

- On each DoorInteractable, set **Door Direction** (Left, Right, or Wall)
- On AdventureHUDController, assign **Door Cursor Left Sprite**, **Door Cursor Right Sprite**, and **Door Cursor Up Sprite** (or at least **Door Cursor Sprite** as fallback)

### No Highlight Feedback (Pierce Animation / Object Glow / Sound)

- Add **HoverFeedback** to the interactable GameObject (or a child)
- Assign at least one of: **Pierce Animation Driver**, **Sprite Renderer** + **Highlight Sprite**, **Object Animator** + **Object Hover Trigger**, or **Hover Sound**
- For Pierce reaction: in Pierce's Animator add a trigger (e.g. "HighlightReact") and set **Highlight React Trigger** on PierceAnimationDriver to that name
