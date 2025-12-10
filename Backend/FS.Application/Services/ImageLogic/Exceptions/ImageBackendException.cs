using FS.Core.Exceptions;

namespace FS.Application.Services.ImageLogic.Exceptions;

public class ImageBackendException(string issue, string message, string? field)
    : DomainException(issue, message, string.IsNullOrEmpty(field) ? "Image" : field);