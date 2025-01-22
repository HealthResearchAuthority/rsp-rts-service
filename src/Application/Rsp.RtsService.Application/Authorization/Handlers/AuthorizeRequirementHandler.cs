using System.Security.Claims;
using Rsp.RtsService.Application.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Rsp.Logging.Extensions;

namespace Rsp.RtsService.Application.Authorization.Handlers;

/// <summary>
/// Authorization handler to validate against the properties defined in the requirement
/// </summary>
public class AuthorizeRequirementHandler(ILogger<AuthorizeRequirementHandler> logger) : AuthorizationHandler<AuthorizeRequirement>
{
    /// <summary>
    /// Handles authorization requirement that user is in a particular role
    /// </summary>
    /// <param name="context">Authorization handler context</param>
    /// <param name="requirement">The requirement being handled by this handler. <see cref="AuthorizeRequirement"/></param>
    /// <returns>A task. Implementation marks the context as either fail or success</returns>
    /// <remarks>
    /// As per docs, Authorization handlers are called even if authentication fails. So always handle scenario in authorization handler when the
    /// user is not authenticated.
    /// </remarks>
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizeRequirement requirement)
    {
        var pendingRequirements = context.PendingRequirements;

        // Multiple requirements will be linked to the CustomAuthorizeAttribute or policy and will work
        // in a mutually exclusive manner. This means that if another requirement is met already by a handler
        // that was executed earlier than this then we don't need to process this requirement
        if (pendingRequirements.SingleOrDefault(pendingRequirement => pendingRequirement == requirement) == null)
        {
            return;
        }

        // if the context has failed
        // the common authorization requirement must not be met
        // user should have email and role claim
        if (context.HasFailed)
        {
            return;
        }

        var roleClaims = context.User.FindAll(ClaimTypes.Role).ToList();

        // user should be in the role specified by the requirement
        if (roleClaims.Find(claim => claim.Value == AuthorizeRequirement.Role) == null)
        {
            logger.LogErrorHp(string.Join(",", roleClaims), "ERR_AUTH_FAILED", "user is not in the required role");

            // Do not fail the requirement as the handler is meant to work as OR
            // so the next handler will pick the next requirement, uncomment if AND behaviour is intended
            // context.Fail(new AuthorizationFailureReason(this, "user is not in the required role"))

            return;
        }

        // if you have further requiements based on the http context
        // uncomment the following and validate as needed
        //if (context.Resource is not HttpContext httpContext)
        //{
        //    return;
        //}

        context.Succeed(requirement);

        logger.LogInformation("requirement was met successfully");

        await Task.CompletedTask;
    }
}