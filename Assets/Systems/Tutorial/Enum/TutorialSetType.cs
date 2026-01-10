
/// <summary>
/// Specifies the available types of tutorial sets.
/// </summary>
/// <remarks>Use this enumeration to indicate the category or theme of a tutorial set. The values represent
/// distinct tutorial set types that may affect content or behavior in the application.</remarks>
public enum TutorialSetType
{
    Controls = 0,
    Basic = 1,
    Goldem = 2,
    Stone = 3,
    Water = 4,
}

/// <summary>
/// Specifies the types of controls available in a tutorial sequence.
/// </summary>
/// <remarks>Use this enumeration to identify or configure the control actions presented to the user during a
/// tutorial. Each value represents a distinct control type that may be required for user interaction.</remarks>
public enum TutorialControlType
{
    MoveCharacter = 0,
    MoveToObstacle = 1, // Move to wall, hole and water hole
    PushNut = 2, // Push nut, Push nut to wall
}

/// <summary>
/// Specifies the basic tutorial types available for gameplay scenarios.
/// </summary>
/// <remarks>Use this enumeration to select the type of tutorial sequence or goal configuration in the tutorial
/// system. Each value represents a distinct objective or path for the tutorial.</remarks>
public enum TutorialBasicType
{
    BasicNutToBasicGoal = 0,
    BasicNutToGoldenGoal = 1,
    BasicNutToHole = 2,
    BasicNutToWaterHole = 3,
}

/// <summary>
/// Specifies the available types of golden nut tutorial goals.
/// </summary>
/// <remarks>This enumeration is used to indicate the target goal type for a golden nut in tutorial scenarios. The
/// values represent different possible destinations or objectives for the golden nut within the tutorial
/// context.</remarks>
public enum TutorialGoldemType
{
    GoldenNutToGoldenGoal = 0,
    GoldenNutToBasicGoal = 1,
    GoldenNutToHole = 2,    
    GoldenNutToWaterHole = 3,
}

/// <summary>
/// Specifies the type of tutorial stone used to indicate different goal or target scenarios in the tutorial.
/// </summary>
/// <remarks>Use this enumeration to distinguish between various tutorial objectives, such as reaching a basic
/// goal, a golden goal, a hole, or a water hole. The values can be used to control tutorial flow or display
/// context-specific instructions.</remarks>
public enum TutorialStoneType
{
    StoneNutToBasicGoal = 0,
    StoneNutToGoldenGoal = 1,
    StoneNutToHole = 2,
    StoneNutToWaterHole = 3,
}

/// <summary>
/// Specifies the type of water-related tutorial objective.
/// </summary>
/// <remarks>Use this enumeration to indicate the specific water-based goal or milestone within a tutorial
/// sequence.</remarks>
public enum TutorialWaterType
{
    WaterNutToBasicGoal = 0,
    WaterNutToGoldenGoal = 1,
    WaterNutToHole = 2,
    WaterNutToWaterHoleSplash = 3, // water, basic, golden goal splash effect
    WaterNutToWaterHoleSplashNoEffect = 4, // water - splash effect, stone nut - no splash effect
    WaterNutToWaterHoleChained = 5, // water - chained effect, 
}