# Interactive Tutorial Overlay System - Implementation Plan

## Co Chceme Dosáhnout

**Interaktivní tutorial overlay** s real gameplay animacemi:
- Mini-grid (5×7 tiles) s plnohodnotnou gameplay grafikou
- Real animace (character moves, nut pushes, effects)
- Izolovaná logika (žádné turn counting, goal tracking)
- Scripted playback (ne player input)
- Replay + Close buttons
- Queue systém pro multiple tutorials
- Auto-show při první nové mechanice
- Settings toggle pro vypnutí

---

## Co Už Máš Hotové ✅

### Event Channels (Assets/Systems/Tutorial/Events/):
- TutorialDirectionEvent.asset
- TutorialCharacterEvent.asset
- TutorialNutEvent.asset
- TutorialGridEvent.asset
- TutorialGoalEvent.asset
- TutorialRoadEvent.asset

**Význam:** Separátní eventy = automatická izolace od main game! Tutorial eventy neovlivní TurnControl, GoalManager ani LevelCompletion.

### Tutorial Sprites (Assets/Systems/Tutorial/Sprites/):
- Tutorial_Veverka.png (character)
- Nuts: BasicNut, StoneNut, WaterNut, GoldenNut
- Goals: Goal, GoalGolden
- Obstacles: Hole, WaterHole, StoneWall, TreeWall, GoldenStatue
- Roads: StoneRoad, HoleStone
- Frame_tutorial.png (pozadí overlay)

### TutorialOverlay.prefab (Assets/Systems/Tutorial/Prefabs/):
- Základní UI struktura
- Grid velikost: 5×7 tiles (height × width)
- Layout: Label_Tutorial (header) + TutorialGrid (zelený panel) + Label_Description (footer)

---

## Implementační Postup

### Phase 1: Datové Struktury (1-2 hodiny)

**Co vytvořit:**

1. **TutorialScenarioData.cs** (ScriptableObject)
   - Cesta: `Assets/Systems/Tutorial/Data/TutorialScenarioData.cs`
   - Obsahuje:
     - **levelSet** (LevelSetType enum) - Basic, Stone, Water, Golden
     - **tutorialIndex** (int) - index v rámci setu (0, 1, 2...)
     - **csvData** (TextAsset) - CSV level data (manuální assign)
     - **actions** (List<TutorialAction>) - scripted sequence
     - **description** (string) - popis v footer (TextArea)
     - **animationSpeed** (float) - rychlost animací (default 0.5f)
     - **Title** (property) - auto-generovaný z levelSet + tutorialIndex

   **Poznámky k designu:**
   - ❌ **NE string ID** - použij enum pro type-safety
   - ✅ **Používá existující LevelSetType** (Basic=0, Stone=1, Water=2, Fire=3)
   - ✅ **Title je auto-generated** - `"{tutorialIndex + 1}. {levelSet.ToString().ToLower()} tutorial"`
   - ✅ **CSV workflow** - stejný formát jako levely, reuse TileParser + GridBuilder
   - ✅ **Animation speed** - pomalejší než gameplay (0.5f), configurable per-tutorial
   - ✅ **Grid offset** - v TutorialOverlay.prefab Transform, NE v ScenarioData

2. **TutorialAction.cs** (struct)
   - Cesta: `Assets/Systems/Tutorial/Data/TutorialAction.cs`
   - Obsahuje:
     - type (enum: MoveCharacter, Wait, HighlightTile)
     - direction (enum: Up, Down, Left, Right)
     - delay (float)

**Testování:**
- Vytvoř test ScriptableObject v Unity
- Right-click → Create → Tutorial/Scenario
- Fill in fields, ověř že se ukládá

---

### Phase 2: Tutorial Director (2-3 hodiny)

**Co vytvořit:**

3. **TutorialDirector.cs**
   - Cesta: `Assets/Systems/Tutorial/Scripts/TutorialDirector.cs`
   - Zodpovědnost:
     - Orchestrates scripted actions
     - Builds mini-grid from CSV
     - Executes action sequence
     - Handles Replay

**Funkce:**
- `PlayTutorial()` - spustí tutorial sequence
- `ExecuteActions()` - coroutine pro scripted actions
- `Replay()` - clear grid + restart
- `BuildTutorialGrid(TextAsset csv)` - použije GridBuilder
- `FindTutorialCharacter()` - najde CharVeverka v gridRoot

**Klíčové závislosti:**
- TutorialScenarioData (scenario data)
- Transform gridRoot (parent pro tiles)
- DirectionEvent tutorialDirectionEvent (pro character movement)

**Testování:**
- Připoj na TutorialOverlay prefab
- Vytvoř jednoduchý CSV (3×3 grid, V + N + G)
- Test PlayTutorial() v Play Mode

---

### Phase 3: Overlay Controller (2-3 hodiny)

**Co vytvořit:**

4. **TutorialOverlayController.cs**
   - Cesta: `Assets/Systems/Tutorial/Scripts/TutorialOverlayController.cs`
   - Zodpovědnost:
     - Receives TutorialScenarioData on spawn
     - Initializes TutorialDirector
     - Updates UI (title, description)
     - Handles button clicks (Replay, Close)

**Funkce:**
- `Initialize(TutorialScenarioData scenario)` - entry point
- `HandleReplayButton()` - calls director.Replay()
- `HandleCloseButton()` - closes overlay via SceneNavigationEvents

**Co upravit v TutorialOverlay.prefab:**
- Přidat TutorialOverlayController komponentu
- Wire Replay button → HandleReplayButton()
- Wire Close button → HandleCloseButton()
- Wire UI references (titleText, descriptionText)

**Testování:**
- Manual spawn overlay
- Test Initialize() s dummy scenario
- Test Replay button
- Test Close button

---

### Phase 4: System Integration (2-3 hodiny)

**Co modifikovat:**

5. **OverlayType.cs**
   - Cesta: `Assets/Systems/Scene/Enums/OverlayType.cs`
   - Změna: Přidat `Tutorial` enum value

6. **OverlayPrefabProvider.cs**
   - Cesta: `Assets/Systems/Scene/Scripts/OverlayPrefabProvider.cs`
   - Změna:
     - Přidat `[SerializeField] GameObject tutorialPrefab`
     - Přidat case `OverlayType.Tutorial => tutorialPrefab`
   - Wire: Assign TutorialOverlay.prefab v Inspectoru

**Testování:**
- Test spawning overlay via SceneNavigationEvents
- Raise event with OverlayType.Tutorial + TutorialScenarioData
- Ověř že se overlay správně otevře

---

### Phase 5: Queue System (2-3 hodiny)

**Co vytvořit:**

7. **TutorialQueueManager.cs** (Singleton)
   - Cesta: `Assets/Systems/Tutorial/Scripts/TutorialQueueManager.cs`
   - Zodpovědnost:
     - Manages queue of TutorialScenarioData
     - Shows tutorials sequentially
     - Tracks active state

**Funkce:**
- `QueueTutorial(TutorialScenarioData)` - přidá do fronty
- `OnTutorialClosed()` - volá se z overlay, shows next
- `ShowNextTutorial()` - internal, raises SceneNavigationEvent

8. **LevelTutorialTrigger.cs**
   - Cesta: `Assets/Systems/Tutorial/Scripts/LevelTutorialTrigger.cs`
   - Zodpovědnost:
     - Attached to Level GameObject
     - Checks PlayerPrefs (které tutorialy už byly viděny)
     - Queues unseen tutorials na Start()

**Funkce:**
- `Start()` - checks tutorials, queues if needed
- `ShouldShowTutorials()` - checks PlayerPrefs "ShowTutorials" setting

**Testování:**
- Create 2 tutorial scenarios
- Add LevelTutorialTrigger to test level
- Play level, ověř sequential display
- Check PlayerPrefs persistence

---

### Phase 6: Settings Integration (1 hodina)

**Co modifikovat:**

9. **SettingsMenu**
   - Přidat toggle "Show Tutorials"
   - Default: ON (PlayerPrefs "ShowTutorials" = 1)
   - Optional: "Reset Tutorials" button

**Testování:**
- Toggle OFF → tutorials se neukazují
- Toggle ON → tutorials se ukazují
- Reset button → smaže PlayerPrefs, tutorials se znovu objeví

---

### Phase 7: Content Creation (ongoing)

**Vytvoř 8 MVP tutorialů:**

1. **BasicNut + BasicGoal**
   - CSV: 5×7, V na levé, N+G na pravé
   - Actions: MoveCharacter Right (×2)

2. **BasicNut + Hole** (warning)
   - CSV: V, N, Hole
   - Actions: Move Right

3. **StoneNut + Hole** (fills)
   - CSV: V, StoneNut, Hole
   - Actions: Move Right

4. **StoneNut + BasicGoal** (creates wall)
   - CSV: V, StoneNut, BasicGoal
   - Actions: Move Right

5. **WaterNut + BasicGoal** (splash)
   - CSV: V, WaterNut, BasicGoal, neighbor nut
   - Actions: Move Right

6. **WaterNut + Hole** (fills with water)
   - CSV: V, WaterNut, Hole
   - Actions: Move Right

7. **GoldenNut + GoldenGoal** (correct)
   - CSV: V, GoldenNut, GoldenGoal
   - Actions: Move Right

8. **GoldenNut + BasicGoal** (wrong - statue)
   - CSV: V, GoldenNut, BasicGoal
   - Actions: Move Right

**Workflow pro každý tutorial:**
- Create CSV file (`Assets/Systems/Tutorial/Levels/Tutorial_X.csv`)
- Create ScriptableObject (Right-click → Create → Tutorial/Scenario)
- Assign CSV, title, instructions
- Define action sequence
- Test v Play Mode

---

## GridBuilder Integration

**Problém:** Jak použít existující GridBuilder pro tutorial grid?

**Řešení 1: Wrapper (DOPORUČENO)**
- Vytvoř `TutorialGridBuilder.cs`
- Wrapper kolem existujícího GridBuilder
- No changes to main GridBuilder

**Řešení 2: Modify GridBuilder**
- Add optional parameter `bool isTutorial = false`
- GridBuilder checks flag, spawns on tutorial parent
- Requires changes in production code

**Doporučuji Řešení 1** - žádné breaking changes

---

## Časový Odhad

- Phase 1 (Data Structures): 1-2 hodiny
- Phase 2 (Tutorial Director): 2-3 hodiny
- Phase 3 (Overlay Controller): 2-3 hodiny
- Phase 4 (System Integration): 2-3 hodiny
- Phase 5 (Queue System): 2-3 hodiny
- Phase 6 (Settings): 1 hodina
- Phase 7 (Content Creation): ongoing (1 hod/tutorial)

**Celkem: 10-14 hodin** (bez content creation)

---

## Kritické Soubory

### K Vytvoření:
1. `Assets/Systems/Tutorial/Data/TutorialScenarioData.cs`
2. `Assets/Systems/Tutorial/Data/TutorialAction.cs`
3. `Assets/Systems/Tutorial/Scripts/TutorialDirector.cs`
4. `Assets/Systems/Tutorial/Scripts/TutorialOverlayController.cs`
5. `Assets/Systems/Tutorial/Scripts/TutorialQueueManager.cs`
6. `Assets/Systems/Tutorial/Scripts/LevelTutorialTrigger.cs`
7. `Assets/Systems/Tutorial/Scripts/TutorialGridBuilder.cs` (wrapper)

### K Modifikaci:
8. `Assets/Systems/Scene/Enums/OverlayType.cs`
9. `Assets/Systems/Scene/Scripts/OverlayPrefabProvider.cs`
10. `Assets/Systems/Tutorial/Prefabs/TutorialOverlay.prefab`

### Reference (read-only):
11. `Assets/Gameplay/Grid/Scripts/GridBuilder.cs`
12. `Assets/Gameplay/Characters/Scripts/CharVeverka.cs`
13. `Assets/Systems/Scene/Scripts/SceneFlowManager.cs`

---

## Otevřené Otázky

### 1. Grid Container Size
- **Otázka:** Plná velikost (717×465px) nebo kompaktní (380×260px)?
- **Poznámka:** Uživatel testuje později

### 2. GridBuilder Integration
- **Doporučení:** Wrapper approach (TutorialGridBuilder.cs)
- **Důvod:** No breaking changes to production code

---

## Výhody

1. Žádná duplikace kódu (reuse GridBuilder, CharVeverka, tiles)
2. Separátní event channels = automatic isolation (JIŽ HOTOVO!)
3. CSV formát (stejný jako levely)
4. Real animations (identické s hrou)
5. Queue systém (multiple tutorials za sebou)
6. Settings toggle (uživatel má kontrolu)
7. Replay funkce
8. Data-driven (ScriptableObjects)
