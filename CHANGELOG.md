# Veverka Changelog v0.12.0

### 📅 Date: `2025-09-04`

### 🔖 Git Version: `v0.12.0`

### 📦 Area: `[TileObjects / Prefabs / UI / Overlays / Levels / GridSystem / Payloads / TurnSystem / InGame]`

### 📝 Description:

> Major refactor of NutTile / GoalTile hierarchy, simplification of payloads
Add new game mechanics, holes and stone nuts. Create stone set with 10 new levels.
Unified visuals by universal UI prefabs, Unified background for Loadings screens and some menus.
> 
> 
> Grid utilities consolidated and obsolete scripts removed.
> 

---

### ➕ Added:

- **Fonts**
    - Google fonts: Piedra, KneWave, Sedgwick Ave, Syne Tactile
- **UI / Overlays**
    - `CreditsOverlay` accessible from `10_MainMenu`
    - Added `How to Play` button on `10_MainMenu`
    - Universal prefabs for buttons and labels:
        - `ButtonMenuCommon` – standard menu button
        - `ButtonMenuBack` – back button with automatic overlay closing (`CloseOverlay`)
        - `ButtonLevelSelect` – redesigned level select button with in-game nut sprite
        - Caption/label text prefabs with preset font & size
    - `RandomTipsDisplay` prefab for displaying hints, tips and messages.
- **TileObjects**
    - `HoleTile` as child of `GoalTile`
    - `StoneNutTile` as child of `NutTile`
- **Levels**
    - Created **10 levels for Stone Set**
- **Scripts**
    - `UniversalTextDisplay` – replaces sprite number display, supports numbers, strings, enums
    - `RandomTipsDisplay` – rotating random tips/hints with fade in/out
    - `UniversalTabManager` – unified tab system (used in GameHelp and LevelSelect)
- **InGame**
    - Bubble messages when game starts and after goal is hit
    - **BubbleMsgBox** – prefab script for displaying and fading character speech bubbles.
    - **BubbleMessageManager** – system for handling and positioning bubble messages.
    - **PulsatingSprite** – pulsates scale & brightness using sine waves.
    - **CharVeverka, GoalTiles -**  added event handle and logic for receiving requests and sending data for bubble boxes
- **Events**
    - **CharacterDataRequestEvent** – new event for requesting/returning character state.

---

### ♻️ Updated / Refactored:

- **TileObject hierarchy**
    - Removed `MovableTile`, `PushableTile`, `StaticTile`
    - Introduced abstract `NutTile` and `GoalTile` classes (direct children of `TileObject`)
    - `BasicNutTile` (former NutTile) and `StoneNutTile` as children of `NutTile`
    - `BasicGoalTile` (former GoalTile) and `HoleTile` as children of `GoalTile`
    - Renamed *Pushable/Movable* terminology to **Nut**
- **Payloads**
    - Unified nut payloads into `NutBasicPayload`
    - Unified goal payloads into `GoalBasicPayload`
    - Unified Character payloads into `CharacterBasicPay`load
- **Overlays & Scenes**
    - Updated all overlays with new prefabs
    - Added support for new back button prefab integration
    - Refactor HelpMenu - add several tabs
    - Huge refactor of LevelSelectMenu, visualy and functionaly. Add tabs and selection of levels based on current tab and nut number.
    - Huge refactor of visuals, unified backgrounds for all loading screens and navigation menus (main, pause etc.)
    - Changed inGame level/Goal/turns sprites and sprite number display with TextMeshPro texts
- **Turn System**
    - Refactored `TurnBuilder` – removed `StartNewTurn`, turn start generalized beyond character movement
- **GameGrid.cs & GridUtils.cs**
    - Tile checks and queries from `TileTypeExtensions.cs` merged into `GameGrid`
    - Level parsing from `LevelUtils` merged into `GridUtils`
    - NutTiles and GoalTiles dictionaries replaced by `NutType[,]` and `GoalType[,]` arrays
    - Simplified tile query logic (removed specialized methods like `IsObstacle`, `IsGoal`)
- **Events**
    - Merged `BuildGrid` and `BuildGridDone` into a single `BuildGrid` event with `BuildDone` flag.

---

### 🔥 Removed:

- Obsolete button and tile sprites, backgrounds, labels etc.
- `TileTypeExtensions.cs`
- `LevelUtils.cs`

---

# Veverka Changelog v0.11.0

### 📅 Date: `2025-08-26`

### 🔖 Git Version: `v0.11.0`

### 📦 Area: `[GameGrid/ LevelLoad / CameraFollow / Events / Levels / GameSettings / UI]`

### 📝 Description:

> Major refactor introducing event-driven GameSettings, decoupling of GameGrid from CameraFollow,
> 
> 
> and new **UI/Settings components** (sliders, dropdowns, layout manager).
> 
> Added more flexible level loading, settings overlay improvements, and cross-platform UI layouts.
> 

---

### ➕ Added:

- **Events**
    - `BuildGridEvent` and `BuildGridDoneEvent` for level building during loading screen.
    - `SettingDataRequestEvent` and `SettingDataBroadcastEvent` with `SettingDataPayload` to manage all `GameSettings` communication.
- **GameSettings / Enums**
    - Added `LayoutStyle` into `GameSettingsEnum`.
    - New enum `LayoutStyleTypes` (`WindowsRight`, `AndroidRight`) for platform-specific UI control.
- **UI / Managers**
    - `LayoutStyleManager` to apply layout style at runtime based on `GameSettings`.
    - Universal `DropDownSettingItem` prefab + script to support any `GameSettingsEnum` with enum selection.
    - Extended settings overlay UI: separated into **Audio / Game / UI columns**.
- **Gameplay / Levels**
    - Levels now ordered by difficulty.
    - Added extra goals and nuts in **Level 8** and **Level 9**.

---

### ♻️ Updated / Refactored:

### Level build refactor

- Moved **level building** from `30_GamePlay` → `20_LevelLoad` scene.
- Added events `BuildGridEvent` / `BuildGridDoneEvent` to trigger grid init and notify completion (replacing `levelInitEvent`).
- Introduced **LevelDatabase** to store grid size, goals, origin transforms, and character references.
- Removed direct `LevelBuild` trigger from gameplay.
- `CameraFollow` now initializes itself at scene awake using shared data in `LevelDatabase`.
- Replaced `levelInitPayload` with shared **LevelDatabase** across scenes (`GamePlay`, `LevelLoad`, `CameraFollow`).

### GameSettings.cs refactor

- Migrated from **direct calls** → **event-driven system**.
- `SettingDataRequestEvent` + `SettingDataBroadcastEvent` now handle all communication.
- Validation and persistence remain centralized in `GameSettings`.
- Updated related scripts to integrate with the new event-driven settings system:
    - **`AudioManager**` (volume, pitch)
    - **`EnvironmentSounds**` (ambient effects volume)
    - **`Characters**` + `CharVeverka` (movement speed from settings)
    - **`TileObject**` and **`MovableTile**` (movement speed + undo integration)
    - **`SmoothMover**` (movement duration scaling)
    - **`SliderSettingItem**` (UI slider → GameSettings binding)
    - **`DropDownSettingItem**` (new generic enum dropdown binding)

### Gameplay

- Difficulty rebalancing across levels.
- Larger control area for Android / Windows layouts.

### Scenes

- In-game button layout support for **Android** and **Windows** (dynamic via `LayoutStyleManager`).
- Settings overlay redesign:
    - New background
    - Split into separate **Audio / Game / UI** configuration sections.

---

### 🔥 Removed

- Removed obsolete `LevelBuild` trigger (replaced by events + `LevelDatabase`).
- Removed `levelInitEvent` and its `levelInitPayload` (replaced by `LevelDatabase`).
- Removed `AudioVolumeEvent` (replaced by `SettingDataRequestEvent` + `SettingDataBroadcastEvent`)

---

# Veverka Changelog v0.10.0

### 📅 Date: `2025-08-20`

### 🔖 Git Version: `v0.10.0`

### 📦 Area: `[Scenes / Overlays / Audio / Events / Documentation]`

### 📝 Description:

This release introduces a **major documentation milestone** together with **refactoring of scene and overlay flow**, **audio system upgrades**, and **event-based architecture integration**.

All core scripts, events, payloads, enums, overlays, and managers have been fully documented using standardized templates (`Class`, `Event`, `Payload`, `Enum`).

The system now uses `SceneRefProvider` and `OverlayPrefabProvider` for references, with `SceneFlowManager` centralizing navigation via events.

Audio handling has been reworked with `AudioManager` subscribing to dedicated events and driven by `AudioConfig` SO for consistent configuration.

---

### ➕ Added:

- **Scene Documentation**
    - Completed full documentation for scenes:
        - `10_MainMenu`, `11_LevelMenu`, `20_LevelLoad`, `21_LevelUnload`, `30_GamePlay`.
    - Added overlay UI documentation: `GameSettings`, `PauseMenu`, `LevelFinishedMenu`, `SettingsMenu`, `GameHelp`.
- **Class Documentation**
    - `OverlayPrefabProvider` (central overlay prefab provider).
    - `SceneRefProvider` (Addressable scene reference provider).
    - `SceneFlowManager` (event-based scene and overlay manager).
    - `EnvironmentSounds` (ambient random sound system).
    - `AudioConfig` (SO for audio balancing).
    - `AudioManager` (reworked with events + config).
- **Event Documentation**
    - `AudioInitEvent`
    - `AudioVolumeEvent`
    - `PlayMusicEvent`
    - `PlaySfxEvent`
    - `PlayUiEvent`
    - `GoToSceneEvent`
    - `OpenOverlayEvent`
- **Payload Documentation**
    - `AudioVolumePayload` (volume adjustment).
- **Enum Documentation**
    - `OverlayType` (overlay identification).

---

### ♻️ Updated / Refactored:

- **SceneFlow System**
    - Replaced hardcoded string-based loading with **Addressable `AssetReference`** via `SceneRefProvider`.
    - Overlays are now instantiated through `OverlayPrefabProvider` instead of `Resources.Load`.
    - `SceneFlowManager` rewritten as a `MonoBehaviour` with event listeners (`GoToSceneEvent`, `OpenOverlayEvent`) instead of static methods.
- **Audio System**
    - `AudioManager` upgraded:
        - Subscribes to `PlaySfxEvent`, `PlayUiEvent`, `PlayMusicEvent`, `AudioInitEvent`, and `AudioVolumeEvent`.
        - Uses `AudioConfig` for volume and pitch adjustments.
        - Smarter random music playback with repeat-avoidance logic.
        - Full documentation applied.
    - `EnvironmentSounds` updated to fetch delays and volumes from `AudioConfig` and `GameSettings`.

---

### 🔥 Removed:

- Hardcoded **scene name strings** in scene navigation.
- **Resources folder dependency** for overlay prefabs.
- Legacy static version of `SceneFlowManager`.

---

# Veverka Changelog v0.9.0
---

### 📅 Date `2025-08-14`

### 🔖 Git Version `v0.9.0`

## 📦 Area

Configs / UI / GameSettings / Characters/ Tiles

## 📝 Description

This release introduces **ScriptableObject-driven configuration** to replace magic numbers and scattered constants, a **unified display pipeline** (including Android‑specific UI matching), and completes work for **Android 13+ deployment**. The result is cleaner code, faster iteration, and more reliable cross‑scene visuals on desktop and mobile.

## ➕ Added

- **Config SOs to centralize defaults**
    - `InGameDisplayConfig` for grid/tile & camera offsets/smoothing.
    - `DisplayConfig` + `DisplaySettings` runtime applier (VSync/FPS, orientation, letterbox/pillarbox, UI scalers).
    - `CanvasMatchOnAndroid` (Android‑only) to auto-pick `CanvasScaler.matchWidthOrHeight` by device aspect.
    - `GameSettingsConfig` for default user settings (volumes, animation speed, active player).
    - `GameplayConfig` for gameplay rules (max undo steps, move time, scoring adjustments).
- **Android 13+ (API 33+) readiness**
    - Project updated for current Android target with runtime aligned display handling and per‑canvas UI matching.

---

## ♻️ Updated / Refactored

- **Camera system**
    - `CameraFollow` now sources offsets, smoothing, tile size, and static thresholds from `InGameDisplayConfig`.
    - Improved centering on even-sized grids via `evenTilesOffset`.
- **Global display pipeline**
    - `DisplaySettings` persists across scenes and re‑applies display/UI after each scene load.
    - Consistent `CanvasScaler` setup across all canvases (reference resolution + match axis).
- **Settings initialization**
    - `GameSettings` integrated with `GameSettingsConfig` (defaults via SO; fallback when missing PlayerPrefs).
- **Gameplay defaults application**
    - `GameplayConfig` applied in `TileObject.cs` and `Characters.cs` to remove hard‑coded values and unify movement timing (`speed = tileSize / moveTime`) and rules.

---

## 🛠️ BugFixes

- fix bug when Grid surrounding walls not generated on last row and column on even sized grids.

## 🔥 Removed

- Scattered **magic numbers/constants** related to camera offsets, tile sizes, undo limits, and default volumes from gameplay & UI scripts (now defined in SO configs).

---

## ⚠️ Breaking Changes

- Scripts expecting hard-coded constants must now **read from the appropriate SO**:
    - Camera-related values → `InGameDisplayConfig`
    - Display/UI behavior → `DisplayConfig` (applied by `DisplaySettings`)
    - User defaults (volume, animation speed, etc.) → `GameSettingsConfig`
    - Gameplay rules (move/scoring) → `GameplayConfig`
    - Each of the config file is **only basic** **config implantation for further expansion!**

----

# Veverka Changelog v0.8.0

### 📅 Date: `2025-08-09`

### 🔖 Git Version: `v0.8.0`

### 📦 Area: `GridSystem/GameGrid`, `Events`, `Payloads`, `TileObject` ,`Character`

### 📝 Description:

> Major refactor of the GameGrid system and its event-driven connections with TileObject derivatives and gameplay systems.
> 
> 
> Completed separation of grid logic from characters and pushable/goal handling using ScriptableObject event channels.
> 

---

### ➕ Added:

- Events:
    - `PushableSetEvent`
    - `PushableRemovedEvent`
    - `GoalSetEvent`
    - `GoalRemovedEvent`
    - `TileQueryEvent`
- Synchronous `OnTileQuery` handling to fill payload with:
    - Grid boundaries check
    - Tile object reference
    - Walkable/obstacle/movable/goal status
- Scripts/Input folder for input script handling
- `ArrowKeyInputManager` in Input, unify events for input from keyboard and on screen buttons.

---

### ♻️ Updated / Refactored:

- Centralized background and surrounding wall pooling logic in `BuildLevelFromGrid` and `BuildSurroundings`. Optimized grid instantiation order for performance.
- Ensured tile type updates always synchronize `grid[,]` with `pushables` and `goals` dictionaries.
- Standardized payload field names (`Position`, object references).

---

### 🔥 Removed

- Remove event `TileObjectAt`, replaced with `TileQueryEvent` for handling tile check only and not complex grid control

---

# Veverka Changelog v0.7.0
---
### 📅 Date: `2025-08-06`

### 🔖 Git Version: `V0.7.0`

### 📦 Area: `[Scene Management / Gameplay / UI / TileObject / GameGrid]`

### 📝 Description:

> This update focuses on restructuring the scene transition system, consolidating scene control under a new `SceneManager` class, simplifying event flow, and improving UI number displays. Additionally, the core `GamePlay` class has been extended to track turn and Goal count. 
Replace direct GameGrid calls from TileObject Class and its children with events.
> 

### ➕ Added:

- `SceneType` enum to unify all gameplay/menu scene references
- `SceneManager.GoToScene(SceneType)` to centralize scene transitions
- `LevelUnloader` script: raises `ResetGridEvent` and returns to MainMenu after delay
- `SpriteNumberDisplay`: general-purpose component for displaying numeric values (e.g. turns, goals, level) using sprite digits
- Turn and goal count tracking and display in `GamePlay.cs`
- Random generation of wall tiles (trees) with `RandomSprite.cs` script
- Fill screen with walls (trees) for game layouts smaller than screen

### ♻️ Updated / Refactored:

- Upgrade visuals of tile sprites (road, veverka, nut, goal)
- Upgrade visuals for in game frame
- Add Label with game name “Veverka”
- Upgrade visuals for menus and its names
- Replaced all uses of `MenuManager` with new `SceneManager` architecture
- SettingMenu replaced with BackOnlyMenu
- Moved `LevelSelectEvent` invocation from various UI menus to `LevelLoader.cs`

### 🔥 Removed

- Event logic previously embedded in `LevelFinishMenu`, `PauseMenu`, and `LevelSelectMenu` related to level selection (`LevelSelectEvent`)  LevelSelectListener.cs was removed.

---

# Veverka Changelog v0.6.0
---
### 📅 Date: `2025-07-26`

### 🔖 Git Version: `V0.6.0`

### 📦 Area: `[Characters / GridSystem / Undo / Documentation]`

### 📝 Description:

> Major refactor and documentation update of the character and tile movement system.
> Introduced unified undo support, event-payload documentation, and full per-class Markdown docs for game architecture

---

### ➕ Added:

- `UndoCharacterAction` class for reversible character movement
- `UndoMovableAction` class for reversing movement of pushable tiles
- `SingleTurnRecord` class for grouping all actions per turn
- `TurnBuilder` singleton for managing synchronized turn tracking
- `UndoManager` class to handle turn history and undo requests
- Character event payloads (`CharacterMovedPayload`, `MovableMovedPayload`)

---

### ♻️ Updated / Refactored:

- `Character` base class now integrates undo logic and smooth mover tracking
- `CharVeverka` now raises `CharacterMovedEvent` and supports `UndoManager`
- `MovableTile` unified movement flow with `TurnBuilder` integration
- `PushableTile` grid update logic split into `OnMoveStart()` / `OnMoveComplete()`
- `NutTile` updated to call `TurnRecordAddandComplete()` after move or goal detection
- Updated all `OnMoveComplete()` methods to finalize undo action flow
- All related methods now follow consistent naming and override hierarchy

---

# Veverka Changelog v0.5.0
### 📅 Date: `2025-07-21`

### 🔖 Git Version: `V0.5.0`

### 📦 Area: `[GameGrid, Camera, inGameScene, Events, UI]`

### 📝 Description:

> The main goal of this change was to separate characters from tile object, center camera on character for big layouts and make the first step to make the game playable on android/iphones/tablets.
> 

### ➕ Added:

- add Character class to represent different type of controlled and enemy characters
- UI - Divide in game screen into game part and control/menu part - Game should be controlled by android/iphone
- Add documentation for events in Scripts/Events
- Add in game button sprites (arrow control, restart, pause, help) and in game border background
- Add DirectionEvent for change of direction of objects via events
- Add UI folder for all UI related scripts → move menu and scene support inside
- Add controls folder for in game UI control scripts →  UI folder
- Add ArrowButtonHandler for control of character via arrows on game screen control  by using DirectionEvent→ Controls folder
- Add Camera system with Camerafollow script for ingame camera handling → UI folder

### ♻️ Updated / Refactored:

- Move Veverka from tile object to Character class
- Game grid - remove center grid and replace it with camFollow call. change gridheight/width into Vector2 grid size field and property.
- Scene → rename scenes with xx_ (00) naming for relevance.

---

# Veverka Changelog v0.4.0 and lower overview
---
### 📅 Date: 2025-07-12

### 🔖 Git Version: V0.4.0

### 📦 Area: `[Scripts]`

### 📝 Description:

> This file tracks the progress of internal documentation and structural design for the game **Veverka**. It includes component documentation, refactoring decisions, and planning for future systems
> 

---

### 🧠 Core Logic

- ✅ `GameInit.cs`: Bootstraps the game — initializes `GameSettings`, `AudioManager`, and loads `MainMenu`
- ✅ `GamePlay.cs`: Listens for goal-related events, tracks win condition, and opens relevant menus

---

### 🎮 Grid System

- ✅ Role: Manages grid structure, cell content, tile behaviors, spatial queries
- ✅ Integrated with prefab instantiation and CSV-based level loading
- ✅ Supports static tiles, pushables, players, walls, and goals
- ✅ Level Loading, building and reset
- 🔁 Uses central `GameGrid` as runtime grid representation

---

### 📜 Menu System

- ✅ `MenuManager.cs`: Central navigation handler — scenes & in-scene prefabs
- ✅ `MainMenu`, `LevelMenu`, `PauseMenu`, `SettingsMenu`, `LevelFinishedMenu`
- ✅ `MenuEnum`: Type-safe navigation system

---

### 🔊 Audio System

- ✅ `AudioManager`: Manages 3 audio channels (SFX, UI, music), uses `Enum` mapping
- ✅ `AudioEnums`: `SfxEnum`, `UiEnum`, `MusicEnum`, `SoundChannel`
- ✅ `UIButtonAudioHook`: Adds click/hover sound automatically to all child buttons

---

### 🧩 Data

- ✅ `GameSettings`, `GameSettingsItem`, `GameSettingsUtils`, `GameSettingsEnum`
- ✅ `LevelDatabase`: Holds current level index (used with CSV loading)
- ✅ Level CSV format documented with symbol-to-object mapping:
    - `V` = Player (Veverka), `N` = Nut, `G` = Goal, `W` = Wall, `.` = Empty

---

### 🧩 🔄 Scene Support

- ✅ `BackgroundSpinner`: Continuous Z-axis rotation for background effects (loading, UI)

---

