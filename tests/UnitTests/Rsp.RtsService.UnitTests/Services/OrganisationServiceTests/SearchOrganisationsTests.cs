using Ardalis.Specification;
using AutoFixture.Xunit2;
using Mapster;
using Moq;
using Rsp.RtsService.Application.Contracts.Repositories;
using Rsp.RtsService.Application.DTOS.Responses;
using Rsp.RtsService.Application.Specifications;
using Rsp.RtsService.Domain.Entities;
using Rsp.RtsService.Services;
using Shouldly;

namespace Rsp.RtsService.UnitTests.Services.OrganisationServiceTests;

public class SearchOrganisationsTests : TestServiceBase<OrganisationService>
{
    [Fact]
    public async Task SearchOrganisations_Should_Pass_Correct_Specification_And_Paging_Parameters()
    {
        // Arrange
        var request = new OrganisationsSearchRequest
        {
            SearchNameTerm = "Nhs",
            ExcludingRoles = ["role-1"],
            Countries = ["England"],
            OrganisationTypes = ["TypeA"],
            OrganisationStatuses = [true]
        };

        const int pageIndex = 2;
        int? pageSize = 15;
        const string sortField = "country";
        const string sortDirection = "desc";

        OrganisationsSearchSpecification? capturedSpec = null;
        int capturedPageIndex = -1;
        int? capturedPageSize = null;

        Mocker
            .GetMock<IOrganisationRepository>()
            .Setup(r => r.GetBySpecification(It.IsAny<OrganisationsSearchSpecification>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string>()))
            .Callback((ISpecification<Organisation> spec, int callbackPageIndex, int? callbackPageSize, string _, string __) =>
            {
                capturedSpec = spec as OrganisationsSearchSpecification;
                capturedPageIndex = callbackPageIndex;
                capturedPageSize = callbackPageSize;
            })
            .ReturnsAsync(([], 0));

        // Act
        await Sut.SearchOrganisations(request, pageIndex, pageSize, sortField, sortDirection);

        // Assert
        capturedSpec.ShouldNotBeNull();
        capturedPageIndex.ShouldBe(pageIndex);
        capturedPageSize.ShouldBe(pageSize);
    }

    [Fact]
    public async Task SearchOrganisations_Should_Return_Mapped_Dtos()
    {
        // Arrange
        var request = new OrganisationsSearchRequest { SearchNameTerm = "Org" };
        const int pageIndex = 1;
        const int pageSize = 10;

        var organisations = new List<Organisation>
        {
            new() { Id = "1", Name = "Org 1", Status = true, Address = "123 Main St", CountryName = "Poland", Type = "Local company" },
            new() { Id = "2", Name = "Org 2", Status = false, Address = "125 Main St", CountryName = "England", Type = "International" }
        };

        var expectedDtos = organisations.Adapt<IEnumerable<SearchOrganisationDto>>();

        Mocker
            .GetMock<IOrganisationRepository>()
            .Setup(r => r.GetBySpecification(It.IsAny<OrganisationsSearchSpecification>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((organisations, organisations.Count));

        // Act
        var result = await Sut.SearchOrganisations(request, pageIndex, pageSize);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldBeAssignableTo<OrganisationSearchResponse>();
        result.Organisations.ShouldBe(expectedDtos, ignoreOrder: false);
    }

    [Theory]
    [AutoData]
    public async Task SearchOrganisations_Should_Return_TotalCount_Of_Records(int totalCount)
    {
        // Arrange
        var request = new OrganisationsSearchRequest();
        const int pageIndex = 1;
        int? pageSize = null;

        Mocker
            .GetMock<IOrganisationRepository>()
            .Setup(r => r.GetBySpecification(It.IsAny<OrganisationsSearchSpecification>(), It.IsAny<int>(), It.IsAny<int?>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(([], totalCount));

        // Act
        var result = await Sut.SearchOrganisations(request, pageIndex, pageSize);

        // Assert
        result.TotalCount.ShouldBe(totalCount);
    }
}