using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Webcrm.ErpIntegrations.Synchronisation.Fortnox.Models
{
    public class FortnoxQueueMessage
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public FortnoxQueueAction Action { get; set; }

        public string SerializedPayload { get; set; }

        [JsonConstructor]
        public FortnoxQueueMessage(
            FortnoxQueueAction action,
            string serializedPayload)
        {
            Action = action;
            SerializedPayload = serializedPayload;
        }

        public FortnoxQueueMessage(
            FortnoxQueueAction action,
            BaseFortnoxPayload payload)
            : this(action, JsonConvert.SerializeObject(payload))
        { }
    }
}