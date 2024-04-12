using Ganss.Xss;
using System.Text;

namespace Api.Middleware;

public class AntiXssMiddleware : IMiddleware
{
    private IHtmlSanitizer HtmlSanitizer { get; }

    public AntiXssMiddleware()
    {
        this.HtmlSanitizer = new HtmlSanitizer();
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Request.EnableBuffering();

        if (!this.ValidateXssString(context.Request.Path.Value))
        {
            await this.CreateAntiXssErrorResponse(context, HttpContextError.Path);
            return;
        }

        if (!this.ValidateXssString(context.Request.QueryString.Value))
        {
            await this.CreateAntiXssErrorResponse(context, HttpContextError.QueryString);
            return;
        }

        using var reader = new StreamReader(
            context.Request.Body,
            encoding: Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            bufferSize: 1024,
            leaveOpen: true);

        var requestedbody = (await reader.ReadToEndAsync()).Replace("\r", "");

        var body = requestedbody.Replace("&", "");

        if (!this.ValidateXssString(body))
        {
            await this.CreateAntiXssErrorResponse(context, HttpContextError.Body, this.HtmlSanitizer.Sanitize(requestedbody));
            return;
        }

        context.Request.Body.Position = 0;

        await next(context);
    }

    private bool ValidateXssString(string? input) => string.IsNullOrWhiteSpace(input) || input == this.HtmlSanitizer.Sanitize(input);

    private async Task CreateAntiXssErrorResponse(HttpContext context, HttpContextError errorType, string? sanitizedBody = null)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/json";

        var errorMessage = string.Empty;

        switch (errorType)
        {
            case HttpContextError.Path:
                errorMessage = "Request contains potentially dangerous input in the http path request which has been rejected.";
                this.LogInformation(errorMessage, context.Request.Path.Value!);

                break;

            case HttpContextError.QueryString:
                errorMessage = "Request contains potentially dangerous input in the http query string request which has been rejected.";
                this.LogInformation(errorMessage, context.Request.QueryString.Value!);

                break;

            case HttpContextError.Body:
                errorMessage = "Request contains potentially dangerous input in the http body request which has been rejected.";
                this.LogInformation(errorMessage, sanitizedBody ?? string.Empty);

                break;
        }

        await context.Response.WriteAsync(errorMessage);
    }

    private void LogInformation(string error, string xssSanitizedContent)
    {
        //LogScope.Current.Write<MerchantServiceLog>(new LogEntry(message: error, submessage: $"Sanitized_Content: {xssSanitizedContent}",
        //    severity: LogSeverity.Information, category: new EntryCategory("AntiXssMiddleware")));
    }

    public enum HttpContextError
    {
        Path,
        QueryString,
        Body,
    }
}