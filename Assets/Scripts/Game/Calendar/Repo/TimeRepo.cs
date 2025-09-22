using Core.Attributes;
using Core.Repository;
using Game.Calendar.Model;

namespace Game.Calendar.Repo
{
    [Repository]
    public class TimeRepo : SingleEntityMemoryRepository<TimeModel>
    {
    }
}