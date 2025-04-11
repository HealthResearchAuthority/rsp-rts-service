namespace Rsp.RtsImport.Application.Contracts;

public interface IOrganisationImportService
{
    Task<int> ImportOrganisationsAndRoles(string lastUpdated, bool onlyActive = false);
}