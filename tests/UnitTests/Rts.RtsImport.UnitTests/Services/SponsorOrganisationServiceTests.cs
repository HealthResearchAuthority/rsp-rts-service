//using Moq;
//using Rsp.RtsImport.Services;
//using Rsp.RtsService.Application.Contracts.Repositories;
//using Rsp.RtsService.Application.Specifications;
//using Rsp.RtsService.Domain.Entities;

//namespace Rts.RtsImport.UnitTests.Services;

//public class SponsorOrganisationServiceTests : TestServiceBase<SponsorOrganisationService>
//{
//    [Fact]
//    public async Task DisableSponsorOrganisation_Disables_LoadsOrganisation_LogsAudit_ReturnsSponsorOrganisation()
//    {
//        // Arrange
//        const string rtsId = "RTS-123";

// var sponsorOrganisation = new SponsorOrganisation(); var organisation = new Organisation();

// var sponsorRepo = Mocker.GetMock<ISponsorOrganisationsRepository>(); var orgRepo =
// Mocker.GetMock<IOrganisationRepository>(); var auditRepo = Mocker.GetMock<ISponsorOrganisationAuditTrailRepository>();

// var sequence = new MockSequence();

// sponsorRepo.InSequence(sequence) .Setup(r => r.DisableSponsorOrganisation(rtsId)) .ReturnsAsync(sponsorOrganisation);

// orgRepo.InSequence(sequence) .Setup(r => r.GetById(It.Is<OrganisationSpecification>(s =>
// SpecMatchesRtsId(s, rtsId)))) .ReturnsAsync(organisation);

// auditRepo.InSequence(sequence) .Setup(r =>
// r.LogSponsorOrganisationAuditTrail(sponsorOrganisation, organisation)) .Returns(Task.CompletedTask);

// // Act var result = await Sut.DisableSponsorOrganisation(rtsId);

// // Assert Assert.Same(sponsorOrganisation, result);

// sponsorRepo.Verify(r => r.DisableSponsorOrganisation(rtsId), Times.Once); orgRepo.Verify(r =>
// r.GetById(It.IsAny<OrganisationSpecification>()), Times.Once); auditRepo.Verify(r =>
// r.LogSponsorOrganisationAuditTrail(sponsorOrganisation, organisation), Times.Once);

// sponsorRepo.VerifyNoOtherCalls(); orgRepo.VerifyNoOtherCalls(); auditRepo.VerifyNoOtherCalls(); }

// [Fact] public async Task
// EnableSponsorOrganisation_Enables_LoadsOrganisation_LogsAudit_ReturnsSponsorOrganisation() { //
// Arrange const string rtsId = "RTS-456";

// var sponsorOrganisation = new SponsorOrganisation(); var organisation = new Organisation();

// var sponsorRepo = Mocker.GetMock<ISponsorOrganisationsRepository>(); var orgRepo =
// Mocker.GetMock<IOrganisationRepository>(); var auditRepo = Mocker.GetMock<ISponsorOrganisationAuditTrailRepository>();

// var sequence = new MockSequence();

// sponsorRepo.InSequence(sequence) .Setup(r => r.EnableSponsorOrganisation(rtsId)) .ReturnsAsync(sponsorOrganisation);

// orgRepo.InSequence(sequence) .Setup(r => r.GetById(It.Is<OrganisationSpecification>(s =>
// SpecMatchesRtsId(s, rtsId)))) .ReturnsAsync(organisation);

// auditRepo.InSequence(sequence) .Setup(r =>
// r.LogSponsorOrganisationAuditTrail(sponsorOrganisation, organisation)) .Returns(Task.CompletedTask);

// // Act var result = await Sut.EnableSponsorOrganisation(rtsId);

// // Assert Assert.Same(sponsorOrganisation, result);

// sponsorRepo.Verify(r => r.EnableSponsorOrganisation(rtsId), Times.Once); orgRepo.Verify(r =>
// r.GetById(It.IsAny<OrganisationSpecification>()), Times.Once); auditRepo.Verify(r =>
// r.LogSponsorOrganisationAuditTrail(sponsorOrganisation, organisation), Times.Once);

// sponsorRepo.VerifyNoOtherCalls(); orgRepo.VerifyNoOtherCalls(); auditRepo.VerifyNoOtherCalls(); }

// [Fact] public async Task
// DisableSponsorOrganisation_WhenDisableThrows_DoesNotLookupOrganisationOrLogAudit() { // Arrange
// const string rtsId = "RTS-ERR";

// var sponsorRepo = Mocker.GetMock<ISponsorOrganisationsRepository>(); var orgRepo =
// Mocker.GetMock<IOrganisationRepository>(); var auditRepo = Mocker.GetMock<ISponsorOrganisationAuditTrailRepository>();

// sponsorRepo .Setup(r => r.DisableSponsorOrganisation(rtsId)) .ThrowsAsync(new InvalidOperationException("boom"));

// // Act + Assert await Assert.ThrowsAsync<InvalidOperationException>(() => Sut.DisableSponsorOrganisation(rtsId));

// sponsorRepo.Verify(r => r.DisableSponsorOrganisation(rtsId), Times.Once); orgRepo.Verify(r =>
// r.GetById(It.IsAny<OrganisationSpecification>()), Times.Never); auditRepo.Verify( r =>
// r.LogSponsorOrganisationAuditTrail(It.IsAny<SponsorOrganisation>(), It.IsAny<Organisation>()),
// Times.Never); }

// [Fact] public async Task EnableSponsorOrganisation_WhenOrganisationLookupThrows_DoesNotLogAudit()
// { // Arrange const string rtsId = "RTS-ORG-ERR";

// var sponsorOrganisation = new SponsorOrganisation();

// var sponsorRepo = Mocker.GetMock<ISponsorOrganisationsRepository>(); var orgRepo =
// Mocker.GetMock<IOrganisationRepository>(); var auditRepo = Mocker.GetMock<ISponsorOrganisationAuditTrailRepository>();

// sponsorRepo .Setup(r => r.EnableSponsorOrganisation(rtsId)) .ReturnsAsync(sponsorOrganisation);

// orgRepo .Setup(r => r.GetById(It.IsAny<OrganisationSpecification>())) .ThrowsAsync(new
// Exception("org lookup failed"));

// // Act + Assert await Assert.ThrowsAsync<Exception>(() => Sut.EnableSponsorOrganisation(rtsId));

// sponsorRepo.Verify(r => r.EnableSponsorOrganisation(rtsId), Times.Once); orgRepo.Verify(r =>
// r.GetById(It.IsAny<OrganisationSpecification>()), Times.Once); auditRepo.Verify( r =>
// r.LogSponsorOrganisationAuditTrail(It.IsAny<SponsorOrganisation>(), It.IsAny<Organisation>()),
// Times.Never); }

// // Keeps the tests resilient if OrganisationSpecification doesn't implement equality (and still
// // lets you assert it was created with the right rtsId if that value is exposed). private static
// bool SpecMatchesRtsId(OrganisationSpecification spec, string expectedRtsId) { var t = spec.GetType();

// foreach (var propName in new[] { "RtsId", "Id", "Value", "OrganisationId", "Key" }) { var prop =
// t.GetProperty(propName); if (prop?.PropertyType == typeof(string)) { var val =
// (string?)prop.GetValue(spec); return string.Equals(val, expectedRtsId, StringComparison.Ordinal);
// } }

//        return spec is not null;
//    }
//}