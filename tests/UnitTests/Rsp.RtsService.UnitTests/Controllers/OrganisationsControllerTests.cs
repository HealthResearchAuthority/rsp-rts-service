using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Rsp.RtsService.Application.Contracts.Services;
using Rsp.RtsService.Application.DTOS.Responses;
using Rsp.RtsService.Application.Enums;
using Rsp.RtsService.WebApi.Controllers;
using Shouldly;

namespace Rsp.RtsService.UnitTests.Controllers;

public class OrganisationsControllerTests : TestServiceBase
{
    private readonly OrganisationsController _controller;

    public OrganisationsControllerTests()
    {
        _controller = Mocker.CreateInstance<OrganisationsController>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("a")]
    [InlineData("ab")]
    public async Task SearchByName_ShouldReturnBadRequest_WhenNameTooShort(string name)
    {
        var result = await _controller.SearchByName(name);

        var badRequest = result.Result.ShouldBeOfType<BadRequestObjectResult>();
        badRequest.Value.ShouldBe("Name needs to include minimum 3 characters");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task SearchByName_ShouldReturnBadRequest_WhenPageIndexInvalid(int pageIndex)
    {
        var result = await _controller.SearchByName("ValidName", pageIndex);

        var badRequest = result.Result.ShouldBeOfType<BadRequestObjectResult>();
        badRequest.Value.ShouldBe("pageIndex must be greater than 0 if specified.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task SearchByName_ShouldReturnBadRequest_WhenPageSizeInvalid(int pageSize)
    {
        var result = await _controller.SearchByName("ValidName", 1, pageSize);

        var badRequest = result.Result.ShouldBeOfType<BadRequestObjectResult>();
        badRequest.Value.ShouldBe("pageSize must be greater than 0 if specified.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task GetAll_ShouldReturnBadRequest_WhenPageIndexInvalid(int pageIndex)
    {
        var result = await _controller.GetAll(pageIndex);

        var badRequest = result.Result.ShouldBeOfType<BadRequestObjectResult>();
        badRequest.Value.ShouldBe("pageIndex must be greater than 0 if specified.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public async Task GetAll_ShouldReturnBadRequest_WhenPageSizeInvalid(int pageSize)
    {
        var result = await _controller.GetAll(1, pageSize);

        var badRequest = result.Result.ShouldBeOfType<BadRequestObjectResult>();
        badRequest.Value.ShouldBe("pageSize must be greater than 0 if specified.");
    }

    [Theory]
    [AutoData]
    public async Task GetById_ShouldReturnOk_WhenFound(string id, GetOrganisationByIdDto dto)
    {
        var mock = Mocker.GetMock<IOrganisationService>();
        mock.Setup(s => s.GetById(id)).ReturnsAsync(dto);

        var result = await _controller.GetById(id);

        var ok = result.Result.ShouldBeOfType<OkObjectResult>();
        ok.Value.ShouldBe(dto);
    }

    [Theory]
    [AutoData]
    public async Task GetAll_ShouldReturnOk_WhenValid(OrganisationSearchResponse response)
    {
        var mock = Mocker.GetMock<IOrganisationService>();
        mock.Setup(s => s.GetAll(1, null, null, SortOrder.Ascending))
            .ReturnsAsync(response);

        var result = await _controller.GetAll();

        var ok = result.Result.ShouldBeOfType<OkObjectResult>();
        ok.Value.ShouldBe(response);
    }

    [Theory]
    [AutoData]
    public async Task SearchByName_ShouldReturnOk_WhenValid(string name, OrganisationSearchResponse response)
    {
        var mock = Mocker.GetMock<IOrganisationService>();
        mock.Setup(s => s.SearchByName(name, 1, null, null, SortOrder.Ascending))
            .ReturnsAsync(response);

        var result = await _controller.SearchByName(name);

        var ok = result.Result.ShouldBeOfType<OkObjectResult>();
        ok.Value.ShouldBe(response);
    }
}