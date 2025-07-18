using Core.Repository;
using Core.Serialization;
using Game.Inventory.Model;
using JetBrains.Annotations;

namespace Game.Inventory.Repo
{
    [UsedImplicitly]
    public class InventoryRepo : SingleEntityPrefsRepository<InventoryModel>
    {
        protected override string Key => "Inventory";
        
        public InventoryRepo(ISerializer deserializer) : base(deserializer)
        {
        }
    }
}