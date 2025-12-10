using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FS.API.NSwag;

public class IssueCodeDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        // 1) Описываем строковый enum "IssueCode"
        var issueCodeSchema = new OpenApiSchema
        {
            Type = "string",
            Enum = new[]
            {
                "REQUIRED", "INVALID_FORMAT", "TOO_SHORT", "TOO_LONG",
                "TOO_LARGE", "TOO_SMALL", "NOT_UNIQUE", "PASSWORD_WEAK",
                "VALIDATION_FAILED", "INTERNAL_ERROR", "UNAUTHORIZED",
                "ACCESS_DENIED", "NOT_FOUND"
            }.Select(v => new Microsoft.OpenApi.Any.OpenApiString(v)).Cast<Microsoft.OpenApi.Any.IOpenApiAny>().ToList()
        };

        // Добавляем/обновляем схему в components
        swaggerDoc.Components.Schemas["IssueCode"] = issueCodeSchema;

        // 2) Находим схему ErrorDetail и заменяем свойство issue на $ref: IssueCode
        if (swaggerDoc.Components.Schemas.TryGetValue("ErrorDetail", out var errorDetailSchema)
            && errorDetailSchema.Properties.TryGetValue("issue", out var issueProp))
        {
            // Ставим ссылку на нашу схему
            issueProp.Reference = new OpenApiReference
            {
                Type = ReferenceType.Schema,
                Id = "IssueCode"
            };

            // Чистим примитивные атрибуты, чтобы не конфликтовали с $ref
            issueProp.Type = null;
            issueProp.Format = null;
            issueProp.Enum?.Clear();
        }
    }
}