namespace MegaMan.Common.Entities.Effects
{
    public class RemoveInventoryEffectPartInfo : IEffectPartInfo
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }

        public IEffectPartInfo Clone()
        {
            return new RemoveInventoryEffectPartInfo {
                ItemName = ItemName,
                Quantity = Quantity
            };
        }
    }
}
