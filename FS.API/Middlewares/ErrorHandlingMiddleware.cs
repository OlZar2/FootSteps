using System.Net;
using System.Text.Json;
using FluentValidation;
using FS.API.Errors;
using FS.Application.Exceptions;
using FS.Application.Services.AuthLogic.Exceptions;
using FS.Application.Services.ImageLogic.Exceptions;
using FS.Contracts.Error;
using FS.Core.Exceptions;

namespace FS.API.Middlewares;

public class ErrorHandlingMiddleware(RequestDelegate next)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task Invoke(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (ValidationException vex)
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            ctx.Response.ContentType = "application/json";

            var payload = ErrorFactory.Validation(vex.Errors);
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(new { error = payload }, JsonOptions));
        }
        catch (ImageValidationException ive)
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            ctx.Response.ContentType = "application/json";
            
            var payload = new { error = ErrorFactory.Domain(ive) };

            await ctx.Response.WriteAsync(JsonSerializer.Serialize(new { error = payload }, JsonOptions));
        }
        catch (ImageBackendException ibe)
        {
            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
            ctx.Response.ContentType = "application/json";
            
            var payload = new
            {
                error = ErrorFactory.Domain(ibe) with
                {
                    code = "IMAGE_BACKEND_ERROR", message = "Ошибка подсистемы обработки изображений."
                }
            };

            await ctx.Response.WriteAsync(JsonSerializer.Serialize(new { error = payload }, JsonOptions));
        }
        catch (DomainException dex)
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            ctx.Response.ContentType = "application/json";

            var payload = ErrorFactory.Domain(dex);
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(new { error = payload }, JsonOptions));
        }
        catch (NotFoundException nfex)
        {
            ctx.Response.StatusCode = StatusCodes.Status404NotFound;
            ctx.Response.ContentType = "application/json";
            
            var payload = ErrorFactory.NotFound(nfex);

            await ctx.Response.WriteAsync(JsonSerializer.Serialize(new { error = payload }, JsonOptions));
        }
        catch (WrongPasswordException)
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            ctx.Response.ContentType = "application/json";
            
            var payload = ErrorFactory.WrongPassword();
            
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(new { error = payload }, JsonOptions));
        }
        catch (Exception ex)
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            ctx.Response.ContentType = "application/json";
            
            var payload = new { error = new InternalError
            {
                Code = "INTERNAL_ERROR",
                Message = "Произошла внутренняя ошибка."
            }};
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(payload, JsonOptions));
        }
    }
}