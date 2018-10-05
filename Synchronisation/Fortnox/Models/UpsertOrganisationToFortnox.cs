using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;

namespace Webcrm.ErpIntegrations.Synchronisation.Fortnox.Models
{
    public class UpsertOrganisationToFortnox : BaseFortnoxPayload
    {
        public OrganisationDto Organisation { get; }

        public UpsertOrganisationToFortnox(OrganisationDto organisation, string webcrmSystemId)
            : base(webcrmSystemId)
        {
            Organisation = organisation;
        }
    }
}