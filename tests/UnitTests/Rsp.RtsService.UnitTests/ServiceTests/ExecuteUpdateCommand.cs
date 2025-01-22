using AutoFixture;
using AutoFixture.Xunit2;
using Rsp.RtsService.Application.Contracts.Repositories;
using Rsp.RtsService.Application.DTOS.Requests;
using Rsp.RtsService.Application.DTOS.Responses;
using Rsp.RtsService.Domain.Entities;
using Rsp.RtsService.Infrastructure.Repositories;
using Rsp.RtsService.Services;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using static Rsp.RtsService.UnitTests.TestData;

namespace Rsp.RtsService.UnitTests.ServiceTests;

/// <summary>
/// Covers the tests for ExcecuteUpdateCommand method
/// </summary>
public class ExecuteUpdateCommand : TestServiceBase<Service>
{
    private readonly Infrastructure.DbContext _context;

    private readonly Repository _repository;

    public ExecuteUpdateCommand()
    {
        var options = new DbContextOptionsBuilder<Infrastructure.DbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString("N")).Options;

        _context = new Infrastructure.DbContext(options);
        _repository = new Repository(_context);
    }

    /// <summary>
    /// Tests that entity is updated
    /// </summary>
    /// <param name="commandRequest">Represents the model for entity request</param>
    [Theory, InlineAutoData(1)]
    public async Task Updates_And_Returns_CommandResponse(int records, Generator<Entity> generator, CommandRequest commandRequest)
    {
        // Arrange
        Mocker.Use<IRepository>(_repository);

        Sut = Mocker.CreateInstance<Service>();

        // seed data with the number of records
        var entities = await SeedData(_context, generator, records);

        commandRequest.Id = entities[0].Id;

        // Act
        var response = await Sut.ExcecuteUpdateCommand(commandRequest.Id, commandRequest);

        // Assert
        response.ShouldNotBeNull();
        response.ShouldBeOfType<CommandResponse>();
        response.ShouldSatisfyAllConditions
        (
            app => app.Id.ShouldBe(commandRequest.Id)
            // check for more properties here
        );
    }

    /// <summary>
    /// Tests that entity is created
    /// </summary>
    /// <param name="commandRequest">Represents the model for new entity request</param>
    [Theory, InlineAutoData(1)]
    public async Task Throws_Exception_If_Id_DoesNotExist(int records, Generator<Entity> generator, CommandRequest commandRequest)
    {
        // Arrange
        Mocker.Use<IRepository>(_repository);

        Sut = Mocker.CreateInstance<Service>();

        // seed data with the number of records
        var entities = await SeedData(_context, generator, records);

        // get the id that won't exists
        commandRequest.Id = entities[0].Id + 1;

        // Act/Assert
        await Should.ThrowAsync<NotImplementedException>(Sut.ExcecuteUpdateCommand(commandRequest.Id, commandRequest));
    }
}