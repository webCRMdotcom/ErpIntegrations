using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Webcrm.ErpIntegrations.Configurations.Models
{
    public abstract class BaseConfiguration : Resource, IValidatableObject
    {
        protected BaseConfiguration()
        {
            AcceptedOrganisationStatuses = new string[0];
            AcceptedOrganisationTypes = new string[0];
            Disabled = false;
            LastSuccessfulCopyFromErpHeartbeat = DateTime.UtcNow;
            LastSuccessfulCopyToErpHeartbeat = DateTime.UtcNow;
            SynchroniseDeliveries = SynchroniseDeliveries.Off;
            SynchroniseProducts = false;
        }

        [Required]
        public IEnumerable<string> AcceptedOrganisationStatuses { get; set; }

        [Required]
        public IEnumerable<string> AcceptedOrganisationTypes { get; set; }

        /// <summary>The name of the field in webCRM that is used to store the ID of the corresponding delivery in the ERP system, e.g. "DeliveryCustom5".</summary>
        [Required]
        public string DeliveryIdFieldName { get; set; }

        /// <summary>Disabled looking for changes in this system. Any current messages on the queue will load.</summary>
        [Required]
        public bool Disabled { get; set; }

        [JsonIgnore]
        public string FirstAcceptedOrganisationStatus => AcceptedOrganisationStatuses?.FirstOrDefault();

        [JsonIgnore]
        public string FirstAcceptedOrganisationType => AcceptedOrganisationTypes?.FirstOrDefault();

        public DateTime LastSuccessfulCopyFromErpHeartbeat { get; set; }

        public DateTime LastSuccessfulCopyToErpHeartbeat { get; set; }

        /// <summary>The name of the field in webCRM that is used to store the ID (the internal system ID) of the corresponding organisation in the ERP system, e.g. "OrganisationExtraCustom8".</summary>
        [Required]
        public string OrganisationIdFieldName { get; set; }

        /// <summary>The name of the field in webCRM that is used to determine if a contact is the primary contact. Only primary contacts are synchronised with PowerOffice. The type of the field has to be set to Checkbox. Example: "PersonCustom4".</summary>
        [Required]
        public string PrimaryContactCheckboxFieldName { get; set; }

        /// <summary>Turn on synchronising deliveries in this system.</summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [Required]
        public SynchroniseDeliveries SynchroniseDeliveries { get; set; }

        /// <summary>Turn on synchronising products in this system.</summary>
        [Required]
        public bool SynchroniseProducts { get; set; }

        [Required]
        public string WebcrmApiKey { get; set; }

        [JsonProperty(PropertyName = "id")]
        [Required]
        public string WebcrmSystemId { get; set; }

        public bool Validate(out ICollection<ValidationResult> results)
        {
            var context = new ValidationContext(this, null, null);
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(this, context, results, true);
        }

        /// <remarks>This method is called by the .NET when validating the object.</remarks>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            bool statusDefined = AcceptedOrganisationStatuses.Any(status => !string.IsNullOrWhiteSpace(status));
            bool typeDefined = AcceptedOrganisationTypes.Any(type => !string.IsNullOrWhiteSpace(type));
            if (!statusDefined && !typeDefined)
            {
                var missingOrganisationStatusOrType = new ValidationResult(
                    "Either AcceptedOrganisationStatuses or AcceptedOrganisationTypes must contain at least one value",
                    new[] { nameof(AcceptedOrganisationStatuses), nameof(AcceptedOrganisationTypes) });

                return new[] { missingOrganisationStatusOrType };
            }

            return new List<ValidationResult>();
        }
    }
}