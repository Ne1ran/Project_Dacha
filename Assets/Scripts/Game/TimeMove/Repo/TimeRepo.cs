using Core.Attributes;
using Core.Repository;
using Game.TimeMove.Model;

namespace Game.TimeMove.Repo
{
    [Repository]
    public class TimeRepo : SingleEntityMemoryRepository<TimeModel>
    {
    }
}