using Microsoft.EntityFrameworkCore;
using Rsp.RtsImport.Application.Constants;
using Rsp.RtsImport.Services;
using Rsp.RtsService.Infrastructure;

namespace Rts.RtsImport.UnitTests.Services;

public class AuditServiceTests : TestServiceBase
{
    private readonly RtsDbContext _context;
    private readonly AuditService _service;

    public AuditServiceTests()
    {
        var options = new DbContextOptionsBuilder<RtsDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        _context = new RtsDbContext(options);
        Mocker.Use(_context);

        _service = Mocker.CreateInstance<AuditService>();
    }

    [Fact]
    public async Task FunctionStarted_AddsAuditEntry()
    {
        await _service.FunctionStarted();

        var audit = await _context.Audit.FirstOrDefaultAsync();
        Assert.NotNull(audit);
        Assert.Equal(AuditConstants.FunctionTimerStarted, audit.Description);
    }

    [Fact]
    public async Task ApiDownloadStarted_AddsAuditEntry()
    {
        await _service.ApiDownloadStarted();

        var audit = await _context.Audit.FirstOrDefaultAsync();
        Assert.NotNull(audit);
        Assert.Equal(AuditConstants.ApiDownloadStarted, audit.Description);
    }

    [Fact]
    public async Task ApiDownloadCompleted_AddsFormattedAuditEntry()
    {
        var recordCount = 50;
        await _service.ApiDownloadCompleted(recordCount);

        var audit = await _context.Audit.FirstOrDefaultAsync();
        Assert.NotNull(audit);
        Assert.Equal(string.Format(AuditConstants.ApiDownloadCompleted, recordCount), audit.Description);
    }

    [Fact]
    public async Task DatabaseOrganisationInsertStarted_AddsAuditEntry()
    {
        await _service.DatabaseOrganisationInsertStarted();

        var audit = await _context.Audit.FirstOrDefaultAsync();
        Assert.NotNull(audit);
        Assert.Equal(AuditConstants.DatabaseOrganisationInsertStarted, audit.Description);
    }

    [Fact]
    public async Task DatabaseOrganisationInsertCompleted_AddsFormattedAuditEntry()
    {
        var count = 25;
        await _service.DatabaseOrganisationInsertCompleted(count);

        var audit = await _context.Audit.FirstOrDefaultAsync();
        Assert.NotNull(audit);
        Assert.Equal(string.Format(AuditConstants.DatabaseOrganisationInsertCompleted, count), audit.Description);
    }

    [Fact]
    public async Task DatabaseOrganisationRolesInsertStarted_AddsAuditEntry()
    {
        await _service.DatabaseOrganisationRolesInsertStarted();

        var audit = await _context.Audit.FirstOrDefaultAsync();
        Assert.NotNull(audit);
        Assert.Equal(AuditConstants.DatabaseOrganisationRolesInsertStarted, audit.Description);
    }

    [Fact]
    public async Task DatabaseOrganisationRolesInsertCompleted_AddsFormattedAuditEntry()
    {
        var count = 17;
        await _service.DatabaseOrganisationRolesInsertCompleted(count);

        var audit = await _context.Audit.FirstOrDefaultAsync();
        Assert.NotNull(audit);
        Assert.Equal(string.Format(AuditConstants.DatabaseOrganisationRolesInsertCompleted, count), audit.Description);
    }

    [Fact]
    public async Task FunctionEnded_AddsAuditEntry()
    {
        await _service.FunctionEnded();

        var audit = await _context.Audit.FirstOrDefaultAsync();
        Assert.NotNull(audit);
        Assert.Equal(AuditConstants.FunctionTimerEnded, audit.Description);
    }

    [Fact]
    public async Task DatabaseSponsorOrganisationUpdateStarted_AddsAuditEntry()
    {
        await _service.DatabaseSponsorOrganisationUpdateStarted();

        var audit = await _context.Audit.FirstOrDefaultAsync();
        Assert.NotNull(audit);
        Assert.Equal(AuditConstants.DatabaseSponsorOrganisationUpdateStarted, audit.Description);
    }

    [Fact]
    public async Task DatabaseSponsorOrganisationUpdateCompleted_AddsAuditEntry()
    {
        var count = 17;
        await _service.DatabaseSponsorOrganisationUpdateCompleted(count);

        var audit = await _context.Audit.FirstOrDefaultAsync();
        Assert.NotNull(audit);
        Assert.Equal(string.Format(AuditConstants.DatabaseSponsorOrganisationUpdateCompleted, count), audit.Description);
    }
}