using System;
using System.Threading.Tasks;
using FluentAssertions;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Test;
using Xunit;
using Xunit.Abstractions;

namespace Webcrm.ErpIntegrations.ApiClients.Test
{
    public class WebcrmClientTester : BaseTester
    {
        public WebcrmClientTester(ITestOutputHelper output) : base(output)
        {
            // It's safe to use .Result in the tests.
            Client = Task.Run(() => WebcrmClient.Create(
                OutputLogger,
                TestTypedEnvironment.WebcrmApiBaseUrl,
                TestConfigurations.WebcrmApiKey)).Result;
        }

        private WebcrmClient Client { get; }

        [Theory]
        [Trait(Traits.Execution, Traits.Automatic)]
        [InlineData(1, "webCRM")]
        public async Task TestGetOrganisationById(int organisationId, string organisationName)
        {
            var organisation = await Client.GetOrganisationById(organisationId);
            organisation.OrganisationName.Should().Be(organisationName);
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestGetUpsertedDeliveries()
        {
            var longAgo = DateTime.UtcNow.AddDays(-1000);
            var upsertedDeliveries = await Client.GetUpsertedDeliveries(longAgo);
            upsertedDeliveries.Should().HaveCountGreaterOrEqualTo(2);
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestGetUpsertedOrganisationsByStatus()
        {
            var longAgo = DateTime.UtcNow.AddDays(-1000);
            string[] acceptedStatuses = { "A Kunde", "B Kunde" };
            var upsertedOrganisations = await Client.GetUpsertedOrganisations(longAgo, acceptedStatuses, null);
            upsertedOrganisations.Should().HaveCountGreaterOrEqualTo(2);

            foreach (var organisation in upsertedOrganisations)
            {
                acceptedStatuses.Should().Contain(organisation.OrganisationStatus);
            }
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestGetUpsertedOrganisationsByType()
        {
            var longAgo = DateTime.UtcNow.AddDays(-1000);
            string[] acceptedTypes = { "Leverandør", "Partner" };
            var upsertedOrganisations = await Client.GetUpsertedOrganisations(longAgo, null, acceptedTypes);
            upsertedOrganisations.Should().HaveCountGreaterOrEqualTo(2);
            upsertedOrganisations.Should().HaveCountGreaterOrEqualTo(2);

            foreach (var organisation in upsertedOrganisations)
            {
                acceptedTypes.Should().Contain(organisation.OrganisationType);
            }
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestGetUpsertedPersons()
        {
            var longAgo = DateTime.Now.AddDays(-1000);
            var upsertedPersons = await Client.GetUpsertedPersons(longAgo, TestConfigurations.PowerofficeConfiguration);
            upsertedPersons.Should().HaveCountGreaterOrEqualTo(2);
        }
    }
}