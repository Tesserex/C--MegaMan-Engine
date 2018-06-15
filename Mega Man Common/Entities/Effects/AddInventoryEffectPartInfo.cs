namespace MegaMan.Common.Entities.Effects
{
    public class AddInventoryEffectPartInfo : IEffectPartInfo
    {
        public string ItemName { get; set; }
        public int Quantity { get; set; }

        public IEffectPartInfo Clone()
        {
            return new AddInventoryEffectPartInfo {
                ItemName = ItemName,
                Quantity = Quantity
            };
        }
    }
}
