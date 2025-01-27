using Microsoft.AspNetCore.Authorization;

namespace Rsp.RtsService.Application.Authorization.Requirements;

/// <summary>
/// Authorization requirement to check if the user is authorized via handler
/// - if user is in a role
/// </summary>
public class AuthorizeRequirement : IAuthorizationRequirement
{
    /// <summary>
    /// User should be in this role
    /// </summary>
    public static string Role => "reviewer";

    /// <summary>
    /// List of application statuses reviewer can query.
    /// These will be retrieved from the database for the user
    /// </summary>
    public IEnumerable<string> AllowedStatuses { get; set; } = [];
}