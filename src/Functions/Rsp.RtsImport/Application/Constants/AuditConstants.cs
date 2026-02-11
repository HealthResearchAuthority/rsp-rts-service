namespace Rsp.RtsImport.Application.Constants;

public static class AuditConstants
{
    public const string FunctionTimerStarted = "Function execution started from timer trigger.";

    public const string ApiDownloadStarted = "API download process started.";
    public const string ApiDownloadCompleted = "API download completed. {0} records pulled.";

    public const string DatabaseOrganisationInsertStarted = "Database Organisation insert process started.";
    public const string DatabaseOrganisationInsertCompleted = "Database Organisation insert process completed. {0} records inserted.";

    public const string DatabaseOrganisationRolesInsertStarted = "Database Organisation Roles insert process started.";
    public const string DatabaseOrganisationRolesInsertCompleted = "Database Organisation Roles insert process completed. {0} records inserted.";

    public const string FunctionTimerEnded = "Function execution completed from timer trigger.";

    public const string DatabaseSponsorOrganisationUpdateStarted = "Database Sponsor Organisation update process started.";
    public const string DatabaseSponsorOrganisationUpdateCompleted = "Database Sponsor Organisation update process completed.";
}