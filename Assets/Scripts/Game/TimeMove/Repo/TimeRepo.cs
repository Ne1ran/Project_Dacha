using Core.Attributes;
using Core.Repository;
using Core.Serialization;
using Game.TimeMove.Model;

namespace Game.TimeMove.Repo
{
    [Repository]
    public class TimeRepo : SingleEntityPrefsRepository<TimeModel>
    {
        protected override string Key => "Time";

        public TimeRepo(ISerializer deserializer) : base(deserializer)
        {
        }
    }
}