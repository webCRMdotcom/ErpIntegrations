namespace Webcrm.ErpIntegrations.Configurations
{
    public static class Constants
    {
        /// <summary>The value that a custom checkbox field has in the webCRM API, when it is checked.</summary>
        public const string CustomCheckboxFieldCheckedValue = "Yes";
        /// <summary>We cannot (yet?) retrieve the fallback sales account through the PowerOffice API. Most PowerOffice systems use 3000 as the default sales account, so for now we simply default to that.</summary>
        public const long DefaultPowerofficeSalesAccount = 3000;
        public const string FortnoxQueueName = "fortnox";
        public const string PowerofficeQueueName = "poweroffice";
    }
}