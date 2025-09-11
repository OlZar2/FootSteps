namespace FS.API.Errors;

public static class IssueToStatusMapper
{
    public static int MapForImageValidation(string issue) => issue switch
    {
        "EMPTY_FILE"           => StatusCodes.Status400BadRequest,
        "NOT_IMAGE_OR_CORRUPT" => StatusCodes.Status422UnprocessableEntity,
        "UNSUPPORTED_FORMAT"   => StatusCodes.Status415UnsupportedMediaType,
        "TOO_LARGE"            => StatusCodes.Status413PayloadTooLarge,
        "DETECTION_FAILED"     => StatusCodes.Status400BadRequest,
        _                      => StatusCodes.Status400BadRequest
    };
}