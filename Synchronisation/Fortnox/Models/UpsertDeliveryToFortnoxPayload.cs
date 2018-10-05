using System.Collections.Generic;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;

namespace Webcrm.ErpIntegrations.Synchronisation.Fortnox.Models
{
    internal class UpsertDeliveryToFortnoxPayload: BaseFortnoxPayload
    {
        public DeliveryDto WebcrmDelivery { get; }
        public List<QuotationLineDto> WebcrmDeliveryLines { get; }

        public UpsertDeliveryToFortnoxPayload(
           DeliveryDto webcrmDelivery,
           List<QuotationLineDto> webcrmDeliveryLines,
           string webcrmSystemId)
           : base(webcrmSystemId)
        {
            WebcrmDelivery = webcrmDelivery;
            WebcrmDeliveryLines = webcrmDeliveryLines;
        }
    }
}
