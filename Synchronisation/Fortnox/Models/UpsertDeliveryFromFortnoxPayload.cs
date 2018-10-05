namespace Webcrm.ErpIntegrations.Synchronisation.Fortnox.Models
{
    public class UpsertDeliveryFromFortnoxPayload : BaseFortnoxPayload
    {
        public string FortnoxCustomerNumber { get; }
        public string FortnoxDocumentNumber { get; }

        public UpsertDeliveryFromFortnoxPayload(
            string fortnoxCustomerNumber,
            string fortnoxDocumentNumber,
            string webcrmSystemId)
            : base(webcrmSystemId)
        {
            FortnoxCustomerNumber = fortnoxCustomerNumber;
            FortnoxDocumentNumber = fortnoxDocumentNumber;
        }
    }
}