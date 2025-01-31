using AutoFixture;
using AutoFixture.Xunit2;
using Rsp.RtsService.Application.Specifications;
using Rsp.RtsService.Domain.Entities;
using Shouldly;

namespace Rsp.RtsService.UnitTests.SpecificationsTests
{
    public class GetSpecificationTests
    {
        [Theory, AutoData]
        public void GetSpecification_ById_ReturnsCorrectSpecification(Generator<Organisation> generator)
        {
            // Arrange
            var entities = generator.Take(3).ToList();

            var spec = new OrganisationSpecification(entities[0].Name, entities[0].Type);

            // Act
            var result = spec
                .Evaluate(entities)
                .SingleOrDefault();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(entities[0].Id);
        }
    }
}