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
        public void GetSpecification_ById_ReturnsCorrectSpecification(Generator<Entity> generator)
        {
            // Arrange
            var entities = generator.Take(3).ToList();

            var spec = new GetSpecification(entities[0].Id);

            // Act
            var result = spec
                .Evaluate(entities)
                .SingleOrDefault();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(entities[0].Id);
        }

        [Theory, InlineAutoData(5, 5), InlineAutoData(0, 10)]
        public void GetSpecification_ByRecords_ReturnsCorrectSpecification(int records, int expected, Generator<Entity> generator)
        {
            // Arrange
            var entities = generator.Take(10).ToList();

            // out of 10 records, it should return expected records
            var spec = new GetSpecification(records: records);

            // Act
            var result = spec
                .Evaluate(entities)
                .Count();

            // Assert
            result.ShouldBe(expected);
        }
    }
}