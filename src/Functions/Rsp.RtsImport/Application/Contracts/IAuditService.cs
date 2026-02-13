using Rsp.RtsImport.Application.Constants;

namespace Rsp.RtsImport.Application.Contracts;

public interface IAuditService
{
    Task FunctionStarted();

    Task ApiDownloadStarted();

    Task ApiDownloadCompleted(int recordCount);

    Task DatabaseOrganisationInsertStarted();

    Task DatabaseOrganisationInsertCompleted(int count);

    Task DatabaseOrganisationRolesInsertStarted();

    Task DatabaseOrganisationRolesInsertCompleted(int count);

    Task DatabaseSponsorOrganisationUpdateStarted();

    Task DatabaseSponsorOrganisationUpdateCompleted(int count);

    Task DatabaseSponsorOrganisationDisabled(string sponsorOrganisation);

    Task DatabaseSponsorOrganisationEnabled(string sponsorOrganisation);

    Task FunctionEnded();
}