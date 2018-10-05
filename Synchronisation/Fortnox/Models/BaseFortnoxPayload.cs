namespace Webcrm.ErpIntegrations.Synchronisation.Fortnox.Models
{
    public abstract class BaseFortnoxPayload
    {
        public string WebcrmSystemId { get; }

        protected BaseFortnoxPayload(
            string webcrmSystemId)
        {
            WebcrmSystemId = webcrmSystemId;
        }
    }
}