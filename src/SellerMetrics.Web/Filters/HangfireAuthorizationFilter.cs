using Hangfire.Dashboard;

namespace SellerMetrics.Web.Filters;

/// <summary>
/// Authorization filter for Hangfire dashboard.
/// Only allows authenticated users to access the dashboard.
/// </summary>
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Only authenticated users can access the Hangfire dashboard
        return httpContext.User.Identity?.IsAuthenticated == true;
    }
}
