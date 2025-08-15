using Core.Attributes;
using Core.Repository;
using Core.Serialization;
using Game.Inventory.Model;

namespace Game.Inventory.Repo
{
    [Repository]
    public class InventoryRepo : SingleEntityPrefsRepository<InventoryModel>
    {
        protected override string Key => "Inventory";
        
        public InventoryRepo(ISerializer deserializer) : base(deserializer)
        {
        }
    }
}