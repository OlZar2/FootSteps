using FS.Core.Exceptions;

namespace FS.Application.Services.ImageLogic.Exceptions;

public class ImageValidationException(string issue, string message)
    : DomainException(issue, message);