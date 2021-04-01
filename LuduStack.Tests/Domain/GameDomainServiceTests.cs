using AutoFixture;
using AutoFixture.AutoNSubstitute;
using FluentAssertions;
using LuduStack.Domain.Interfaces.Repository;
using LuduStack.Domain.Messaging.Queries.Game;
using LuduStack.Domain.Models;
using LuduStack.Domain.Services;
using LuduStack.Infra.CrossCutting.Messaging;
using LuduStack.Tests.Attributes;
using NSubstitute;
using System.Collections.Generic;
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
    }
}