using System.Collections.Generic;
using Core.Attributes;
using Core.Repository;
using Game.Calendar.Model;

namespace Game.Calendar.Repo
{
    [Repository]
    public class CalendarRepo : MemoryRepository<int, List<CalendarDayWeather>>  {
        
    }
}