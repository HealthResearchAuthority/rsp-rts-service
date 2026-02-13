using Moq;
using Rsp.RtsImport.Application.ServiceClients;
using Rsp.RtsImport.Services;

namespace Rts.RtsImport.UnitTests.Services;

public class SponsorOrganisationServiceTests : TestServiceBase<SponsorOrganisationService>
{
    [Fact]
    public async Task DisableSponsorOrganisation_CallsClientOnce_WithRtsId()
    {
        // Arrange
        const string rtsId = "RTS-123";

        var client = Mocker.GetMock<ISponsorOrganisationsServiceClient>();
        client.Setup(c => c.DisableSponsorOrganisation(rtsId));

        // Act
        await Sut.DisableSponsorOrganisation(rtsId);

        // Assert
        client.Verify(c => c.DisableSponsorOrganisation(rtsId), Times.Once);
        client.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task EnableSponsorOrganisation_CallsClientOnce_WithRtsId()
    {
        // Arrange
        const string rtsId = "RTS-456";

        var client = Mocker.GetMock<ISponsorOrganisationsServiceClient>();
        client.Setup(c => c.EnableSponsorOrganisation(rtsId));

        // Act
        await Sut.EnableSponsorOrganisation(rtsId);

        // Assert
        client.Verify(c => c.EnableSponsorOrganisation(rtsId), Times.Once);
        client.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DisableSponsorOrganisation_WhenClientThrows_Throws_AndNoOtherCalls()
    {
        // Arrange
        const string rtsId = "RTS-ERR";

        var client = Mocker.GetMock<ISponsorOrganisationsServiceClient>();
        client.Setup(c => c.DisableSponsorOrganisation(rtsId))
              .ThrowsAsync(new InvalidOperationException("boom"));

        // Act + Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => Sut.DisableSponsorOrganisation(rtsId));

        client.Verify(c => c.DisableSponsorOrganisation(rtsId), Times.Once);
        client.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task EnableSponsorOrganisation_WhenClientThrows_Throws_AndNoOtherCalls()
    {
        // Arrange
        const string rtsId = "RTS-ORG-ERR";

        var client = Mocker.GetMock<ISponsorOrganisationsServiceClient>();
        client.Setup(c => c.EnableSponsorOrganisation(rtsId))
              .ThrowsAsync(new Exception("client failed"));

        // Act + Assert
        await Assert.ThrowsAsync<Exception>(() => Sut.EnableSponsorOrganisation(rtsId));

        client.Verify(c => c.EnableSponsorOrganisation(rtsId), Times.Once);
        client.VerifyNoOtherCalls();
    }
}