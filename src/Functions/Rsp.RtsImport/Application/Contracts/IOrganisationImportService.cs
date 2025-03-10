namespace Rsp.RtsImport.Application.Contracts;

public interface IOrganisationImportService
{
    Task<int> ImportOrganisations(string dateModified, bool onlyActive = false);

    Task<int> ImportRoles(string dateModified, bool onlyActive = false);

    Task<int> ImportTermsets(string dateModified);
}