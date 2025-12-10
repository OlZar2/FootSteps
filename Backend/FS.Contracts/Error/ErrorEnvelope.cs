namespace FS.Contracts.Error;

public record ErrorEnvelope(string code, string message, IReadOnlyList<ErrorDetail> details);