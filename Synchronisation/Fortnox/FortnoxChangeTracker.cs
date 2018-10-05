using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.ApiClients.FortnoxApiClient;
using Webcrm.ErpIntegrations.ApiClients.FortnoxApiClient.Models;
using Webcrm.ErpIntegrations.Configurations.Models;
using Webcrm.ErpIntegrations.GeneralUtilities;
using Webcrm.ErpIntegrations.Synchronisation.Fortnox.Models;

namespace Webcrm.ErpIntegrations.Synchronisation.Fortnox
{
    public sealed class FortnoxChangeTracker
    {
        private FortnoxChangeTracker(
            ILogger logger,
            FortnoxQueue fortnoxQueue)
        {
            Logger = logger;
            FortnoxQueue = fortnoxQueue;
        }

        public static async Task<FortnoxChangeTracker> Create(
            ILogger logger,
            string storageAccountConnectionString)
        {
            var fortnoxQueue = await FortnoxQueue.Create(storageAccountConnectionString);
            return new FortnoxChangeTracker(logger, fortnoxQueue);
        }

        private ILogger Logger { get; }
        private FortnoxQueue FortnoxQueue { get; }

        public async Task EnqueueUpsertedItems(
            DateTime upsertedAfterUtc,
            FortnoxConfiguration configuration)
        {
            await EnqueueUpsertedCustomers(upsertedAfterUtc, configuration);
            await EnqueueUpsertedInvoices(upsertedAfterUtc, configuration);
        }

        internal async Task EnqueueUpsertedCustomers(
            DateTime upsertedAfterUtc,
            FortnoxConfiguration configuration)
        {
            var fortnoxApiKeys = new FortnoxApiKeys(configuration.FortnoxAccessToken, configuration.FortnoxClientSecret);
            var fortnoxClient = new FortnoxClient(fortnoxApiKeys);

            // Convert utc date we pass in to either CEST or CET.
            var dateTimeSinceUpsertedSwedish = upsertedAfterUtc.FromUtcToSwedish();

            var upsertedCustomers = await fortnoxClient.GetRecentlyUpsertedCustomers(dateTimeSinceUpsertedSwedish);

            Logger.LogInformation($"Found {upsertedCustomers.Count} upserted organisations in Fortnox upserted since {dateTimeSinceUpsertedSwedish:yyyy-MM-dd HH:mm:ss}.");

            foreach (var customer in upsertedCustomers)
            {
                var payload = new UpsertOrganisationFromFortnoxPayload(customer.CustomerNumber, configuration.WebcrmSystemId);

                await FortnoxQueue.Enqueue(new FortnoxQueueMessage(FortnoxQueueAction.UpsertWebcrmOrganisation, payload));
            }
        }

        internal async Task EnqueueUpsertedInvoices(
            DateTime upsertedAfterUtc,
            FortnoxConfiguration configuration)
        {
            var fortnoxApiKeys = new FortnoxApiKeys(configuration.FortnoxAccessToken, configuration.FortnoxClientSecret);
            var fortnoxClient = new FortnoxClient(fortnoxApiKeys);

            // Convert utc date we pass in to either CEST or CET.
            var dateTimeSinceUpsertedSwedish = upsertedAfterUtc.FromUtcToSwedish();

            var upsertedInvoices = await fortnoxClient.GetRecentlyUpsertedInvoices(dateTimeSinceUpsertedSwedish);

            Logger.LogInformation($"Found {upsertedInvoices.Count} upserted invoices in Fortnox upserted since {dateTimeSinceUpsertedSwedish:yyyy-MM-dd HH:mm:ss}.");

            foreach (var invoice in upsertedInvoices)
            {
                var payload = new UpsertDeliveryFromFortnoxPayload(invoice.CustomerNumber, invoice.DocumentNumber, configuration.WebcrmSystemId);

                await FortnoxQueue.Enqueue(new FortnoxQueueMessage(FortnoxQueueAction.UpsertFortnoxDelivery, payload));
            }
        }
    }
}