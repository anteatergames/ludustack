using AutoFixture;
using AutoFixture.AutoNSubstitute;
using FluentAssertions;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Models;
using LuduStack.Domain.Services;
using LuduStack.Tests.Attributes;
using NSubstitute;
using System.Threading.Tasks;
using Xunit;

namespace LuduStack.Tests.Domain
{
    [Trait("Category", "Domain")]
    public class GameDomainServiceTests
    {
        public IFixture Fixture { get; }

        public GameDomainServiceTests()
        {
            AutoNSubstituteCustomization customization = new AutoNSubstituteCustomization { ConfigureMembers = true };

            Fixture = new Fixture().Customize(customization);
            Fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [Theory]
        [Trait("Method", "GetAll")]
        [AutoSubstituteData]
        public async Task ShouldReturnGames(IGameRepository repository)
        {
            GameDomainService sut = new GameDomainService(repository);

            System.Collections.Generic.IEnumerable<Game> result = sut.GetAll();

            await repository.Received().GetAll();

            result.Should().NotBeNull();
            result.Should().HaveCountGreaterOrEqualTo(0);
        }

        [Theory]
        [Trait("Method", "GetById")]
        [AutoSubstituteData]
        public async Task ShouldReturnSingleGame(IGameRepository repository)
        {
            Game obj = Fixture.Create<Game>();

            repository.GetById(obj.Id).Returns(obj);

            GameDomainService sut = new GameDomainService(repository);

            Game result = sut.GetById(obj.Id);

            await repository.Received().GetById(obj.Id);

            result.Should().NotBeNull();

            result.Id.Should().Be(obj.Id);
        }
    }
}