using FluentAssertions;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.Test;
using Xunit;
using Xunit.Abstractions;

namespace Webcrm.ErpIntegrations.Configurations.Test
{
    public class FortnoxConfigServiceTester : BaseTester
    {
        public FortnoxConfigServiceTester(ITestOutputHelper output) : base(output)
        {
            // It's safe to use .Result in the tests.
            ConfigService = Task.Run(() => FortnoxConfigService.Create(TestTypedEnvironment.DatabaseCredentials)).Result;
        }

        private FortnoxConfigService ConfigService { get; }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public void TestLoadFortnoxConfiguration()
        {
            var configuration = ConfigService.LoadFortnoxConfiguration(TestConfigurations.WebcrmSystemId);

            configuration.Should().NotBeNull();

            configuration.OrganisationIdFieldName.Should().Be(TestConfigurations.FortnoxOrganisationIdFieldName);
            configuration.FortnoxAccessToken.Should().Be(TestConfigurations.FortnoxAccessToken);
            configuration.FortnoxClientSecret.Should().Be(TestConfigurations.FortnoxClientSecret);
            configuration.WebcrmApiKey.Should().Be(TestConfigurations.WebcrmApiKey);
        }

        [Fact(Skip = "TODO 1458: Enable Fortnox.")]
        [Trait(Traits.Execution, Traits.Automatic)]
        public void TestLoadFortnoxConfigurations()
        {
            var configurations = ConfigService.LoadEnabledFortnoxConfigurations();

            configurations.Should().NotBeNull();
            configurations.Should().NotBeEmpty();
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestSaveFortnoxConfiguration()
        {
            await ConfigService.SaveFortnoxConfiguration(TestConfigurations.FortnoxConfiguration);
        }
    }
}