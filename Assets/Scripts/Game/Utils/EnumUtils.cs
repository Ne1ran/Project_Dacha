using System;

namespace Game.Utils
{
    public static class EnumUtils
    {
        public static string ToLowerString(this Enum item)
        {
            return item.ToString().ToLowerInvariant();
        }
    }
}