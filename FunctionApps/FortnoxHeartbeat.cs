using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;
using Webcrm.ErpIntegrations.Configurations;
using Webcrm.ErpIntegrations.Synchronisation.Fortnox;

namespace Webcrm.ErpIntegrations.FunctionApps
{
    public static class FortnoxHeartbeat
    {
        [FunctionName("CopyFromFortnoxHeartbeat")]
        public static async Task CopyFromFortnox(
            // Trigger every six minutes.
            [TimerTrigger("0 */6 * * * *")] TimerInfo timer,
            ILogger logger)
        {
            try
            {
                var changeTracker = await FortnoxChangeTracker.Create(
                    logger,
                    TypedEnvironment.AzureWebJobsStorage);

                var configService = await FortnoxConfigService.Create(TypedEnvironment.DatabaseCredentials);
                var configurations = configService.LoadEnabledFortnoxConfigurations();
                foreach (var configuration in configurations)
                {
                    var dateTimeBeforeSync = DateTime.UtcNow;
                    await changeTracker.EnqueueUpsertedItems(configuration.LastSuccessfulCopyFromErpHeartbeat, configuration);
                    await configService.UpdateLastSuccessfulCopyFromErpHeartbeat(configuration.WebcrmSystemId, dateTimeBeforeSync);
                }
            }
            catch (SwaggerException ex)
            {
                SwaggerExceptionLogger.Log(ex);
            }
        }

        [FunctionName("CopyToFortnoxHeartbeat")]
        public static async Task CopyToFortnox(
            // Trigger every six minutes, offset by three.
            [TimerTrigger("0 3-59/6 * * * *")] TimerInfo timer,
            ILogger logger)
        {
            try
            {
                var webcrmClientFactory = new WebcrmClientFactory(logger, TypedEnvironment.WebcrmApiBaseUrl);
                var fortnoxQueue = await FortnoxQueue.Create(TypedEnvironment.AzureWebJobsStorage);
                var webcrmChangeTracker = WebcrmToFortnoxChangeTracker.Create(logger, webcrmClientFactory, fortnoxQueue);

                var configService = await FortnoxConfigService.Create(TypedEnvironment.DatabaseCredentials);
                var configurations = configService.LoadEnabledFortnoxConfigurations();
                foreach (var configuration in configurations)
                {
                    var dateTimeBeforeSync = DateTime.UtcNow;
                    await webcrmChangeTracker.EnqueueUpsertedItemsToFortnox(configuration.LastSuccessfulCopyToErpHeartbeat, configuration);
                    await configService.UpdateLastSuccessfulCopyToErpHeartbeat(configuration.WebcrmSystemId, dateTimeBeforeSync);
                }
            }
            catch (SwaggerException ex)
            {
                SwaggerExceptionLogger.Log(ex);
            }
        }
    }
}