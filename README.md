# Veverka

## Overview

Veverka is a 2D Unity project that demonstrates a lightweight menu and level
flow. It includes scenes for bootstrapping the game, navigating menus, loading
levels, and playing through grid-based stages. The project uses Unity's 2D
feature set and TextMeshPro for crisp UI text.

## Unity Version and Packages

- **Editor:** Unity 2022.3.54f1
- **Key packages:**
  - `com.unity.feature.2d`
  - `com.unity.textmeshpro`
  - `com.unity.test-framework` (for PlayMode/EditMode tests)

## Opening the Project

1. Install Unity **2022.3.54f1** or later.
2. In Unity Hub, click **Add project** and select this repository's root folder.
3. Open one of the scenes in `Assets/Scenes`:
   - `00_GameLoad`
   - `10_MainMenu`
   - `11_LevelMenu`
   - `20_LevelLoad`
   - `21_LevelUnload`
   - `30_GamePlay`
4. Essential prefabs and assets live in `Assets/Prefabs`, `Assets/Sprites`, and
   related folders.

## Building

1. In the Unity Editor, open **File → Build Settings**.
2. Ensure the desired scenes are added in the build list (use the order above).
3. Choose your target platform and click **Build**.

## How to run

Windows: 
Download VeverkaGame_WinVx.x.x.zip, unpack open and run Veverka.exe -> play

Android 12+: 
Download VeverkaGame_AndroidVx.x.x.apk, open -> install -> play

## How to play

- movement - cursor buttons or keyboard arrows
- Undo - one of the squirrel buttons or key "B"
- Pause - pause button or "Esc"

## Contribution

Contributions are welcome! Please fork the repository and submit a pull
request. For significant changes, open an issue first to discuss what you would
like to change.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file
for details.
