# Veverka

A complete 2D Unity puzzle game featuring Sokoban-style gameplay mechanics. The squirrel (Veverka) must push nuts to their designated goals across increasingly challenging levels.

**Key Features:**

- Grid-based movement and puzzle mechanics
- 30 challenging levels with progressive difficulty
- User-created levels with import/export functionality
- Undo system for strategic gameplay
- Complete audio system with music and sound effects
- Multi-platform support (Windows & Android 13+)
- Professional UI/UX with smooth transitions
- Configurable game settings
- Lightweight menu and level flow system
- TextMeshPro integration for crisp UI text

**Technical Highlights:**

- Unity 2022.3.54f1 with 2D feature set
- Key packages: `com.unity.feature.2d`, `com.unity.textmeshpro`, `com.unity.test-framework`
- ScriptableObject-based configuration system
- Event-driven architecture
- Clean scene flow management with 5 dedicated scenes
- Addressable asset system integration
- Comprehensive testing framework support

**Game Scenes:**

- `Bootstrap` - Game bootstrap and initialization
- `MainMenu` - Main menu navigation
- `LevelSelect` - Level selection interface
- `LevelTransition` - Level loading and transition sequence
- `GamePlay` - Core gameplay experience

**How to Play:**

- **Movement:** Arrow keys, cursor buttons, or on-screen controls
- **Undo:** Press 'B' key or squirrel button to reverse moves
- **Pause:** Press 'Esc' key or pause button to access menu
- **Goal:** Push all nuts onto their designated goal positions

**Installation & Running:**

- **Windows:** Download `VeverkaGame_WinVx.x.x.zip`, extract and run `Veverka.exe`
- **Android 13+:** Download `VeverkaGame_AndroidVx.x.x.apk` and install

**Asset Structure:**

- `AddressableAssetsData` - Addressable asset configurations
- `GamePlay` - Characters, grid, and gameplay logic
- `Scenes` - Unity scene files
- `Shared` - Shared utilities and UI components
- `Systems` - Core systems such as Audio, Input, Menu, Scene, Settings

## Getting Started

### Development Setup

1. Install Unity **2022.3.54f1** or later
2. In Unity Hub, click **Add project** and select the `Veverka/` folder
3. Open scene `Bootstrap` to start the game flow
4. For development, explore scenes in `Assets/Scenes/` directory
5. Build via **File → Build Settings** (ensure all 5 scenes are added in correct order)

### Building for Distribution

1. Open **File → Build Settings** in Unity Editor
2. Add scenes in this order: `Bootstrap`, `MainMenu`, `LevelSelect`, `LevelTransition`, `GamePlay`
3. Select target platform (Windows/Android)
4. Configure platform-specific settings
5. Click **Build** to generate executable

### Development Requirements

- Unity 2022.3.54f1+
- Git
- Platform-specific SDKs (for mobile deployment)

## Project Structure

```
Veverka/
├── Assets/             # Game assets and scripts
│   ├── AddressableAssetsData/  # Addressable asset configuration
│   ├── GamePlay/               # Gameplay scripts and assets
│   ├── Scenes/                 # Unity scenes
│   ├── Shared/                 # Shared utilities and UI
│   └── Systems/                # Core systems (Audio, Input, etc.)
├── Packages/          # Package manifest and dependencies
├── ProjectSettings/    # Unity project configuration
├── CHANGELOG.md        # Version history
├── LICENCE.md          # License information
├── README.md           # Documentation
└── ignore.conf         # Git ignore rules
```

## Contributing

Contributions are welcome! Please fork the repository and submit pull requests. For significant changes, open an issue first to discuss proposed modifications.

## License

This project is licensed under the MIT License.

## Version

Current version: v0.14.0
Release date: 2025-10-19
