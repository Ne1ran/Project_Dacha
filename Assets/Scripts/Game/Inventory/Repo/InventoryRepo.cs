using Core.Attributes;
using Core.Repository;
using Game.Inventory.Model;

namespace Game.Inventory.Repo
{
    [Repository]
    public class InventoryRepo : SingleEntityMemoryRepository<InventoryModel>
    {
       
    }
}