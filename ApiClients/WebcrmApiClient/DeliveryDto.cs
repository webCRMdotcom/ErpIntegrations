using System;
using Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models.Invoices;
using Webcrm.ErpIntegrations.Configurations.Models;
using Webcrm.ErpIntegrations.GeneralUtilities;

namespace Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient
{
    public partial class DeliveryDto
    {
        public DeliveryDto()
        { }

        public DeliveryDto(
            OutgoingInvoiceWithLines powerofficeDelivery,
            int webcrmOrganisationId,
            PowerofficeConfiguration configuration)
        {
            DeliveryAssignedTo = 0;
            DeliveryAssignedTo2 = 0;
            DeliveryResponsible = 0;

            Update(powerofficeDelivery, webcrmOrganisationId, configuration);
        }

        public Guid? GetPowerofficeDeliveryId(string deliveryIdFieldName)
        {
            string stringValue = this.GetPropertyValue(deliveryIdFieldName);
            if (!Guid.TryParse(stringValue, out Guid powerofficeDeliveryId))
            {
                return null;
            }

            return powerofficeDeliveryId;
        }

        public void SetPowerofficeDeliveryId(string deliveryIdFieldName, Guid powerofficeDeliveryId)
        {
            this.SetPropertyValue(deliveryIdFieldName, powerofficeDeliveryId.ToString());
        }

        public void Update(
            OutgoingInvoiceWithLines powerofficeDelivery,
            int webcrmOrganisationId,
            PowerofficeConfiguration configuration)
        {
            DeliveryErpSyncDateTime = DateTime.UtcNow;
            DeliveryNumber = GetDeliveryNumber(powerofficeDelivery);
            DeliveryOrderDate = powerofficeDelivery.OrderDate;
            DeliveryOrganisationId = webcrmOrganisationId;
            DeliveryStatus = ToWebcrmStatus(powerofficeDelivery.Status);

            SetPowerofficeDeliveryId(configuration.DeliveryIdFieldName, powerofficeDelivery.Id);
        }

        private static string GetDeliveryNumber(OutgoingInvoiceWithLines powerofficeDelivery)
        {
            if (string.IsNullOrWhiteSpace(powerofficeDelivery.DocumentNo))
                return powerofficeDelivery.OrderNo.ToString();

            return powerofficeDelivery.DocumentNo;
        }

        private static DeliveryDtoDeliveryStatus ToWebcrmStatus(OutgoingInvoiceStatus powerofficeStatus)
        {
            switch (powerofficeStatus)
            {
                case OutgoingInvoiceStatus.Draft:
                    return DeliveryDtoDeliveryStatus.New;

                case OutgoingInvoiceStatus.Approved:
                    return DeliveryDtoDeliveryStatus.Planned;

                case OutgoingInvoiceStatus.Sent:
                    return DeliveryDtoDeliveryStatus.OnGoing;

                case OutgoingInvoiceStatus.Paid:
                    return DeliveryDtoDeliveryStatus.Completed;

                default:
                    throw new ArgumentOutOfRangeException(nameof(powerofficeStatus), powerofficeStatus, null);
            }
        }
    }
}