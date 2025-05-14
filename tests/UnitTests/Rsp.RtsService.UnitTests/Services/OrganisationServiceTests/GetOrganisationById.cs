using Mapster;
using Moq;
using Rsp.RtsService.Application.Contracts.Repositories;
using Rsp.RtsService.Application.DTOS.Responses;
using Rsp.RtsService.Application.Specifications;
using Rsp.RtsService.Domain.Entities;
using Rsp.RtsService.Services;
using Shouldly;

namespace Rsp.RtsService.UnitTests.Services.OrganisationServiceTests;

public class GetOrganisationById : TestServiceBase<OrganisationService>
{
    [Fact]
    public async Task GetById_Should_Return_Mapped_Dto_When_Organisation_Exists()
    {
        // Arrange
        var orgId = "org-1";
        var organisation = new Organisation { Id = orgId, Name = "Test Org" };
        var expectedDto = organisation.Adapt<GetOrganisationByIdDto>();

        Mocker
            .GetMock<IOrganisationRepository>()
            .Setup(r => r.GetById(It.IsAny<OrganisationSpecification>()))
            .ReturnsAsync(organisation);

        // Act
        var result = await Sut.GetById(orgId);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(expectedDto.Id);
        result.Name.ShouldBe(expectedDto.Name);
    }

    [Fact]
    public async Task GetById_Should_Throw_When_Organisation_Not_Found()
    {
        // Arrange
        var orgId = "not-found";
        Mocker
            .GetMock<IOrganisationRepository>()
            .Setup(r => r.GetById(It.IsAny<OrganisationSpecification>()))
            .ReturnsAsync((Organisation?)null);

        // Act & Assert
        var result = await Sut.GetById(orgId);

        result.ShouldBeNull();
    }
}