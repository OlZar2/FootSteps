using FS.Application.Exceptions;
using FS.Contracts.Error;
using FS.Core.Exceptions;

namespace FS.API.Errors;

public static class ErrorFactory
{
    public static ErrorEnvelope Validation(IEnumerable<FluentValidation.Results.ValidationFailure> failures) =>
        new(
            code: "VALIDATION_FAILED",
            message: "Данные не проходят валидацию.",
            details: failures.Select(f =>
                new ErrorDetail(
                    field: f.PropertyName,
                    issue: f.ErrorCode ?? "INVALID",
                    message: f.ErrorMessage
                )).ToList()
        );
    
    public static ErrorEnvelope Domain(DomainException de) =>
        new(
            code: "DOMAIN_RULE_VIOLATION",
            message: "Нарушено доменное правило.",
            details:
            [
                new ErrorDetail(field: de.Field ?? "", issue: de.Issue, message: de.Message)
            ]);
    
    public static ErrorEnvelope NotFound(NotFoundException nfex) =>
        new(
            code: "NOT_FOUND",
            message: "Не найдено.",
            details:
            [
                new ErrorDetail(field: "", issue: IssueCodes.NotFound, message: nfex.Message)
            ]);
    
    public static ErrorEnvelope WrongPassword() =>
        new(
            code: "UNAUTHORIZED",
            message: "Не найдено.",
            details:
            [
                new ErrorDetail(field: "", issue: IssueCodes.InvalidCredentials, "Неверный логин или пароль")
            ]);
}