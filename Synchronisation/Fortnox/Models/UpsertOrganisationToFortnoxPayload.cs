using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;

namespace Webcrm.ErpIntegrations.Synchronisation.Fortnox.Models
{
    internal class UpsertOrganisationToFortnoxPayload : BaseFortnoxPayload
    {
        public OrganisationDto WebcrmOrganisation { get; }

        public UpsertOrganisationToFortnoxPayload(
            OrganisationDto webcrmOrganisation,
            string webcrmSystemId)
            : base(webcrmSystemId)
        {
            WebcrmOrganisation = webcrmOrganisation;
        }
    }
}
