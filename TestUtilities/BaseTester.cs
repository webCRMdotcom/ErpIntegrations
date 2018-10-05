using Microsoft.Extensions.Logging;
using Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;
using Webcrm.ErpIntegrations.Synchronisation;
using Xunit.Abstractions;

namespace Webcrm.ErpIntegrations.Test
{
    /// <summary>Includes a OutputLogger implementation that outputs lines to the console.</summary>
    public abstract class BaseTester
    {
        protected BaseTester(ITestOutputHelper output)
        {
            OutputLogger = new XunitLogger(output);
            TestPowerofficeClientFactory = new PowerofficeClientFactory(TestTypedEnvironment.PowerofficeApiSettings);
            TestPowerofficeQueueFactory = new PowerofficeQueueFactory(OutputLogger, TestTypedEnvironment.AzureWebJobsStorage);
            TestWebcrmClientFactory = new WebcrmClientFactory(OutputLogger, TestTypedEnvironment.WebcrmApiBaseUrl);
        }

        protected ILogger OutputLogger { get; }
        protected PowerofficeClientFactory TestPowerofficeClientFactory { get; }
        protected PowerofficeQueueFactory TestPowerofficeQueueFactory { get; }
        protected WebcrmClientFactory TestWebcrmClientFactory { get; }
    }
}