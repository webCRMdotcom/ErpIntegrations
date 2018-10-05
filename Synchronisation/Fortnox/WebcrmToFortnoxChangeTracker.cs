using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;
using Webcrm.ErpIntegrations.Configurations.Models;
using Webcrm.ErpIntegrations.Synchronisation.Fortnox.Models;

namespace Webcrm.ErpIntegrations.Synchronisation.Fortnox
{
    public sealed class WebcrmToFortnoxChangeTracker
    {
        private WebcrmToFortnoxChangeTracker(
            ILogger logger,
            WebcrmClientFactory webcrmClientFactory,
            FortnoxQueue fortnoxQueue)
        {
            Logger = logger;
            WebcrmClientFactory = webcrmClientFactory;
            Fortnox = fortnoxQueue;
        }

        public static WebcrmToFortnoxChangeTracker Create(
            ILogger logger,
            WebcrmClientFactory webcrmClientFactory,
            FortnoxQueue fortnoxQueue)
        {
            var webcrmChangeTracker = new WebcrmToFortnoxChangeTracker(logger, webcrmClientFactory, fortnoxQueue);
            return webcrmChangeTracker;
        }

        private ILogger Logger { get; }
        private FortnoxQueue Fortnox { get; }
        private WebcrmClientFactory WebcrmClientFactory { get; }

        public async Task EnqueueUpsertedItemsToFortnox(
            DateTime upsertedAfterUtc,
            BaseConfiguration configuration)
        {
            var webcrmClient = await WebcrmClientFactory.Create(configuration.WebcrmApiKey);
            // Synchronising organisations first, since the deliveries might depend on them.
            await EnqueueUpsertedOrganisations(upsertedAfterUtc, webcrmClient, configuration);

            if (configuration.SynchroniseDeliveries == SynchroniseDeliveries.ToErp)
                await EnqueueUpsertedDeliveries(upsertedAfterUtc, webcrmClient, configuration.WebcrmSystemId);
        }

        private async Task EnqueueUpsertedOrganisations(
            DateTime upsertedAfterUtc,
            WebcrmClient webcrmClient,
            BaseConfiguration configuration)
        {
            var upsertedOrganisations = await webcrmClient.GetUpsertedOrganisations(upsertedAfterUtc, configuration.AcceptedOrganisationStatuses, configuration.AcceptedOrganisationTypes);
            Logger.LogInformation($"Found {upsertedOrganisations.Count} organisations in webCRM upserted after {upsertedAfterUtc:yyyy-MM-dd HH:mm:ss}.");

            var organisationPayloads = upsertedOrganisations
                .Select(organisation => new UpsertOrganisationToFortnoxPayload(organisation, configuration.WebcrmSystemId));

            await EnqueueActions(FortnoxQueueAction.UpsertFortnoxOrganisation, organisationPayloads);
        }

        private async Task EnqueueUpsertedDeliveries(
            DateTime upsertedAfterUtc,
            WebcrmClient webcrmClient,
            string webcrmSystemId)
        {
            var upsertedDeliveries = await webcrmClient.GetUpsertedDeliveries(upsertedAfterUtc);
            Logger.LogInformation($"Found {upsertedDeliveries.Count} deliveries in webCRM upserted after {upsertedAfterUtc:yyyy-MM-dd HH:mm:ss}.");

            const int millisecondsDelayBetweenCalls = 20;
            var createDeliveryPayloadTasks = upsertedDeliveries
                .Select(async (delivery, index) =>
                {
                    // Adding an indexed delay before fetching the delivery lines to avoid that all the calls are made simultaneously in the WhenAll line below. Without this delay we get sporadic errors about 'connection was forcibly closed', 'unauthorized' or 'https connection was dropped dropped'. This may be due to an error in our REST API not handling a lot of simultaneous requests correctly.
                    await Task.Delay(index * millisecondsDelayBetweenCalls);
                    var deliveryLines = await webcrmClient.GetDeliveryLines(delivery.DeliveryId);
                    return new UpsertDeliveryToFortnoxPayload(delivery, deliveryLines, webcrmSystemId);
                });

            var deliveryPayloads = await Task.WhenAll(createDeliveryPayloadTasks);

            await EnqueueActions(FortnoxQueueAction.UpsertFortnoxDelivery, deliveryPayloads);
        }

        private async Task EnqueueActions(
            FortnoxQueueAction action,
            IEnumerable<BaseFortnoxPayload> payloads)
        {
            foreach (var payload in payloads)
            {
                var queueMessage = new FortnoxQueueMessage(action, payload);
                await Fortnox.Enqueue(queueMessage);
            }
        }
    }
}
