# PopoutImage Prefab Setup Guide

This guide explains how to configure PopoutImage prefabs after the SpriteRenderer refactoring.

## Overview

PopoutImage system now uses **SpriteRenderer** instead of UI Image, allowing for:
- Multiple prefab variants with different sprites
- Random sprite selection from arrays
- Manual sprite assignment in prefabs (not runtime)
- World space positioning (no UI canvas dependency)
- Explicit type identification via **PopoutImageType** enum

## Required Prefab Structure

Each PopoutImage prefab needs:
1. **GameObject** - root object
2. **SpriteRenderer** component - displays the sprite
3. **PopoutImageBox** component - handles animation
4. **RandomSprite** component (optional) - for random sprite selection

## PopoutImageType Enum

The system uses **PopoutImageType** enum to explicitly identify which type of popout image to display. This allows the code to specify exactly which prefab variant should be used.

### Available Types:
- **NoImage** - No popout (default/none value)
- **BasicGoal** - Basic goal popout (for standard nut in goal scenarios)
- **GoldenGoal** - Golden goal popout (for golden nut in golden goal scenarios)
- *(Future extensions: WaterGoal, ObstacleHit, etc.)*

### Usage in Code:

When creating a popout payload, specify the type explicitly:

```csharp
// For golden goal popout
goalPayload.PopoutImage = PopoutImageEventPayload.CreateForSubPayload(
    PopoutImageType.GoldenGoal,
    gameplayConfig
);

// For basic goal popout
goalPayload.PopoutImage = PopoutImageEventPayload.CreateForSubPayload(
    PopoutImageType.BasicGoal,
    gameplayConfig
);

// For no popout
goalPayload.PopoutImage = PopoutImageEventPayload.None;
```

### Prefab Selection Logic:

**PopoutImageManager** selects prefabs based on the **ImageType** specified in the payload:
- `PopoutImageType.GoldenGoal` → `popoutGoldenGoalPrefab`
- `PopoutImageType.BasicGoal` → `popoutGoalPrefab`
- `PopoutImageType.NoImage` → No popout created

This approach **decouples** popout type from goal type, allowing more flexibility (e.g., you could show a GoldenGoal popout for obstacle events if desired).

## Setup Steps

### Step 1: Edit Existing Prefabs or Create New Ones

You'll need at least two prefab variants:
- `Popout_Goal` - for basic goal popouts
- `Popout_GoldenGoal` - for golden goal popouts

#### Option A: Convert Existing Prefab

1. Open prefab in Unity (e.g., `Assets/Gameplay/Tiles/Prefabs/Roads/Popout_Goal.prefab`)
2. **Remove** any existing Canvas structure (if present)
3. Add **SpriteRenderer** component to root GameObject
4. Ensure **PopoutImageBox** component is present
5. Save prefab

#### Option B: Create New Prefab

1. Create new GameObject in scene
2. Add **SpriteRenderer** component
3. Add **PopoutImageBox** component
4. Drag to Project window to create prefab
5. Delete from scene

### Step 2: Configure SpriteRenderer

#### For Single Sprite Setup:

1. Select the prefab
2. In SpriteRenderer component:
   - **Sprite**: Assign your sprite (e.g., `Tile_GoldenNut.png`)
   - **Sorting Layer**: Set appropriate layer (e.g., "Default" or "UI")
   - **Order in Layer**: Set to appear above game tiles (e.g., 10)
   - **Color**: White (will be tinted by payload.TintColor at runtime)

#### For Random Sprite Setup:

1. Add **RandomSprite** component to the prefab
2. Leave **Sprite** field in SpriteRenderer empty (or set default fallback)
3. In RandomSprite component:
   - **Sprites Array**: Add multiple sprites to choose from
   - **Random Seed**: Leave at 0 (use system time) for true randomness
4. Save prefab

### Step 3: Assign Prefabs to PopoutImageManager

1. Find **PopoutImageManager** in your scene (usually attached to GameManager or similar)
2. In Inspector:
   - **Popout Goal Prefab**: Assign `Popout_Goal` prefab
   - **Popout Golden Goal Prefab**: Assign `Popout_GoldenGoal` prefab
3. Save scene

## Example Configurations

### Example 1: Basic Goal Popout (Single Sprite)

```
Popout_Goal (GameObject)
├── SpriteRenderer
│   ├── Sprite: Tile_Goal.png
│   ├── Sorting Layer: Default
│   ├── Order in Layer: 10
│   └── Color: White
└── PopoutImageBox (Script)
```

### Example 2: Golden Goal Popout (Random Selection)

```
Popout_GoldenGoal (GameObject)
├── SpriteRenderer
│   ├── Sprite: (empty or fallback)
│   ├── Sorting Layer: Default
│   ├── Order in Layer: 10
│   └── Color: White
├── PopoutImageBox (Script)
└── RandomSprite (Script)
    └── Sprites Array:
        ├── Element 0: Tile_GoalGolden_01.png
        ├── Element 1: Tile_GoalGolden_02.png
        └── Element 2: Tile_GoalGolden_03.png
```

## Testing

### Test Checklist:

1. **Compile Test** - Project compiles without errors
2. **Prefab Validation** - All prefabs have SpriteRenderer + PopoutImageBox
3. **Manager Assignment** - PopoutImageManager has both prefabs assigned
4. **Golden Goal Test** - Place golden nut in golden goal, verify popout appears
5. **Basic Goal Test** - Place nut in basic goal, verify popout appears (if configured)
6. **Random Sprites Test** - Multiple activations show different sprites (if using RandomSprite)
7. **Animation Test** - Popout moves and fades correctly
8. **Position Test** - Popout appears at correct grid position
9. **Scale Test** - Size matches tile size with payload.Scale applied
10. **Tint Test** - TintColor from GoldenGoalTile inspector works

## Troubleshooting

### Problem: Popout doesn't appear

**Possible causes:**
- PopoutImageManager prefabs not assigned
- GoldenGoalTile not creating PopoutImage payload
- PopoutImageBox script missing from prefab
- GridRoot not found in scene

**Solution:**
1. Check PopoutImageManager inspector - verify prefabs assigned
2. Check console for warnings/errors
3. Verify prefab has both SpriteRenderer and PopoutImageBox components

### Problem: Popout appears but no sprite visible

**Possible causes:**
- SpriteRenderer.sprite is null
- RandomSprite.sprites array is empty
- Sorting layer incorrect (behind other objects)

**Solution:**
1. Check SpriteRenderer has sprite assigned (or RandomSprite has array filled)
2. Verify Sorting Layer and Order in Layer are correct
3. Check sprite alpha is not 0

### Problem: All popouts show same sprite (when using RandomSprite)

**Possible causes:**
- RandomSprite component not on prefab
- sprites array has only one element
- Same sprite assigned multiple times in array

**Solution:**
1. Ensure RandomSprite component is attached
2. Fill sprites array with multiple DIFFERENT sprites
3. Check Debug.Log output to verify random selection is working

### Problem: Popout appears at wrong position

**Possible causes:**
- GridUtils.GridToWorld() calculation incorrect
- PositionOffset not set correctly
- Parent transform (GridRoot) has offset

**Solution:**
1. Check GridRoot position is (0, 0, 0)
2. Verify DisplaySettings.tileSize is correct
3. Check GoldenGoalTile.popoutPositionOffset in inspector

## Advanced Configuration

### Per-Tile Animation Overrides

GoldenGoalTile supports per-tile animation customization via inspector:
- **Popout Position Offset**: Offset from grid position (default: 0,0)
- **Popout Start Delay**: Delay before animation (default: 0s)
- **Popout Scale**: Scale multiplier (default: 1.0)
- **Popout Tint Color**: Color tint (default: White)

These settings override the defaults from GameplayConfig.

### Adding More Prefab Variants

To add new prefab variants (e.g., for different obstacle types):

1. Create new prefab following setup steps above
2. Add new field to PopoutImageManager:
   ```csharp
   [SerializeField] private GameObject popoutWaterGoalPrefab;
   ```
3. Update selection logic in OnGoalEvent() or OnObstacleEvent()
4. Assign prefab in inspector

## Migration Notes

### Changes from Previous System:

- ❌ **Removed**: `Sprite Image` field from PopoutImageEventPayload
- ❌ **Removed**: `goldenNutPopoutSprite` field from GoldenGoalTile
- ❌ **Removed**: Runtime sprite assignment in PopoutImageBox.Init()
- ✅ **Added**: Multiple prefab variants support
- ✅ **Added**: RandomSprite component for sprite variation (existing component reused)
- ✅ **Added**: **PopoutImageType** enum for explicit type identification
- ✅ **Added**: `ShouldShow` property - simplified to check only ImageType (no redundant flag)
- ✅ **Changed**: PopoutImageManager now selects prefab based on **ImageType** (not GoalType)

### Code Updates Required:

All factory method calls updated from:
```csharp
// OLD (before SpriteRenderer refactor):
PopoutImageEventPayload.CreateForSubPayload(goldenNutPopoutSprite, gameplayConfig)

// INTERMEDIATE (after SpriteRenderer, before PopoutImageType):
PopoutImageEventPayload.CreateForSubPayload(gameplayConfig)

// NEW (current with PopoutImageType):
PopoutImageEventPayload.CreateForSubPayload(PopoutImageType.GoldenGoal, gameplayConfig)
```

### Payload Validation:

- `IsValid()` no longer checks for null sprite (sprite is in prefab)
- **`ShouldShow` property** - simplified to `ImageType != PopoutImageType.NoImage` (no redundant flag)
- `None` property returns instance with `ImageType = NoImage`

### Design Philosophy:

Both **BubbleMessageEventPayload** and **PopoutImageEventPayload** now use **enum-only validation**:
- `BubbleMessage.ShouldShow` → checks `MessageType != NoMessage`
- `PopoutImage.ShouldShow` → checks `ImageType != NoImage`

This eliminates redundant control flags (`ShowMessage`, `ShowPopout`) - the enum value itself determines visibility.

---

**For questions or issues, check the main refactoring plan document or Unity console logs.**
