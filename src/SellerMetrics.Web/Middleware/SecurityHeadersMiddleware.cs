namespace SellerMetrics.Web.Middleware;

/// <summary>
/// Middleware to add security headers to all responses.
/// These headers help protect against common web vulnerabilities.
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Prevent MIME type sniffing
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

        // Prevent clickjacking by disallowing iframe embedding
        context.Response.Headers.Append("X-Frame-Options", "DENY");

        // Enable XSS filter in browsers
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

        // Control referrer information sent with requests
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

        // Prevent browser from caching sensitive data
        context.Response.Headers.Append("Cache-Control", "no-store, no-cache, must-revalidate");
        context.Response.Headers.Append("Pragma", "no-cache");

        // Content Security Policy - restrict resource loading
        // Adjust as needed based on your application's requirements
        var csp = "default-src 'self'; " +
                  "script-src 'self' 'unsafe-inline' 'unsafe-eval' https://cdn.jsdelivr.net; " +
                  "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; " +
                  "img-src 'self' data:; " +
                  "font-src 'self' https://cdn.jsdelivr.net; " +
                  "form-action 'self'; " +
                  "frame-ancestors 'none'; " +
                  "base-uri 'self';";
        context.Response.Headers.Append("Content-Security-Policy", csp);

        // Permissions Policy - restrict browser features
        context.Response.Headers.Append("Permissions-Policy",
            "accelerometer=(), camera=(), geolocation=(), gyroscope=(), magnetometer=(), microphone=(), payment=(), usb=()");

        await _next(context);
    }
}

/// <summary>
/// Extension method to add security headers middleware to the pipeline.
/// </summary>
public static class SecurityHeadersMiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SecurityHeadersMiddleware>();
    }
}
