using System.ComponentModel.DataAnnotations;

namespace Webcrm.ErpIntegrations.ApiClients.PowerofficeApiClient.Models
{
    public class VatCode
    {
        [Key]
        public long Id { get; set; }

        public string Code { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public decimal Rate { get; set; }
    }
}