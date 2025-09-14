/// <summary>
/// Enum defining different types of character events
/// </summary>
public enum CharacterEventType
{
  // Request/ Response events
  DataRequest,
  DataResponse,

  // Movement events    
  MoveStarted,
  MoveCompleted,
  MoveFailed,

  // Rotation events
  RotateStarted,
  RotateCompleted

}