using System;
using System.ComponentModel.DataAnnotations;

namespace Webcrm.ErpIntegrations.Configurations.Models
{
    public class PowerofficeConfiguration : BaseConfiguration
    {
        public PowerofficeConfiguration()
        {
            AcceptedOrganisationStatuses = new string[0];
            AcceptedOrganisationTypes = new string[0];
        }

        /// <summary>The name of the field in webCRM that is used to store the customer number that PowerOffice generates. This is typically a number between 10.000 and 19.999. This is called the organisation number is the PowerOffice user interface.</summary>
        [Required]
        public string OrganisationCodeFieldName { get; set; }

        /// <summary>The name of the field in webCRM that is used to store the VAT number (CVR-nummer) of the corresponding organisation in the ERP system, e.g. "OrganisationExtraCustom1". Note that webCRM's property named OrganisationVatNumber is not used.</summary>
        public string OrganisationVatNumberFieldName { get; set; }

        /// <summary>The name of the field in webCRM that is used to store the ID of the corresponding person in the ERP system, e.g. "PersonCustom5".</summary>
        [Required]
        public string PersonIdFieldName { get; set; }

        /// <summary>A GUID used to authenticate towards the PowerOffice API.</summary>
        [Required]
        public Guid PowerofficeClientKey { get; set; }

        /// <summary>E.g. "OrganisationCustom1".</summary>
        public string PrimaryDeliveryAddressFieldName { get; set; }

        /// <summary>The name of the field in webCRM that is used to store the product code of the corresponding product in the ERP system, e.g. "QuotationLineLinkedDataItemData3".</summary>
        [Required]
        public string ProductCodeFieldName { get; set; }

        /// <summary>The name of the field in webCRM that is used to store the ID of the corresponding product in the ERP system, e.g. "QuotationLineLinkedDataItemData1".</summary>
        [Required]
        public string ProductIdFieldName { get; set; }

        /// <summary>The name of the field in webCRM that is used ot store the name of the corresponding product in the ERP system, e.g. "QuotationLineLinkedDataItemData2".</summary>
        [Required]
        public string ProductNameFieldName { get; set; }

        /// <summary>the name of the field in webCRM that is used to store the unit of the product ("kilo", "a piece", etc.), e.g. "QuotationLineLinkedDataItem4".</summary>
        public string ProductUnitFieldName { get; set; }
    }
}