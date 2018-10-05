using FluentAssertions;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.Configurations.Models;
using Webcrm.ErpIntegrations.Test;
using Xunit;
using Xunit.Abstractions;

namespace Webcrm.ErpIntegrations.Configurations.Test
{
    public class PowerofficeConfigServiceTester : BaseTester
    {
        public PowerofficeConfigServiceTester(ITestOutputHelper output) : base(output)
        {
            // It's safe to use .Result in the tests.
            ConfigService = Task.Run(() => PowerofficeConfigService.Create(TestTypedEnvironment.DatabaseCredentials)).Result;
        }

        private PowerofficeConfigService ConfigService { get; }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestCreateReadUpdateDeleteConfiguration()
        {
            var now = DateTime.UtcNow;
            const string systemId = "TestSystemId";
            const string originalPersonIdFieldName = "OriginalPersonIdFieldName";
            const string updatedPersonIdFieldName = "UpdatedPersonIdFieldName";

            var testConfiguration = new PowerofficeConfiguration
            {
                AcceptedOrganisationStatuses = new[] { "AcceptedOrganisationStatuses1", "AcceptedOrganisationStatuses2" },
                AcceptedOrganisationTypes = new[] { "AcceptedOrganisationTypes1", "AcceptedOrganisationTypes2" },
                DeliveryIdFieldName = "DeliveryIdFieldName",
                Disabled = true,
                LastSuccessfulCopyFromErpHeartbeat = now,
                LastSuccessfulCopyToErpHeartbeat = now,
                OrganisationCodeFieldName = "OrganisationCodeFieldName",
                OrganisationIdFieldName = "OrganisationIdFieldName",
                PersonIdFieldName = originalPersonIdFieldName,
                PowerofficeClientKey = new Guid("7931f515-1114-4215-940a-aa18c3b49f31"),
                PrimaryContactCheckboxFieldName = "PrimaryContactCheckboxFieldName",
                ProductCodeFieldName = "ProductCodeFieldName",
                ProductIdFieldName = "ProductIdFieldName",
                ProductNameFieldName = "ProductNameFieldName",
                WebcrmApiKey = "WebcrmApiKey",
                WebcrmSystemId = systemId
            };

            bool isValid = testConfiguration.Validate(out var results);
            foreach (var result in results)
                OutputLogger.LogTrace(result.ErrorMessage);

            isValid.Should().BeTrue();
            results.Should().HaveCount(0);

            await ConfigService.SavePowerofficeConfiguration(testConfiguration);
            var loadedConfiguration = ConfigService.LoadPowerofficeConfiguration(systemId);
            loadedConfiguration.Should().NotBeNull();
            loadedConfiguration.Disabled.Should().BeTrue();
            loadedConfiguration.PersonIdFieldName.Should().Be(originalPersonIdFieldName);
            loadedConfiguration.WebcrmSystemId.Should().Be(systemId);

            loadedConfiguration.PersonIdFieldName = updatedPersonIdFieldName;
            await ConfigService.SavePowerofficeConfiguration(loadedConfiguration);
            var updatedConfiguration = ConfigService.LoadPowerofficeConfiguration(systemId);
            updatedConfiguration.PersonIdFieldName.Should().Be(updatedPersonIdFieldName);

            await ConfigService.DeletePowerofficeConfiguration(updatedConfiguration);
            var deletedConfiguration = ConfigService.LoadPowerofficeConfiguration(systemId);
            deletedConfiguration.Should().BeNull();
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public void TestValidatePowerofficeConfiguration()
        {
            var invalidConfigurations = new[] {
                // PersonIdFieldName is missing in the config below.
                new PowerofficeConfiguration
                {
                    AcceptedOrganisationStatuses = new[] { "status" },
                    AcceptedOrganisationTypes = new [] { "type" },
                    DeliveryIdFieldName = "DeliveryIdFieldName",
                    Disabled = true,
                    LastSuccessfulCopyFromErpHeartbeat = DateTime.UtcNow,
                    LastSuccessfulCopyToErpHeartbeat = DateTime.UtcNow,
                    OrganisationCodeFieldName = "OrganisationCodeFieldName",
                    OrganisationIdFieldName = "OrganisationIdFieldName",
                    PowerofficeClientKey = new Guid("7931f515-1114-4215-940a-aa18c3b49f31"),
                    PrimaryContactCheckboxFieldName = "PrimaryContactCheckboxFieldName",
                    ProductCodeFieldName = "ProductCodeFieldName",
                    ProductIdFieldName = "ProductIdFieldName",
                    ProductNameFieldName = "ProductNameFieldName",
                    WebcrmApiKey = "WebcrmApiKey",
                    WebcrmSystemId = "WebcrmSystemId"
                },
                new PowerofficeConfiguration
                {
                    AcceptedOrganisationStatuses = new[] { "", " " },
                    AcceptedOrganisationTypes = new string[] { },
                    DeliveryIdFieldName = "DeliveryIdFieldName",
                    Disabled = true,
                    LastSuccessfulCopyFromErpHeartbeat = DateTime.UtcNow,
                    LastSuccessfulCopyToErpHeartbeat = DateTime.UtcNow,
                    OrganisationCodeFieldName = "OrganisationCodeFieldName",
                    OrganisationIdFieldName = "OrganisationIdFieldName",
                    PersonIdFieldName = "PersonIdFieldName",
                    PowerofficeClientKey = new Guid("7931f515-1114-4215-940a-aa18c3b49f31"),
                    PrimaryContactCheckboxFieldName = "PrimaryContactCheckboxFieldName",
                    ProductCodeFieldName = "ProductCodeFieldName",
                    ProductIdFieldName = "ProductIdFieldName",
                    ProductNameFieldName = "ProductNameFieldName",
                    WebcrmApiKey = "WebcrmApiKey",
                    WebcrmSystemId = "WebcrmSystemId"
                }
            };

            foreach (var invalidConfiguration in invalidConfigurations)
            {
                invalidConfiguration.Validate(out _).Should().BeFalse();
            }

            var validConfigurations = new[] {
                new PowerofficeConfiguration
                {
                    AcceptedOrganisationStatuses = new[] { "status" },
                    AcceptedOrganisationTypes = new [] { "" },
                    DeliveryIdFieldName = "DeliveryIdFieldName",
                    Disabled = true,
                    LastSuccessfulCopyFromErpHeartbeat = DateTime.UtcNow,
                    LastSuccessfulCopyToErpHeartbeat = DateTime.UtcNow,
                    OrganisationCodeFieldName = "OrganisationCodeFieldName",
                    OrganisationIdFieldName = "OrganisationIdFieldName",
                    PersonIdFieldName = "PersonIdFieldName",
                    PowerofficeClientKey = new Guid("7931f515-1114-4215-940a-aa18c3b49f31"),
                    PrimaryContactCheckboxFieldName = "PrimaryContactCheckboxFieldName",
                    ProductCodeFieldName = "ProductCodeFieldName",
                    ProductIdFieldName = "ProductIdFieldName",
                    ProductNameFieldName = "ProductNameFieldName",
                    WebcrmApiKey = "WebcrmApiKey",
                    WebcrmSystemId = "WebcrmSystemId"
                },
                new PowerofficeConfiguration
                {
                    AcceptedOrganisationStatuses = new[] { " " },
                    AcceptedOrganisationTypes = new [] { "type" },
                    DeliveryIdFieldName = "DeliveryIdFieldName",
                    Disabled = true,
                    LastSuccessfulCopyFromErpHeartbeat = DateTime.UtcNow,
                    LastSuccessfulCopyToErpHeartbeat = DateTime.UtcNow,
                    OrganisationCodeFieldName = "OrganisationCodeFieldName",
                    OrganisationIdFieldName = "OrganisationIdFieldName",
                    PersonIdFieldName = "PersonIdFieldName",
                    PowerofficeClientKey = new Guid("7931f515-1114-4215-940a-aa18c3b49f31"),
                    PrimaryContactCheckboxFieldName = "PrimaryContactCheckboxFieldName",
                    ProductCodeFieldName = "ProductCodeFieldName",
                    ProductIdFieldName = "ProductIdFieldName",
                    ProductNameFieldName = "ProductNameFieldName",
                    WebcrmApiKey = "WebcrmApiKey",
                    WebcrmSystemId = "WebcrmSystemId"
                }
            };

            foreach (var validConfiguration in validConfigurations)
            {
                bool isValid = validConfiguration.Validate(out var results);
                foreach (var result in results)
                    OutputLogger.LogTrace(result.ErrorMessage);

                isValid.Should().BeTrue();
            }
        }

        /// <summary>This test verifies that the PowerOffice configuration stored in Azure matches the one hardcoded in `TestConfigurations`. The test `SystemConfiguratorTester.TestPowerofficeSetup` copies the values in `TestConfigurations` to the database in Azure.</summary>
        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public void TestLoadPowerofficeConfiguration()
        {
            var configuration = ConfigService.LoadPowerofficeConfiguration(TestConfigurations.WebcrmSystemId);

            configuration.Should().NotBeNull();
            configuration.AcceptedOrganisationStatuses.Should().Equal(TestConfigurations.PowerofficeConfiguration.AcceptedOrganisationStatuses);
            configuration.AcceptedOrganisationTypes.Should().Equal(TestConfigurations.PowerofficeConfiguration.AcceptedOrganisationTypes);
            configuration.DeliveryIdFieldName.Should().Be(TestConfigurations.PowerofficeConfiguration.DeliveryIdFieldName);
            configuration.OrganisationIdFieldName.Should().Be(TestConfigurations.PowerofficeConfiguration.OrganisationIdFieldName);
            configuration.PersonIdFieldName.Should().Be(TestConfigurations.PowerofficeConfiguration.PersonIdFieldName);
            configuration.PowerofficeClientKey.Should().Be(TestConfigurations.PowerofficeConfiguration.PowerofficeClientKey);
            configuration.ProductIdFieldName.Should().Be(TestConfigurations.PowerofficeConfiguration.ProductIdFieldName);
            configuration.ProductNameFieldName.Should().Be(TestConfigurations.PowerofficeConfiguration.ProductNameFieldName);
            configuration.ProductUnitFieldName.Should().Be(TestConfigurations.PowerofficeConfiguration.ProductUnitFieldName);
            configuration.WebcrmApiKey.Should().Be(TestConfigurations.WebcrmApiKey);
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public void TestLoadEnabledPowerofficeConfigurations()
        {
            var configurations = ConfigService.LoadEnabledPowerofficeConfigurations();

            configurations.Should().NotBeNull();
            configurations.Should().NotBeEmpty();
        }

        [Fact]
        [Trait(Traits.Execution, Traits.Automatic)]
        public async Task TestSavePowerofficeConfiguration()
        {
            await ConfigService.SavePowerofficeConfiguration(TestConfigurations.PowerofficeConfiguration);
        }
    }
}