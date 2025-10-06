using System.Collections.Generic;

namespace Game.Weather.Model
{
    public enum WindType
    {
        Calm = 0,
        Light = 1,
        Medium = 2,
        Strong = 3,
        Storm = 4
    }

    public static class WindTypeUtils
    {
        private static readonly Dictionary<WindType, float> _windTypes = new Dictionary<WindType, float>() {
                { WindType.Calm, 2f },
                { WindType.Light, 4f },
                { WindType.Medium, 6f },
                { WindType.Strong, 8f },
                { WindType.Storm, 10f }
        };

        public static WindType GetWindType(float value)
        {
            WindType resultType = WindType.Calm;
            
            foreach ((WindType type, float speed) in _windTypes) {
                if (value <= speed) {
                    break;
                }
                
                resultType = type;
            }

            return resultType;
        }
    }
}