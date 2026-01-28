# Implementation Plan: Tutorial Controls System
**Date**: 2026-01-26

## Overview
Přidání kompletního ovládacího systému pro tutoriály:
- Autoplay/Manual režim s Next tlačítkem
- Text speed (1x, 3x, 5x) - fade in/out + display time
- Animation speed (1x, 3x, 5x) - movement/rotation + delay mezi akcemi
- UI zobrazení rychlostí a režimu
- Perzistence nastavení mezi tutoriály
- Restart s fade away → fade in

## Dotčené Oblasti
- **Tutorial Settings System** - vlastní persistence (PlayerPrefs s "Tutorial_" prefix)
- **TutorialActionSequencer** - aplikace rychlostí, manual mode logika
- **CharVeverka** - speed control pro animace
- **GridBuilder** - fade out pro restart
- **TutorialOverlay UI** - ovládací tlačítka a text displays

## Architectural Decision: Separate Tutorial Settings
Tutorial settings jsou **oddělené od GameSettings** pro lepší:
- **Separation of Concerns** - Tutorial je self-contained modul
- **User Experience** - Settings se mění v tutorial overlay, ne v settings menu
- **Flexibility** - Možnost různých settings pro různé tutorial sety
- **Clean Code** - Tutorial folder obsahuje vše co tutorial potřebuje

## Kritické Soubory

### Modifikace Existujících:
1. [TutorialActionSequencer.cs](Assets/Systems/Tutorial/Scripts/TutorialActionSequencer.cs) - hlavní logika, speed application, manual mode
2. [CharVeverka.cs](Assets/Gameplay/Characters/Scripts/CharVeverka.cs) - animation speed pro movement/rotation
3. [GridBuilder.cs](Assets/Gameplay/Grid/Scripts/GridBuilder.cs) - fade out method pro restart
4. [TutorialOverlay.prefab](Assets/Systems/Tutorial/Prefabs/TutorialOverlay.prefab) - UI controls (Unity Inspector)

### Nové Soubory:
5. **TutorialSettings.cs** - ScriptableObject singleton pro tutorial preferences (autoplay, speeds)
6. **TutorialSpeedConfig.cs** - ScriptableObject s speed multipliers (1.0f, 3.0f, 5.0f)
7. **TutorialSpeedMultiplier.cs** - static helper pro aplikaci speed calculations
8. **TutorialControlPanel.cs** - MonoBehaviour pro UI controls logic

---

## Implementační Pořadí

### STAGE 1: Settings & Data (Persistence)

#### 1.1 TutorialSettings (NOVÝ)
**Lokace**: `Assets/Systems/Tutorial/Data/Settings/TutorialSettings.cs`

**Co vytvořit**:
- ScriptableObject class (singleton pattern)
- Static Instance property s lazy initialization
- Fields:
  - `autoplayEnabled` (bool, default true)
  - `textSpeedIndex` (int, default 0 = 1x)
  - `animSpeedIndex` (int, default 0 = 1x)
- Methods:
  - `LoadFromPlayerPrefs()` - načte z PlayerPrefs při startu
  - `SaveToPlayerPrefs()` - uloží změny
  - `SetAutoplay(bool value)` - set + save
  - `SetTextSpeed(int index)` - set + save (clamp 0-2)
  - `SetAnimSpeed(int index)` - set + save (clamp 0-2)

**PlayerPrefs Keys**:
- "Tutorial_Autoplay" (int: 0/1)
- "Tutorial_TextSpeed" (int: 0/1/2)
- "Tutorial_AnimSpeed" (int: 0/1/2)

**Proč**:
- Oddělení tutorial settings od game settings
- Persistence mezi tutoriály
- Self-contained tutorial modul

**Asset**: Vytvořit `.asset` v Unity: `Assets/Systems/Tutorial/Data/Settings/TutorialSettings.asset`

---

#### 1.2 TutorialSpeedConfig (NOVÝ)
**Lokace**: `Assets/Systems/Tutorial/Data/Config/TutorialSpeedConfig.cs`

**Co vytvořit**:
- ScriptableObject class
- Arrays pro multipliers:
  - `float[] textSpeedMultipliers = {1.0f, 3.0f, 5.0f}`
  - `float[] animSpeedMultipliers = {1.0f, 3.0f, 5.0f}`
- Getter methods:
  - `GetTextMultiplier(int index)` - vrací multiplier pro text (s bounds check)
  - `GetAnimMultiplier(int index)` - vrací multiplier pro animace (s bounds check)

**Proč**: Centralizace speed hodnot, konfigurovatelné v Unity Inspector.

**Asset**: Vytvořit `.asset` v Unity: `Assets/Systems/Tutorial/Data/Config/TutorialSpeedConfig.asset`

---

#### 1.3 TutorialSpeedMultiplier (NOVÝ)
**Lokace**: `Assets/Systems/Tutorial/Scripts/TutorialSpeedMultiplier.cs`

**Co vytvořit**:
- Static helper class
- Static reference na TutorialSpeedConfig (serialized, set in Inspector nebo via Resources.Load)
- Methods pro speed aplikaci:
  - `ApplyTextSpeed(float baseDuration, int speedIndex)` → `baseDuration / GetTextMultiplier(speedIndex)`
  - `ApplyAnimSpeed(float baseDuration, int speedIndex)` → `baseDuration / GetAnimMultiplier(speedIndex)`
  - `GetTextMultiplier(int index)` - deleguje na TutorialSpeedConfig
  - `GetAnimMultiplier(int index)` - deleguje na TutorialSpeedConfig

**Proč**:
- Centralizace speed výpočtů
- Vyšší speed = kratší čas (division, ne multiplication)
- Reusable napříč systémem

**Použití**: TutorialActionSequencer, CharVeverka, GridBuilder

---

### STAGE 2: Core Tutorial Logic (Sequencer)

#### 2.1 TutorialActionSequencer Enhancement
**Soubor**: `Assets/Systems/Tutorial/Scripts/TutorialActionSequencer.cs`

**Nové Fields Přidat**:
- `int textSpeedIndex` - cached from TutorialSettings
- `int animSpeedIndex` - cached from TutorialSettings
- `bool isWaitingForNext` - tracking manual mode state
- `bool currentMessageFadedIn` - tracking zda message je fully visible

**Modifikovat Awake()/OnEnable()**:
- Call `TutorialSettings.Instance.LoadFromPlayerPrefs()` - načte settings při startu

**Modifikovat StartSequence()**:
- Load `isAutoplay` z TutorialSettings.Instance.autoplayEnabled (ne z sequenceData)
- Load `textSpeedIndex` z TutorialSettings.Instance.textSpeedIndex
- Load `animSpeedIndex` z TutorialSettings.Instance.animSpeedIndex
- Initialize `isWaitingForNext = false`

**Nová Method: RequestNext()**:
- Bude volána Next tlačítkem v UI
- If message je faded in:
  - Set `isSkipRequested = true` → triggers fade out + advance
- Else (animace/undo running):
  - Set `isSkipRequested = true` → skips wait

**Modifikovat ExecuteMessage()**:
- Aplikovat text speed na fade a display:
  - `fadeInDuration = GetMessageFadeIn() / TutorialSpeedMultiplier.GetTextMultiplier(textSpeedIndex)`
  - `fadeOutDuration = GetMessageFadeOut() / TutorialSpeedMultiplier.GetTextMultiplier(textSpeedIndex)`
  - `displayTime = duration / TutorialSpeedMultiplier.GetTextMultiplier(textSpeedIndex)`
- V manual mode:
  - Po fade in complete: `currentMessageFadedIn = true`
  - Wait for `isSkipRequested` (set by Next button)
  - On skip: fade out → advance
- V auto mode:
  - Současné chování s speed-adjusted timings

**Modifikovat ExecuteAnimation()**:
- Aplikovat anim speed na action delay:
  - `actionDelay = GetActionDelay() / TutorialSpeedMultiplier.GetAnimMultiplier(animSpeedIndex)`
- Samotná animace (movement/rotation) se řídí z CharVeverka (ne zde)

**Modifikovat ExecuteUndo()**:
- Aplikovat anim speed na action delay:
  - `actionDelay = GetActionDelay() / TutorialSpeedMultiplier.GetAnimMultiplier(animSpeedIndex)`

**Nové Public Methods**:
- `ToggleAutoplay()` - už existuje, rozšířit:
  - Toggle `isAutoplay`
  - Call `TutorialSettings.Instance.SetAutoplay(isAutoplay)` - auto-save
- `SetTextSpeed(int index)` - call `TutorialSettings.Instance.SetTextSpeed(index)`, update local `textSpeedIndex`
- `SetAnimSpeed(int index)` - call `TutorialSettings.Instance.SetAnimSpeed(index)`, update local `animSpeedIndex`
- `GetCurrentTextSpeed()` - vrací `textSpeedIndex`
- `GetCurrentAnimSpeed()` - vrací `animSpeedIndex`
- `IsNextButtonActive()` - vrací true if manual mode AND (message faded in OR action complete)

**Proč**:
- Centralizace tutorial playback logiky
- Integrace speed controls do timing calculations
- Manual mode state tracking

**Key Insight**: Speed multiplier DĚLÍ duration (vyšší speed = kratší čas)

---

### STAGE 3: Animation Speed (Character)

#### 3.1 CharVeverka Enhancement
**Soubor**: `Assets/Gameplay/Characters/Scripts/CharVeverka.cs`

**Přístup: Tutorial-Aware Context**

**Co upravit**:
- Detekovat tutorial context (např. flag `isTutorial` set by TutorialActionSequencer)
- If in tutorial:
  - Load `animSpeedIndex` from TutorialSettings.Instance.animSpeedIndex
  - Aplikovat speed na movement/rotation durations:
    - `moveDuration = baseMoveDuration / TutorialSpeedMultiplier.GetAnimMultiplier(animSpeedIndex)`
    - `rotationDuration = baseRotationDuration / TutorialSpeedMultiplier.GetAnimMultiplier(animSpeedIndex)`
- If NOT in tutorial:
  - Use existing gameplay animation speed

**Proč**:
- Tutorial má vlastní speed control nezávislý na gameplay
- Character animace je primární vizuální feedback

**Trade-off**: Potrebuje tutorial context awareness (např. via public field `isTutorial` set by sequencer)

---

### STAGE 4: UI Controls

#### 4.1 TutorialControlPanel (NOVÝ)
**Lokace**: `Assets/Systems/Tutorial/Scripts/TutorialControlPanel.cs`

**Co vytvořit**:
- MonoBehaviour component
- References na:
  - `TutorialActionSequencer tutorialActionSequencer`
  - `Button autoplayToggleButton`
  - `Button nextButton`
  - `Button textSpeedButton`
  - `Button animSpeedButton`
  - `TextMeshProUGUI modeDisplayText`
  - `TextMeshProUGUI textSpeedLabel` (na tlačítku)
  - `TextMeshProUGUI animSpeedLabel` (na tlačítku)

**Methods**:
- `Awake()` - hook up button onClick listeners
- `OnEnable()` - refresh UI z TutorialSettings
- `OnAutoplayToggleClicked()` - call `sequencer.ToggleAutoplay()`, then RefreshUI()
- `OnNextClicked()` - call `sequencer.RequestNext()`
- `OnTextSpeedClicked()` - cycle 0→1→2→0, call `sequencer.SetTextSpeed()`, then RefreshUI()
- `OnAnimSpeedClicked()` - cycle 0→1→2→0, call `sequencer.SetAnimSpeed()`, then RefreshUI()
- `RefreshUI()` - read from TutorialSettings.Instance and update labels:
  - textSpeedLabel.text = "Text: 1x/3x/5x" (based on textSpeedIndex)
  - animSpeedLabel.text = "Anim: 1x/3x/5x" (based on animSpeedIndex)
  - modeDisplayText.text = "Mode: Auto/Manual" (based on autoplayEnabled)
- `Update()` - check `sequencer.IsNextButtonActive()` a enable/disable Next button

**Proč**: Centralizovaná UI control logika, separation of concerns.

---

#### 4.2 TutorialOverlay Prefab Update
**Soubor**: `Assets/Systems/Tutorial/Prefabs/TutorialOverlay.prefab`

**Co přidat v Unity Inspector**:

**Nový GameObject Hierarchy**:
```
TutorialOverlay
├── (existing children)
└── ControlPanel (new GameObject)
    ├── AutoplayToggle (Button + TMP "Auto/Manual")
    ├── NextButton (Button + TMP "Next" - initially disabled)
    ├── TextSpeedButton (Button + TMP "Text: 1x")
    ├── AnimSpeedButton (Button + TMP "Anim: 1x")
    └── ModeDisplay (TextMeshProUGUI "Mode: Auto")
```

**Component na ControlPanel**:
- Attach `TutorialControlPanel` script
- Wire references v Inspector

**Layout**:
- Position buttons v bottom-right nebo top bar
- Ensure visibility during tutorial

**Proč**: Visual interface pro user interaction.

---

### STAGE 5: Restart with Fade

#### 5.1 GridBuilder Fade Out
**Soubor**: `Assets/Gameplay/Grid/Scripts/GridBuilder.cs`

**Nová Method: FadeOutGrid()**:
- Coroutine that:
  1. Get all GameObjectAnimator components v gridRoot
  2. Load `animSpeedIndex` from TutorialSettings.Instance.animSpeedIndex
  3. Calculate fade duration: `baseFadeDuration / TutorialSpeedMultiplier.GetAnimMultiplier(animSpeedIndex)`
  4. For each animator: start `FadeOut(duration, curve)` coroutine
  5. Wait for longest fade to complete
  6. Call `ResetLevel()` to destroy objects

**Použití**: Volá se z `TutorialActionSequencer.Replay()`

**Proč**: Smooth visual transition při restartu, respektuje animation speed.

---

#### 5.2 TutorialActionSequencer Replay
**Soubor**: `Assets/Systems/Tutorial/Scripts/TutorialActionSequencer.cs`

**Modifikovat Replay() Method**:
1. Stop current playback coroutine
2. Hide message (fade out)
3. Call `GridBuilder.FadeOutGrid()` - wait for completion
4. Call `GridBuilder.BuildLevel()` - fade-in handled by GameObjectAnimator automatically
5. Call `StartSequence()` to restart

**Proč**: Seamless restart experience s fade transitions.

---

## Testing & Verification

### 1. Persistence Test
- Set autoplay to Manual → exit tutorial → re-enter → verify Manual mode
- Set text speed to 5x → exit → re-enter → verify 5x
- Set anim speed to 3x → exit → re-enter → verify 3x

### 2. Timing Test
- Measure message display at 1x, 3x, 5x → verify correct ratios
- Measure animation delay at 1x, 3x, 5x → verify correct ratios
- Verify fade durations scale correctly

### 3. Manual Mode Test
- Manual mode + message → Next button activates after fade-in
- Click Next → message fades out, next action starts
- Manual mode + animation → Next button activates when complete
- Click Next → next action starts immediately

### 4. UI State Test
- Toggle autoplay → verify "Mode: Auto/Manual" updates
- Change text speed → verify "Text: Nx" label updates
- Change anim speed → verify "Anim: Nx" label updates
- Next button disabled in auto mode
- Next button disabled during action (except message post-fade)

### 5. Restart Test
- Click restart → grid fades out smoothly
- Grid fades in with new tiles
- Tutorial starts from beginning
- Speed settings maintained

### 6. Edge Cases
- Rapid button clicks → no crashes, proper handling
- Speed change during message → takes effect next action
- Restart during action → clean interruption

---

## Architectural Decisions & Trade-offs

### Speed Control
- **Decision**: Multipliers applied at execution time
- **Rationale**: Dynamic speed changes bez restart
- **Trade-off**: Slightly complex timing calculations

### Manual Mode
- **Decision**: Use existing `isSkipRequested` + new state flags
- **Rationale**: Minimal changes, reuses skip logic
- **Trade-off**: Message handling special case (fade-out before next)

### Animation Speed Scope
- **Decision**: Tutorial-specific (separate from gameplay AnimationSpeed)
- **Rationale**: Tutorial a gameplay mají independent pacing
- **Trade-off**: Dva separate speed settings

### Persistence
- **Decision**: GameSettings + PlayerPrefs
- **Rationale**: Consistent s codebase patterns
- **Trade-off**: Settings shared across všechny tutorials (not per-tutorial)

### UI Design
- **Decision**: Buttons s text labels ("Text: 3x")
- **Rationale**: Clear feedback, minimal screen space
- **Trade-off**: Cycling values (no direct selection)

---

## Key Implementation Notes

### Speed Formula
```
Adjusted Duration = Base Duration / Speed Multiplier
```
Vyšší speed → nižší duration (faster playback)

### Message Special Case (Manual Mode)
1. Message fades in
2. Set `currentMessageFadedIn = true`
3. Next button activates
4. User clicks Next
5. Message fades OUT
6. Next message fades IN

### Tutorial Context Flag
CharVeverka potřebuje vědět, že je v tutorial mode:
- Option 1: Public field `isTutorial` set by TutorialActionSequencer
- Option 2: Check if tutorial scene active
- Option 3: Event-based notification

Doporučení: Option 1 (explicit field)

---

## Files Summary

### Must Modify:
- TutorialActionSequencer.cs - core logic, speed application, manual mode
- CharVeverka.cs - animation speed pro tutorial context
- GridBuilder.cs - fade out pro restart
- TutorialOverlay.prefab - UI controls (Unity Inspector)

### Must Create:
- TutorialSettings.cs + .asset - singleton ScriptableObject pro persistence
- TutorialSpeedConfig.cs + .asset - speed multipliers config
- TutorialSpeedMultiplier.cs - static helper pro calculations
- TutorialControlPanel.cs - MonoBehaviour pro UI logic

**Total**: 4 modifications + 4 new files (8 files celkem)

### Benefits of This Architecture:
- ✅ Tutorial system is self-contained
- ✅ No modifications to GameSettings system
- ✅ Clean separation of concerns
- ✅ Tutorial settings visible only in tutorial overlay
- ✅ Easy to extend with per-tutorial-set settings in future
