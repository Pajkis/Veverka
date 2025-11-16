# Changelog v0.14.2

### 📅 Date: `2025-11-15`
### 🔖 Git Version: `v0.14.2`
### 📦 Area: `[Tiles / Visuals]`
### 📝 Description:
Minor update adding **Stone Road Tile** variant for visual variety and fixing rendering layer bug.

### ➕ Added:
- Stone road tile variant (Tile_RoadStone.prefab) with new sprite
- Updated 01_TutorialRoad level to showcase new tile type

### ♻️ Updated / Refactored:
- Tile_Road and Tile_Background prefabs - minor adjustments for consistency

### 🛠️ Bug Fixes:
- [[Scene_GamePlay]] - Fixed rendering layer configuration where tiles were displaying over game frame
- TagManager - Corrected layer settings for proper Z-ordering

### 🔥 Removed:
*None*

---

# Changelog v0.14.1

### 📅 Date: `2025-11-01`
### 🔖 Git Version: `v0.14.1`
### 📦 Area: `[Scene Transitions / Audio System / UI]`
### 📝 Description:
Quality-of-life update introducing **Smooth Scene Transitions & Overlay Animations** with configurable fade effects, **Smart Music Management** preventing unwanted song changes, and **Audio Feedback** for UI overlays.

### ➕ Added:
**Scene Transitions & Overlay Animations:**
- Smooth fade transitions between scenes with configurable durations, animation curves, and colors
- Animated overlay open/close with fade effects
- Per-scene and per-overlay transition settings with default + override pattern
- Music crossfade integration (only triggers when crossing Menu↔Game boundaries)
- Unscaled time support (transitions work during pause with Time.timeScale = 0)
- Independent fade-in/fade-out enable flags for instant transitions when needed

**Audio System Enhancements:**
- Smart music management: prevents song changes when staying within same music type (Menu or Game)
- Audio feedback for notifications (UiType.Notification)
- Audio feedback for validation errors (UiType.Error)
- Audio feedback for level completion (SfxType.LevelFinished)

### ♻️ Updated / Refactored:
**Scene Flow:**
- Scene transitions now use fade effects instead of instant loading
- Overlays open/close with smooth animations
- Music only crossfades when switching between Menu and Game music types

**Singleton Persistence:**
- Removed DontDestroyOnLoad from child components (rely on parent GameObject)
- Cleaner hierarchy without duplicate persistence calls

**Config Organization:**
- Moved all config ScriptableObjects to Data/Config folders for better project structure

### 🛠️ BugFixes:
- Fixed music changing unnecessarily when transitioning between scenes with same MusicType
- Fixed screen staying black when both fade-in and fade-out disabled
- Fixed DontDestroyOnLoad warnings for child GameObjects

### 🔥 Removed:
*None*

---

# Changelog v0.14.0

### 📅 Date: `2025-10-19`
### 🔖 Git Version: `**v0.14.0**`
### 📦 Area: `[User Levels / Grid System / Notifications / Tiles / Debug System / UI]`
### 📝 Description:
Major feature release introducing **User-Created Levels** with import/export, complete **Grid System Refactor** for modularity, **Notification System** for user feedback, **Obstacle System** unification, new **Stone Road Tiles**, comprehensive **Level Validation**, and **Debug Logger** integration across all systems.

### ➕ Added:
**User Level System:**
- User-created level support with persistent storage in Application.persistentDataPath
- Import/export functionality with unified path: Documents/GetNuts on both platforms
- SimpleFileBrowser for cross-platform file picker (Windows, Android, Editor)
- Android Content URI support for Scoped Storage compatibility
- Android Auto Backup disabled via AndroidManifest to prevent data corruption
- UserLevelManager, UserLevelSet, UserLevelInitializer for level management
- LevelInitData and LevelSelection ScriptableObjects for level configuration
- UserLevelManual.txt v1.2 with CSV format specification and data loss warnings

**Grid System Refactor:**
- Extracted GameGrid into modular components: GridBuilder, GridData, GridRuntime
- New TileParser system with configurable tile symbol mapping via TileParsingConfig SO
- Improved grid construction, data storage, and runtime operations
- Enhanced GridUtils with expanded utility functions

**Notification System:**
- NotificationOverlay with NotificationManager for user feedback
- ErrorCode enum (37 codes) with ErrorMessages for user-friendly error display
- NotificationType enum (Success, Error, Info)
- Visual notification sprites and UI components

**Level Validation:**
- LevelValidationOverlay with LevelValidationErrorManager
- Row length mismatch detection for CSV format validation
- Comprehensive error reporting with visual feedback

**Tile System:**
- ObstacleTile base class for unified obstacle handling
- Stone road tiles (Tile_RoadStone prefab with sprite)
- Stone-filled hole tiles (Tile_HoleStone sprite)

**Debug System:**
- DebugLoggerInitializer for global debug logger initialization
- Category-based logging integrated across 20+ scripts (replacing Debug.Log)
- Enhanced DebugLogConfig with reorganized categories

**UI Components:**
- HideOnTabActive and TabDependentVisibility for tab-based UI control
- Simple menu button prefab with sprites
- Enhanced tab management system

### ♻️ Updated / Refactored:
**Grid System:**
- GameGrid refactored from monolithic class to delegating architecture (GridBuilder/GridData/GridRuntime)
- Enhanced GridUtils and GridUtilsTests with improved functionality and test coverage

**Obstacle System:**
- Unified HoleTile and WaterHoleTile under ObstacleTile base class
- Moved obstacle scripts from GoalTiles/ to ObstacleTiles/ folder
- Renamed WallEvents → ObstacleEvents, WallEventPayload → ObstacleEventPayload
- Reorganized obstacle sprites from Goals/ and Wall/ folders to unified Obstacle/ folder

**Goal System:**
- Refactored BasicGoalTile and GoalTile for improved goal resolution
- Enhanced GoalEventPayload structure

**Level System:**
- Enhanced LevelLoader and LevelUnloader with improved error handling
- Added User level set type to LevelSetType enum
- Major UI overhaul in LevelSelect scene

**Scene Management:**
- Renamed LevelLoad → LevelTransition scene
- Added NotificationOverlay and LevelValidationOverlay to OverlayType enum
- Enhanced Bootstrap scene initialization

**Character & Turn System:**
- Integrated DebugLogger across Character, TurnControl systems
- Removed obsolete Debug.Log calls from TurnRecorder and UndoController

**UI System:**
- Enhanced UniversalTabManager, RandomTipsDisplay, LevelSelectMenu
- Updated menu systems (LevelFinishedMenu, InGameMenu, PauseMenu)

**Sprite Organization:**
- Renamed InGameBorder_* → Frame_* for all background frames
- Renamed ButtonMenuCommon → ButtonMenuNutLeafs
- Renamed TipMessageFrame → Frame_TipMessage
- Renamed Shared/UI/Sprites/ → Shared/UI/SpriteSupport/
- Optimized nut sprites (BasicNut, StoneNut, 14 water nuts - avg 30% size reduction)

**Android:**
- Updated minimum and target SDK versions

**Debug Integration:**
- Replaced Debug.Log with DebugLogger across BubbleMessageManager, Character, GamePlay, AudioManager, UIButtonAudioHook, CameraFollow, DisplaySettings

## 🛠️ BugFixes:
- Fixed level row length mismatches in CSV parsing
- Fixed level validation inconsistencies and improved error reporting
- Fixed level loading/unloading flow issues
- Fixed tile parsing edge cases and grid query performance
- Fixed tab visibility state inconsistencies
- Fixed button state handling and overlay display timing
- Fixed duplicate sprite references and sprite import settings

### 🔥 Removed:
- LevelDatabase.cs/asset replaced by LevelInitData
- WallEventPayload replaced by ObstacleEventPayload
- WallEvents.asset replaced by ObstacleEvents.asset
- LevelUnload.unity scene removed from build
- Obsolete Debug.Log calls from TurnRecorder, UndoController
- Temporary button sprites (Button_NotPressed_Temp, Button_Pressed_Temp)
- Obsolete frame sprite (InGameBorder_3)
- Redundant sprite assets cleaned up

# Changelog v0.13.0

### **📅 Date: `2025-09-26`**

### **🔖 Git Version: `v0.13.0`**

### **📦 Area:**

**[Water Mechanics / Project Structure / Debug System / Event System / Level Design / Testing]**

### **📝 Description:**

 Major milestone introducing **Water Set mechanics** with water nuts, water holes, and splash physics. Complete project structure refactor for better organization, comprehensive debug logging system, extensive event system unification, and robust testing framework. Added 10 new water-themed levels and improved development workflow with Notion documentation integration.

---

### **➕ Added:**

- **Water Set Game Mechanics**
    - Water nut tiles with splash physics and push interactions
    - Water hole goal tiles with specialized interaction logic
    - Water splash push mechanics for strategic gameplay
    - 10 new water-themed levels including tutorial level
    - Water level set integration in level select menu
- **Debug & Logging System**
    - Centralized debug logging system across all components
    - GamePlay debug logging for gameplay events
    - Settings debug logging for configuration tracking
    - Extensive event tracking and state monitoring
    - Scene management flow debugging
- **Testing Framework**
    - Comprehensive GridUtils test suite
    - Test assembly definitions for editor and runtime
    - Unit tests for grid logic and utilities
    - Test infrastructure for gameplay mechanics
- **Level Design & Content**
    - New bubble message types for enhanced player communication
    - Random tips display system with rotating hints
    - Enhanced goal-related sound effects and audio logic
    - Improved bubble message display for character interactions
- **Event System Enhancements**
    - Enhanced GoalPayload with additional data fields
    - Character data request events for state management
    - Unified nut event system with comprehensive payloads
    - Improved turn recording and undo system integration
    - Event-based undo system with better state tracking
- **Development Tools**
    - Notion documentation analysis scripts
    - Unity to Notion documentation comparison tools
    - Automated documentation workflow integration
    - Development workflow improvements

---

### **♻️ Updated / Refactored:**

- **Project Structure (Major Overhaul)**
    - Assets reorganized into Core folder structure
    - UI-related assets moved to shared locations
    - Level and turn-related assets consolidated
    - Audio folders renamed to 'Clips' for clarity
    - Scene renaming for consistency (numerical prefixes)
    - GamePlay folder structure for shared assets
- **Event System Unification**
    - Unified character event system across all character types
    - Consolidated grid events for better organization
    - Merged grid build events into single system
    - Unified nut events with comprehensive payloads
    - Unified settings events for configuration management
    - Unified road and wall handling events
    - Unified audio events for consistent sound management
    - Unified scene navigation events
- **Character & Movement System**
    - Character rotation animation implementation
    - Enhanced character movement event handling
    - Improved character and grid logic integration
    - Character and nut pushing mechanics refinement
    - Turn control refactored to remove singleton pattern
- **Grid & Gameplay Logic**
    - Goal tile logic refactoring for water mechanics
    - Game grid and nut interaction improvements
    - Grid surroundings and camera bounds enhancement
    - Camera initialization after character spawn
    - Level loading and scene management improvements
- **Configuration & Settings**
    - Game settings configuration refactoring
    - Display configuration consolidated into single SO
    - Gameplay config implementation for consistent rules
    - Music playback with fade and delay improvements

---

### **🛠️ BugFixes:**

- **Critical Fixes**
    - StackOverflow fix: Recursive call prevention in SetEvents Init
    - Water splash push logic corrections
    - Scene loading and grid check fixes
    - Android pause and level finished background resolution fix
    - Wrong event assignment to TurnController correction
- **Stability Improvements**
    - Grid check logic improvements
    - Level data consistency fixes
    - Event initialization order fixes
    - Memory management improvements in water mechanics

---

### **🔥 Removed:**

- **Cleanup & Optimization**
    - Unused overlay prefabs removal
    - Unnecessary dotnet workflows removal
    - Obsolete changelog modifications cleanup
    - Addressables removed from git tracking
    - Legacy project structure artifacts

---

### **🔧 Technical Infrastructure:**

- **None Enum Members**: Added None member for Goal/Nut/Road/Wall enums for better null handling
- **Debug Logging**: Comprehensive logging system for all major game systems
- **Event Payload Enhancement**: Improved data structures for event communication
- **Test Coverage**: Extensive test suite for grid utilities and core mechanics
- **Documentation Integration**: Notion API integration for automated documentation workflow

---

### **🎮 Gameplay Features:**

- **Water Mechanics**: Complete water-based puzzle mechanics with physics
- **Level Progression**: 10 new challenging water levels
- **Audio Enhancement**: Goal-specific sound effects and undo move sounds
- **User Experience**: Enhanced bubble messages and tip system
- **Turn System**: Improved turn recording with better undo functionality

---

# Veverka Changelog v0.12.0

### 📅 Date: `2025-09-04`

### 🔖 Git Version: `v0.12.0`

### 📦 Area: `[TileObjects / Prefabs / UI / Overlays / Levels / GridSystem / Payloads / TurnSystem / InGame]`

### 📝 Description:

Major refactor of NutTile / GoalTile hierarchy, simplification of payloads
Add new game mechanics, holes and stone nuts. Create stone set with 10 new levels.
Unified visuals by universal UI prefabs, Unified background for Loadings screens and some menus.
Grid utilities consolidated and obsolete scripts removed.

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

Major refactor introducing event-driven GameSettings, decoupling of GameGrid from CameraFollow,
and new **UI/Settings components** (sliders, dropdowns, layout manager).
Added more flexible level loading, settings overlay improvements, and cross-platform UI layouts.

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
    - `SceneNavigationEvent`
- **Payload Documentation**
    - `AudioVolumePayload` (volume adjustment).
- **Enum Documentation**
    - `OverlayType` (overlay identification).

---

### ♻️ Updated / Refactored:

- **SceneFlow System**
    - Replaced hardcoded string-based loading with **Addressable `AssetReference`** via `SceneRefProvider`.
    - Overlays are now instantiated through `OverlayPrefabProvider` instead of `Resources.Load`.
    - `SceneFlowManager` rewritten as a `MonoBehaviour` with event listener (`SceneNavigationEvent`) instead of static methods.
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

