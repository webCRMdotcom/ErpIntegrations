using System;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Synchronisation.Fortnox;
using Webcrm.ErpIntegrations.Test;
using Xunit;
using Xunit.Abstractions;

namespace Webcrm.ErpIntegrations.Synchronisation.Test
{
    public class FortnoxChangeTrackerTester : BaseTester
    {
        public FortnoxChangeTrackerTester(ITestOutputHelper output) : base(output)
        {
            // It's safe to use .Result in the tests.
            ChangeTracker = Task.Run(() => FortnoxChangeTracker.Create(
                OutputLogger,
                TestTypedEnvironment.AzureWebJobsStorage)).Result;
        }

        private FortnoxChangeTracker ChangeTracker { get; }

        [Fact(Skip = "TODO 1458: Enable Fortnox.")]
        [Trait(Traits.Execution, Traits.Manual)]
        public async Task TestEnqueueUpsertedCustomers()
        {
            var anHourAgo = DateTime.UtcNow.AddHours(-1);
            await ChangeTracker.EnqueueUpsertedCustomers(anHourAgo, TestConfigurations.FortnoxConfiguration);
        }

        [Fact(Skip = "TODO 1458: Enable Fortnox.")]
        [Trait(Traits.Execution, Traits.Manual)]
        public async Task TestEnqueueUpsertedInvoices()
        {
            var anHourAgo = DateTime.UtcNow.AddHours(-1);
            await ChangeTracker.EnqueueUpsertedInvoices(anHourAgo, TestConfigurations.FortnoxConfiguration);
        }
    }
}