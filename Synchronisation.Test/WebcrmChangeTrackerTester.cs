using System;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Test;
using Xunit;
using Xunit.Abstractions;

namespace Webcrm.ErpIntegrations.Synchronisation.Test
{
    public class WebcrmChangeTrackerTester : BaseTester
    {
        public WebcrmChangeTrackerTester(ITestOutputHelper output) : base(output)
        {
            // It's safe to use .Result in the tests.
            ChangeTracker = Task.Run(() => WebcrmToPowerofficeChangeTracker.Create(
                OutputLogger,
                TestWebcrmClientFactory,
                TestPowerofficeQueueFactory)).Result;
        }

        private WebcrmToPowerofficeChangeTracker ChangeTracker { get; }

        [Fact]
        [Trait(Traits.Execution, Traits.Manual)]
        public async Task TestEnqueueUpsertedItemsToPoweroffice()
        {
            DateTime yesterday = DateTime.UtcNow.AddDays(-1);
            await ChangeTracker.EnqueueUpsertedItemsToPoweroffice(yesterday, TestConfigurations.PowerofficeConfiguration);
        }
    }
}