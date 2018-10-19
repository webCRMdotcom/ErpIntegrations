namespace Webcrm.ErpIntegrations.Synchronisation.Fortnox.Models
{
    internal class FortnoxItemTypeAndId
    {
        public FortnoxItemTypeAndId(FortnoxItemType itemType, int itemId)
        {
            ItemType = itemType;
            ItemId = itemId;
        }

        public FortnoxItemType ItemType { get; }
        public int ItemId { get; }
    }
}