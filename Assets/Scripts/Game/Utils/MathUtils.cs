using UnityEngine;

namespace Game.Utils
{
    public static class MathUtils
    {
        public static Vector3 RotateAroundX(Vector3 localPoint, float angle)
        {
            float angleRadians = Mathf.Deg2Rad * angle;
            float y = localPoint.y * Mathf.Cos(angleRadians) - localPoint.z * Mathf.Sin(angleRadians);
            float z = localPoint.y * Mathf.Sin(angleRadians) + localPoint.z * Mathf.Cos(angleRadians);
            return new(localPoint.x, y, z);
        }

        public static Vector3 RotateAroundY(Vector3 center, Vector3 localPoint, float angle)
        {
            return center + RotateAroundY(localPoint, angle);
        }

        public static Vector3 RotateAroundY(Vector3 worldPoint, float angle)
        {
            float angleRadians = Mathf.Deg2Rad * angle;
            float x = worldPoint.x * Mathf.Cos(angleRadians) - worldPoint.z * Mathf.Sin(angleRadians);
            float z = worldPoint.x * Mathf.Sin(angleRadians) + worldPoint.z * Mathf.Cos(angleRadians);
            return new(x, worldPoint.y, z);
        }

        public static Vector3 RotateAroundZ(Vector3 localPoint, float angle)
        {
            float angleRadians = Mathf.Deg2Rad * angle;
            float x = localPoint.x * Mathf.Cos(angleRadians) - localPoint.y * Mathf.Sin(angleRadians);
            float y = localPoint.x * Mathf.Sin(angleRadians) + localPoint.y * Mathf.Cos(angleRadians);
            return new(x, y, localPoint.z);
        }

        public static Vector3 RotatePoint(Vector3 center, Vector3 localPoint, Vector3 eulerAngles)
        {
            Vector3 aroundX = RotateAroundX(localPoint, eulerAngles.x);
            Vector3 aroundY = RotateAroundY(aroundX, eulerAngles.y);
            Vector3 aroundZ = RotateAroundZ(aroundY, eulerAngles.z);
            return center + aroundZ;
        }

        public static float PercentFrom(float sourceValue, float percent)
        {
            return sourceValue * (percent / 100);
        }

        public static float CalculateResist(float sourceValue, float resist)
        {
            return sourceValue * (1 - resist);
        }

        public static bool IsFuzzyEquals(float a, float b, float fuzzy = 0.0005f)
        {
            return Mathf.Abs(a - b) <= fuzzy;
        }

        public static bool IsFuzzyEquals(Vector3 a, Vector3 b, float fuzzy = 0.0005f)
        {
            return IsFuzzyEquals(a.x, b.x, fuzzy) && IsFuzzyEquals(a.y, b.y, fuzzy) && IsFuzzyEquals(a.z, b.z, fuzzy);
        }
        
        public static bool IsFuzzyClose2D(Vector3 a, Vector3 b, float fuzzy = 0.0005f)
        {
            return Distance2D(a, b) <= fuzzy;
        }

        public static float Bezier(float start, float middle, float end, float t)
        {
            if (t <= 0) {
                return start;
            }

            if (t >= 1) {
                return end;
            }

            float result = start * ((1 - t) * (1 - t)) + middle * 2 * t * (1 - t) + end * (t * t);
            return result;
        }

        public static Vector3 Direction(Vector3 from, Vector3 to)
        {
            return (to - from).normalized;
        }

        public static Vector3 Direction2D(Vector3 from, Vector3 to)
        {
            Vector3 direction = Direction(from, to);
            direction.y = 0;
            return direction;
        }

        /// <summary>
        /// Возвращает угол в градусах по системе координат X и Y. Рассчет градусов идет справа против часовой стрелки.
        /// </summary>
        public static float Angle2D(Vector3 from, Vector3 to)
        {
            return Angle2D(new(from.x, from.z), new Vector2(to.x, to.z)) * Mathf.Rad2Deg;
        }

        public static float Angle2D(Vector2 p1, Vector2 p2)
        {
            float oa = p2.x - p1.x;
            float ab = p2.y - p1.y;
            float ob = Mathf.Sqrt(oa * oa + ab * ab);

            float sin = ab / ob;
            float cos = oa / ob;

            float result = sin > 0 ? Mathf.Acos(cos) : -Mathf.Acos(cos);
            if (result < 0) {
                result += 2 * Mathf.PI;
            }

            return result;
        }

        public static float SqrDistance2D(Vector3 a, Vector3 b)
        {
            float leg1 = a.x - b.x;
            float leg2 = a.z - b.z;
            return leg1 * leg1 + leg2 * leg2;
        }
        
        public static Vector3 GetVector2D(Vector3 vector)
        {
            return new(vector.x, 0, vector.z);
        }
        
        public static float Distance2D(Vector3 a, Vector3 b)
        {
            return Mathf.Sqrt(SqrDistance2D(a, b));
        }

        public static Vector3 Reflect2D(Vector3 inDirection, Vector3 normal)
        {
            Vector2 result = Vector2.Reflect(new(inDirection.x, inDirection.z), new(normal.x, normal.z));
            return new(result.x, 0, result.y);
        }

        /// <summary>
        /// Возвращает разность между углами,
        /// то есть в какую сторону и на сколько нужно
        /// поворачивать чтобы достичь целевого поворота.
        /// </summary>
        /// <param name="radTargetAngle">целевой угол в радианах</param>
        /// <param name="radCurAngle">текущий угол в радианах</param>
        /// <returns>разноcть между углами в радианах</returns>
        public static float GetDeltaRadAngle(float radTargetAngle, float radCurAngle)
        {
            return Mathf.Atan2(Mathf.Sin(radTargetAngle - radCurAngle), Mathf.Cos(radTargetAngle - radCurAngle));
        }
    }
}