using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Webcrm.ErpIntegrations.Synchronisation.Models
{
    public class PowerofficeQueueMessage
    {
        [JsonConstructor]
        public PowerofficeQueueMessage(
            PowerofficeQueueAction action,
            string serializedPayload)
        {
            Action = action;
            SerializedPayload = serializedPayload;
        }

        public PowerofficeQueueMessage(
            PowerofficeQueueAction action,
            BasePowerofficePayload payload)
            : this(action, JsonConvert.SerializeObject(payload))
        { }

        [JsonConverter(typeof(StringEnumConverter))]
        public PowerofficeQueueAction Action { get; }

        public string SerializedPayload { get; }
    }
}