using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Xunit;

namespace Kevsoft.FeatureManagement.BranchByAbstraction.Tests
{
    public class BranchByAbstractionTests
    {
        [Fact]
        public async Task ShouldCreateColourDisplayType()
        {
            await using var serviceProvider = CreateServiceProvider(true);
            using var scope = serviceProvider.CreateScope();
            var factory = scope.ServiceProvider.GetRequiredService<IBranchFactory<IDisplay>>();

            (await factory.Create())
                .Should().BeOfType<ColourDisplay>();
        }

        [Fact]
        public async Task ShouldCreateBlackAndWhiteDisplayType()
        {
            await using var serviceProvider = CreateServiceProvider(false);
            using var scope = serviceProvider.CreateScope();
            var factory = scope.ServiceProvider.GetRequiredService<IBranchFactory<IDisplay>>();

            (await factory.Create())
                .Should().BeOfType<BlackAndWhiteDisplay>();
        }

        private static ServiceProvider CreateServiceProvider(bool featureEnabled)
        {
            var inMemoryCollection = new Dictionary<string, string>
            {
                ["FeatureManagement:ColourDisplay"] = featureEnabled.ToString(CultureInfo.InvariantCulture)
            };
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddFeatureManagement();
            serviceCollection.AddBranchByAbstraction<IDisplay, ColourDisplay, BlackAndWhiteDisplay>("ColourDisplay");
            var configurationBuilder = new ConfigurationBuilder();

            configurationBuilder.AddInMemoryCollection(inMemoryCollection);
            var implementationInstance = configurationBuilder.Build();
            serviceCollection.AddSingleton<IConfiguration>(implementationInstance);
            return serviceCollection.BuildServiceProvider();
        }
    }
}