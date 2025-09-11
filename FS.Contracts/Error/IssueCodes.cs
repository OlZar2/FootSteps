namespace FS.Contracts.Error;

public static class IssueCodes
{
    public static class File
    {
        public const string Empty = "EMPTY_FILE";
        public const string UnsupportedFormat = "UNSUPPORTED_FORMAT";
        public const string NotImageOrCorrupt = "NOT_IMAGE_OR_CORRUPT";
    }
    
    public const string Required        = "REQUIRED";
    public const string InvalidFormat   = "INVALID_FORMAT";
    public const string TooShort        = "TOO_SHORT";
    public const string TooLong         = "TOO_LONG";
    public const string TooLarge        = "TOO_LARGE";
    public const string TooSmall        = "TOO_SMALL";
    public const string NotUnique       = "NOT_UNIQUE";
    public const string InvalidValue    = "INVALID_VALUE";
    public const string InvalidDate     = "INVALID_DATE";
    public const string PasswordWeak    = "PASSWORD_WEAK";
    public const string AccessDenied    = "ACCESS_DENIED";
    public const string ValidationFailed= "VALIDATION_FAILED";
    public const string InternalError   = "INTERNAL_ERROR";
    public const string Unauthorized    = "UNAUTHORIZED";
    public const string NotFound        = "NOT_FOUND";
    public const string InvalidCredentials    = "INVALID_CREDENTIALS";
}