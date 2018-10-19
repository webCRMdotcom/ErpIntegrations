using System;
using Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models;
using Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models.Invoices;
using Webcrm.ErpIntegrations.Configurations.Models;
using Webcrm.ErpIntegrations.GeneralUtilities;

namespace Webcrm.ErpIntegrations.ApiClients.WebcrmApiClient
{
    public partial class QuotationLineDto
    {
        public QuotationLineDto()
        { }

        public QuotationLineDto(
            OutgoingInvoiceLine powerofficeLine,
            Product powerofficeProduct,
            int webcrmDeliveryId,
            int webcrmOrganisationId,
            double vatPercentage,
            PowerofficeConfiguration configuration)
        {
            QuotationLineDiscount = Convert.ToDouble(powerofficeLine.DiscountPercent);
            QuotationLineMemo = powerofficeLine.Description;
            QuotationLineOpportunityId = webcrmDeliveryId;
            QuotationLineOrganisationId = webcrmOrganisationId;
            QuotationLinePrice = GetPrice(powerofficeLine, powerofficeProduct);
            QuotationLineQuantity = Convert.ToDouble(powerofficeLine.Quantity);
            QuotationLineSortOrder = powerofficeLine.SortOrder;
            QuotationLineVatPercentage = vatPercentage;

            SetPropertyValueUsingLinkedDataItemFieldName(configuration.ProductCodeFieldName, powerofficeLine.ProductCode);
            SetPropertyValueUsingLinkedDataItemFieldName(configuration.ProductNameFieldName, powerofficeProduct.Name);
            SetPropertyValueUsingLinkedDataItemFieldName(configuration.ProductIdFieldName, powerofficeProduct.Id.ToString());

            if (!string.IsNullOrWhiteSpace(configuration.ProductUnitFieldName))
            {
                string unit = string.IsNullOrWhiteSpace(powerofficeProduct.Unit)
                    ? QuotationLineLinkedDataItemDto.DataItemDefault
                    : powerofficeProduct.Unit;

                SetPropertyValueUsingLinkedDataItemFieldName(configuration.ProductUnitFieldName, unit);
            }
        }

        public string GetPowerofficeProductCode(string productCodeFieldName)
        {
            string propertyName = ConvertToQuotationLineFieldName(productCodeFieldName);
            return this.GetPropertyValue(propertyName);
        }

        private static double GetPrice(OutgoingInvoiceLine powerofficeLine, Product powerofficeProduct)
        {
            if (powerofficeLine.UnitPrice.HasValue)
                return Convert.ToDouble(powerofficeLine.UnitPrice.Value);

            return Convert.ToDouble(powerofficeProduct.SalesPrice);
        }

        /// <summary>Convert the field name from a QuotationLineLinkedDataItem to a QuotationLine before setting the value, e.g. from `QuotationLineLinkedDataItemData5` to `QuotationLineData5`.</summary>
        private void SetPropertyValueUsingLinkedDataItemFieldName(string quotationLineLinkedDataItemFieldName, string value)
        {
            string quotationLineFieldName = ConvertToQuotationLineFieldName(quotationLineLinkedDataItemFieldName);
            this.SetPropertyValue(quotationLineFieldName, value);
        }

        /// <summary>Delivery lines (QuotationLine) are based on product lines (QuotationLineLinkedDataItem), so the custom product properties that are named `QuotationLineLinkedDataItemDataX` are changed into `QuotationLineDataX` when accessed here on a QuotationLine.</summary>
        private static string ConvertToQuotationLineFieldName(string productFieldName)
        {
            return productFieldName.Replace("LinkedDataItem", "");
        }
    }
}