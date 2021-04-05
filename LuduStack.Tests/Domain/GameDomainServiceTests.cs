using AutoFixture;
using AutoFixture.AutoNSubstitute;
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
    }
}