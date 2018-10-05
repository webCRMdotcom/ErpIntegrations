using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Webcrm.ErpIntegrations.ApiClients.FortnoxApiClient;
using Webcrm.ErpIntegrations.ApiClients.FortnoxApiClient.Models;
using Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient;
using Webcrm.ErpIntegrations.Configurations.Models;
using Webcrm.ErpIntegrations.GeneralUtilities;

namespace Webcrm.ErpIntegrations.Synchronisation.Fortnox
{
    internal sealed class FortnoxDataCopier
    {
        private FortnoxDataCopier(
            ILogger logger,
            FortnoxApiKeys fortnoxApiKeys,
            WebcrmClient webcrmClient)
        {
            Logger = logger;
            FortnoxClient = new FortnoxClient(fortnoxApiKeys);
            WebcrmClient = webcrmClient;
        }

        internal static async Task<FortnoxDataCopier> Create(
            ILogger logger,
            WebcrmClientFactory webcrmClientFactory,
            FortnoxConfiguration configuration)
        {
            var webcrmClient = await webcrmClientFactory.Create(configuration.WebcrmApiKey);
            var fortnoxApiKeys = new FortnoxApiKeys(configuration.FortnoxAccessToken, configuration.FortnoxClientSecret);
            return new FortnoxDataCopier(logger, fortnoxApiKeys, webcrmClient);
        }

        private FortnoxClient FortnoxClient { get; }
        private ILogger Logger { get; }
        private WebcrmClient WebcrmClient { get; }

        internal async Task CopyDeliveryToFortnox(
            string payloadFortnoxCustomerNumber,
            string fortnoxCustomerNumber,
            string organisationIdFieldName)
        {
            // TODO FORTNOX: Implement this. Using an empty await to satisfy the compiler.
            await Task.Run(() => { });
        }

        internal async Task CopyOrganisationFromFortnox(string customerNumber, string organisationIdFieldName)
        {
            var customer = await FortnoxClient.GetCustomer(customerNumber);

            if (customer == null)
            {
                Logger.LogWarning($"Fortnox customer '{customerNumber}' could not be found.");
                return;
            }

            Logger.LogInformation($"Found and retrieved fortnox customer with customer number '{customer.CustomerNumber}'.");

            var correspondingWebcrmOrganisation = await WebcrmClient.GetOrganisationByField(organisationIdFieldName, customerNumber);

            if (correspondingWebcrmOrganisation == null)
            {
                var newWebcrmOrganisation = new OrganisationDto();
                CopyOrganisationProperties(customer, newWebcrmOrganisation, organisationIdFieldName);
                await WebcrmClient.CreateOrganisation(newWebcrmOrganisation);
            }
            else
            {
                if (!customer.HasRelevantChanges(correspondingWebcrmOrganisation))
                    return;

                CopyOrganisationProperties(customer, correspondingWebcrmOrganisation, organisationIdFieldName);
                await WebcrmClient.UpdateOrganisation(correspondingWebcrmOrganisation);
            }
        }

        internal async Task CopyOrganisationToFortnox(OrganisationDto organisation, string organisationIdFieldName)
        {
            var customerNumber = organisation.GetPropertyValue(organisationIdFieldName);
            if (string.IsNullOrWhiteSpace(customerNumber))
            {
                var newFortnoxOrganisation = new Customer();
                CopyOrganisationProperties(organisation, newFortnoxOrganisation);
                var newCustomer = await FortnoxClient.CreateCustomer(newFortnoxOrganisation);

                organisation.SetPropertyValue(organisationIdFieldName, newCustomer.CustomerNumber);
                await WebcrmClient.UpdateOrganisation(organisation);
            }
            else
            {
                var customer = await FortnoxClient.GetCustomer(customerNumber);
                if (!customer.HasRelevantChanges(organisation))
                    return;

                CopyOrganisationProperties(organisation, customer);
                await FortnoxClient.UpdateCustomer(customer);
            }
        }

        private void CopyOrganisationProperties(
            Customer sourceFortnoxOrganisation,
            OrganisationDto targetWebcrmOrganisation,
            string organisationIdFieldName)
        {
            // Map Fortnox customer to webCRM organisation.
            targetWebcrmOrganisation.OrganisationAddress = sourceFortnoxOrganisation.ConcatenatedAddress;
            targetWebcrmOrganisation.OrganisationName = sourceFortnoxOrganisation.Name;
            targetWebcrmOrganisation.OrganisationPostCode = sourceFortnoxOrganisation.ZipCode;
            targetWebcrmOrganisation.OrganisationCity = sourceFortnoxOrganisation.City;
            targetWebcrmOrganisation.OrganisationWww = sourceFortnoxOrganisation.WWW;
            targetWebcrmOrganisation.OrganisationTelephone = sourceFortnoxOrganisation.Phone1;

            targetWebcrmOrganisation.SetPropertyValue(organisationIdFieldName, sourceFortnoxOrganisation.CustomerNumber);
        }

        private void CopyOrganisationProperties(
            OrganisationDto sourceWebcrmOrganisation,
            Customer targetFortnoxOrganisation)
        {
            // Map webCRM organisation to Fortnox customer.
            targetFortnoxOrganisation.City = sourceWebcrmOrganisation.OrganisationCity;
            targetFortnoxOrganisation.Name = sourceWebcrmOrganisation.OrganisationName;
            targetFortnoxOrganisation.Phone1 = sourceWebcrmOrganisation.OrganisationTelephone;
            targetFortnoxOrganisation.WWW = sourceWebcrmOrganisation.OrganisationWww;
            targetFortnoxOrganisation.ZipCode = sourceWebcrmOrganisation.OrganisationPostCode;
        }
    }
}