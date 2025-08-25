# Veverka

A complete 2D Unity puzzle game featuring Sokoban-style gameplay mechanics. The squirrel (Veverka) must push nuts to their designated goals across increasingly challenging levels.

**Key Features:**

-   Grid-based movement and puzzle mechanics
-   10 challenging levels with progressive difficulty
-   Undo system for strategic gameplay
-   Complete audio system with music and sound effects
-   Multi-platform support (Windows & Android 12+)
-   Professional UI/UX with smooth transitions
-   Configurable game settings
-   Lightweight menu and level flow system
-   TextMeshPro integration for crisp UI text

**Technical Highlights:**

-   Unity 2022.3.54f1 with 2D feature set
-   Key packages: `com.unity.feature.2d`, `com.unity.textmeshpro`, `com.unity.test-framework`
-   ScriptableObject-based configuration system
-   Event-driven architecture
-   Clean scene flow management with 6 dedicated scenes
-   Addressable asset system integration
-   Comprehensive testing framework support

**Game Scenes:**

-   `00_GameLoad` - Game bootstrap and initialization
-   `10_MainMenu` - Main menu navigation
-   `11_LevelMenu` - Level selection interface
-   `20_LevelLoad` - Level loading sequence
-   `21_LevelUnload` - Level cleanup
-   `30_GamePlay` - Core gameplay experience

**How to Play:**

-   **Movement:** Arrow keys, cursor buttons, or on-screen controls
-   **Undo:** Press 'B' key or squirrel button to reverse moves
-   **Pause:** Press 'Esc' key or pause button to access menu
-   **Goal:** Push all nuts onto their designated goal positions

**Installation & Running:**

-   **Windows:** Download `VeverkaGame_WinVx.x.x.zip`, extract and run `Veverka.exe`
-   **Android 12+:** Download `VeverkaGame_AndroidVx.x.x.apk` and install

**Asset Structure:**

-   Essential prefabs located in `Assets/Prefabs`
-   Sprites and visual assets in `Assets/Sprites`
-   Audio files organized by type (music, effects, UI sounds)

## Getting Started

### Development Setup

1. Install Unity **2022.3.54f1** or later
2. In Unity Hub, click **Add project** and select the `Veverka/` folder
3. Open scene `00_GameLoad` to start the game flow
4. For development, explore scenes in `Assets/Scenes/` directory
5. Build via **File → Build Settings** (ensure all 6 scenes are added in correct order)

### Building for Distribution

1. Open **File → Build Settings** in Unity Editor
2. Add scenes in this order: `00_GameLoad`, `10_MainMenu`, `11_LevelMenu`, `20_LevelLoad`, `21_LevelUnload`, `30_GamePlay`
3. Select target platform (Windows/Android)
4. Configure platform-specific settings
5. Click **Build** to generate executable

### Development Requirements

-   Unity 2022.3.54f1+
-   Git
-   Platform-specific SDKs (for mobile deployment)

## Project Structure

```
Veverka/
├── Assets/           # Game assets and scripts
│   ├── Prefabs/      # Essential game prefabs
│   ├── Sprites/      # Visual assets
│   ├── Audio/        # Music, effects, UI sounds
│   └── Scenes/       # Game scenes (6 total)
├── ProjectSettings/  # Unity project configuration
├── README.md         # Detailed game documentation
└── CHANGELOG.md      # Version history
```

## Contributing

Contributions are welcome! Please fork the repository and submit pull requests. For significant changes, open an issue first to discuss proposed modifications.

## License

This project is licensed under the MIT License.

## Version

Current version: v0.10.0 (Initial Release)  
Release date: 2025-08-20
