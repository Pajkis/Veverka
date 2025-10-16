/// <summary>
/// Error codes following HTTP status code conventions where applicable
/// Custom game-specific codes use 1000+ range
/// </summary>
public enum ErrorCode
{
    None = 0,

    // Client Errors (400-499)
    BadRequest = 400,               // Invalid request/data format
    PermissionDenied = 403,         // Access denied
    FileNotFound = 404,             // File or resource not found
    FileAlreadyExists = 409,        // Conflict - file already exists

    // Server/System Errors (500-599)
    InternalError = 500,            // General system error
    NotImplemented = 501,           // Feature not yet implemented

    // Level System Errors (1000-1099)
    LevelDataCorrupted = 1001,      // Level file corrupted or invalid
    GridSizeMismatch = 1002,        // Grid dimensions don't match expected size
    InvalidLevelFormat = 1003,      // Level CSV format is invalid

    // Grid Validation Errors (1010-1019)
    NoVeverkaFound = 1010,          // No Veverka found in grid
    MultipleVeverkas = 1011,        // Multiple Veverkas found (should be exactly 1)
    NoNutsFound = 1012,             // No nuts found in grid (need at least 1)
    NoGoalsFound = 1013,            // No goals found in grid (need at least 1)
    InvalidTileSymbol = 1014,       // Unknown tile symbol at position
    EmptyTileCell = 1015,           // Empty cell (no symbol) at position
    RowLengthMismatch = 1016,       // Row has different length than expected

    // File System Errors (1100-1199)
    DirectoryCreationFailed = 1100, // Failed to create directory
    FileReadFailed = 1101,          // Failed to read file
    FileWriteFailed = 1102,         // Failed to write file
}
