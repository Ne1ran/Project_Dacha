using Core.Attributes;
using Core.Repository;
using Game.Items.Model;

namespace Game.Equipment.Repo
{
    [Repository]
    public class EquipmentRepo : SingleEntityMemoryRepository<ItemModel> { }
}