/// <summary>
/// Centralized error message definitions
/// Maps ErrorCode to user-friendly messages
/// </summary>
public static class ErrorMessages
{
    /// <summary>
    /// Get error message for given error code
    /// </summary>
    /// <param name="code">Error code</param>
    /// <param name="context">Optional context to append to message (e.g., file name, level number)</param>
    /// <returns>Formatted error message</returns>
    public static string Get(ErrorCode code, string context = "")
    {
        string message = code switch
        {
            // Client Errors
            ErrorCode.BadRequest => "Invalid request",
            ErrorCode.PermissionDenied => "Access denied",
            ErrorCode.FileNotFound => "File not found",
            ErrorCode.FileAlreadyExists => "File already exists",

            // Server/System Errors
            ErrorCode.InternalError => "An error occurred",
            ErrorCode.NotImplemented => "Feature not yet implemented",

            // Level System Errors
            ErrorCode.LevelDataCorrupted => "Level data is corrupted",
            ErrorCode.GridSizeMismatch => "Grid size doesn't match",
            ErrorCode.InvalidLevelFormat => "Invalid level format",

            // File System Errors
            ErrorCode.DirectoryCreationFailed => "Failed to create directory",
            ErrorCode.FileReadFailed => "Failed to read file",
            ErrorCode.FileWriteFailed => "Failed to write file",

            // Default
            ErrorCode.None => "",
            _ => "Unknown error"
        };

        // Append context if provided
        if (!string.IsNullOrEmpty(context))
        {
            return $"{message}: {context}";
        }

        return message;
    }

    /// <summary>
    /// Get error code as string for notification display
    /// </summary>
    public static string GetCodeString(ErrorCode code)
    {
        return code == ErrorCode.None ? "" : ((int)code).ToString();
    }
}
