using Core.Repository;
using Core.Serialization;
using Game.TimeMove.Model;
using JetBrains.Annotations;

namespace Game.TimeMove.Repo
{
    [UsedImplicitly]
    public class TimeRepo : SingleEntityPrefsRepository<TimeModel>
    {
        protected override string Key => "Time";

        public TimeRepo(ISerializer deserializer) : base(deserializer)
        {
        }
    }
}