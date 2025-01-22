using System.Security.Claims;
using Rsp.RtsService.Application.Authorization.Handlers;
using Rsp.RtsService.Application.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace Rsp.RtsService.UnitTests.Authorization.AuthHandlers.AuthorizationRequirementHandlerTests;

public class HandleRequirementAsync : TestServiceBase
{
    [Fact]
    public async Task WithRequirementAlreadyMet_Returns_CompletedTask()
    {
        // Arrange
        var requirement = new AuthorizeRequirement();

        var authorizationHandlerContext = new AuthorizationHandlerContext
        (
            [requirement],
            new ClaimsPrincipal(),
            null
        );

        authorizationHandlerContext.Succeed(requirement);

        var assignedRoleHandler = Mocker.CreateInstance<AuthorizeRequirementHandler>();

        // Act
        await assignedRoleHandler.HandleAsync(authorizationHandlerContext);

        // Assert
        authorizationHandlerContext
            .HasFailed
            .ShouldBeFalse();

        authorizationHandlerContext
            .HasSucceeded
            .ShouldBeTrue();

        // Verify
        Mocker
            .GetMock<ILogger<AuthorizeRequirementHandler>>()
            .Verify(logger => logger.IsEnabled(LogLevel.Information), Times.Never);
    }

    [Fact]
    public async Task WithRequirementFails_Returns_CompletedTask()
    {
        // Arrange
        var authorizationHandlerContext = new AuthorizationHandlerContext
        (
            [new AuthorizeRequirement()],
            new ClaimsPrincipal(),
            null
        );

        // if the requirement was failed previously
        authorizationHandlerContext.Fail();

        var assignedRoleHandler = Mocker.CreateInstance<AuthorizeRequirementHandler>();

        // Act
        await assignedRoleHandler.HandleAsync(authorizationHandlerContext);

        // Assert
        authorizationHandlerContext
            .HasFailed
            .ShouldBeTrue();

        authorizationHandlerContext
            .HasSucceeded
            .ShouldBeFalse();
    }

    [Fact]
    public async Task WithUserHasNoValidRoles_Returns()
    {
        // Arrange
        var identity = new ClaimsIdentity
        (
            [
                new(ClaimTypes.Role, "admin"),
                new(ClaimTypes.Role, "user")
            ],
            "any"
        );

        var authorizationHandlerContext = new AuthorizationHandlerContext
        (
            [new AuthorizeRequirement()],
            new ClaimsPrincipal(identity),
            null
        );

        var assignedRoleHandler = Mocker.CreateInstance<AuthorizeRequirementHandler>();

        // Act
        await assignedRoleHandler.HandleAsync(authorizationHandlerContext);

        // Assert
        authorizationHandlerContext
            .HasFailed
            .ShouldBeFalse();

        authorizationHandlerContext
            .HasSucceeded
            .ShouldBeFalse();

        // Verify
        Mocker
            .GetMock<ILogger<AuthorizeRequirementHandler>>()
            .Verify(logger => logger.IsEnabled(LogLevel.Error), Times.Once);
    }

    [Fact]
    public async Task WithNoValidStatuses_Returns()
    {
        // Arrange
        var identity = new ClaimsIdentity
        (
            [
                new(ClaimTypes.Role, "reviewer"),
                new(ClaimTypes.Role, "user")
            ],
            "any"
        );

        var authorizationHandlerContext = new AuthorizationHandlerContext
        (
            [new AuthorizeRequirement()],
            new ClaimsPrincipal(identity),
            new DefaultHttpContext()
        );

        var assignedRoleHandler = Mocker.CreateInstance<AuthorizeRequirementHandler>();

        // Act
        await assignedRoleHandler.HandleAsync(authorizationHandlerContext);

        // Assert
        authorizationHandlerContext
            .HasFailed
            .ShouldBeFalse();

        authorizationHandlerContext
            .HasSucceeded
            .ShouldBeFalse();

        // Verify
        Mocker
            .GetMock<ILogger<AuthorizeRequirementHandler>>()
            .Verify(logger => logger.IsEnabled(LogLevel.Error), Times.Once);
    }

    [Fact]
    public async Task WithValidStatuses_Returns()
    {
        // Arrange
        var identity = new ClaimsIdentity
        (
            [
                new(ClaimTypes.Role, "reviewer"),
                new(ClaimTypes.Role, "user")
            ],
            "any"
        );

        var routeValues = new RouteValuesFeature
        {
            RouteValues = new RouteValueDictionary
            {
                { "status", "pending" }
            }
        };

        var routing = new RoutingFeature
        {
            RouteData = new RouteData(routeValues.RouteValues)
        };

        var httpContext = new DefaultHttpContext();
        httpContext.Features.Set<IRoutingFeature>(routing);

        var authorizationHandlerContext = new AuthorizationHandlerContext
        (
            [new AuthorizeRequirement()],
            new ClaimsPrincipal(identity),
            httpContext
        );

        var assignedRoleHandler = Mocker.CreateInstance<AuthorizeRequirementHandler>();

        // Act
        await assignedRoleHandler.HandleAsync(authorizationHandlerContext);

        // Assert
        authorizationHandlerContext
            .HasFailed
            .ShouldBeFalse();

        authorizationHandlerContext
            .HasSucceeded
            .ShouldBeTrue();

        // Verify
        Mocker
            .GetMock<ILogger<AuthorizeRequirementHandler>>()
            .Verify(logger => logger.IsEnabled(LogLevel.Information), Times.Once);
    }
}