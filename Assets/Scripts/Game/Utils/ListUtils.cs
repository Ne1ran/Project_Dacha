using System.Collections.Generic;
using System.Linq;

namespace Game.Utils
{
    public static class ListUtils
    {
        public static List<List<T>> Split<T>(List<T> list, int sizePart)
        {
            return list.Select((x, y) => new {
                               Index = y,
                               Value = x
                       })
                       .GroupBy(x => x.Index / sizePart)
                       .Select(x => x.Select(y => y.Value).ToList())
                       .ToList();
        }
    }
}