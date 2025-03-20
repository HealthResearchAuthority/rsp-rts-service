using Rsp.RtsImport.Application.DTO.Responses;
using Rsp.RtsService.Domain.Entities;

namespace Rsp.RtsImport.Application;

public static class DataModelConverters
{
    public static OrganisationTermset ConvertToDbModel(this RtsTermset input)
    {
        var output = new OrganisationTermset
        {
            Id = input.Identifier,
            EndDate = input.EffectiveEndDate,
            Imported = DateTime.Now,
            LastUpdated = input.ModifiedDate,
            Name = input.Name,
            StartDate = input.EffectiveStartDate,
            SystemUpdated = DateTime.Now,
        };
        return output;
    }

    public static OrganisationRole ConvertToDbModel(this RtsRole input)
    {
        var output = new OrganisationRole
        {
            Id = input.RoleType,
            EndDate = input.EffectiveEndDate,
            Imported = DateTime.Now,
            LastUpdated = input.ModifiedDate,
            StartDate = input.EffectiveStartDate,
            SystemUpdated = DateTime.Now,
            Scoper = 1,//!string.IsNullOrEmpty(input.ParentIdentifier) ? input.ParentIdentifier : string.Empty,
            CreatedDate = input.CreatedDate.GetValueOrDefault(),
            OrganisationId = input.OrgIdentifier,
            Status = input.Status
        };

        return output;
    }

    public static Organisation ConvertToDbModel(this RtsOrganisation input)
    {
        var output = new Organisation
        {
            Id = input.Identifier,
            Imported = DateTime.Now,
            LastUpdated = input.ModifiedDate,
            SystemUpdated = DateTime.Now,
            Status = true,//input.Status,
            Address = input.Address,
            CountryIdentifier = input.UKCountryIdentifier,
            CountryName = input.UKCountryName,
            Name = input.Name,
            Type = input.Type
        };
        return output;
    }
}