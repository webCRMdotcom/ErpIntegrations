namespace Webcrm.ErpIntegrations.Synchronisation.Fortnox.Models
{
    public class UpsertOrganisationFromFortnoxPayload : BaseFortnoxPayload
    {
        public string FortnoxCustomerNumber { get; }

        public UpsertOrganisationFromFortnoxPayload(
            string fortnoxCustomerNumber,
            string webcrmSystemId
            )
            : base(webcrmSystemId)
        {
            FortnoxCustomerNumber = fortnoxCustomerNumber;
        }
    }
}