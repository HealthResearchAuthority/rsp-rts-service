namespace Rsp.RtsImport.Application.Contracts;

public interface IOrganisationImportService
{
    Task<int> ImportOrganisationsAndRoles(string dateModified, bool onlyActive = false);
}