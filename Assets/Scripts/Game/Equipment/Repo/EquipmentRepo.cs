using Core.Repository;
using Game.Items.Model;
using JetBrains.Annotations;

namespace Game.Equipment.Repo
{
    [UsedImplicitly]
    public class EquipmentRepo : SingleEntityMemoryRepository<ItemModel> { }
}