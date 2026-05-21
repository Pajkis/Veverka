/// <summary>
/// enumeration of sound enumerators
/// </summary>
public enum SoundChannel
{ 
    SoundEffect,
    SoundMusic,
    SoundUI,
    NoSound
}

/// <summary>
/// Sound effects enum
/// </summary>
public enum SfxType   
{
    VeverkaMove,
    VeverkaRotate,
    NutMove,
    GoalReached,
    LevelFinished,
    Undo,
    WaterSplash,
    WaterFill,
    NutInWater,
    NutInHole,
    Build,
    Oops,
}

/// <summary>
/// Sound music enum - background music
/// </summary>
public enum MusicType
{
 Menu,
 Game
}

/// <summary>
/// sound of UI
/// </summary>
public enum UiType
{
  ButtonClick,
  ButtonHover,
  Slider,
  Notification,
  Error
}

/// <summary>
/// Types of audio events handled by <see cref="AudioEvents"/>.
/// </summary>
public enum AudioEventType
{
    Init,
    PlayMusic,
    PlaySfx,
    PlayUi,
    MuteSfx
}

