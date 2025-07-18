using Core.Repository;
using Core.Serialization;
using Game.Inventory.Model;

namespace Game.Inventory.Repo
{
    public class InventoryRepo : SingleEntityPrefsRepository<InventoryModel>
    {
        protected override string Key => "Inventory";
        
        public InventoryRepo(ISerializer deserializer) : base(deserializer)
        {
        }
    }
}