using System.Collections.Generic;
using UnityEngine;

namespace Game.Utils
{
    public static class PhysicsUtils
    {
        public static IReadOnlyList<RaycastHit> RaycastAround(Vector3 center,
                                                              Vector3 startDirection,
                                                              int raycastCount,
                                                              float maxDistance,
                                                              int layerMask)
        {
            List<RaycastHit> result = new(raycastCount);

            int anglePerCast = 360 / raycastCount;
            Vector3 castDirection = startDirection;

            int currentAngle = 0;

            for (int i = 0; i < raycastCount; i++) {
                if (Physics.Raycast(center, castDirection, out RaycastHit hitInfo, maxDistance, layerMask)) {
                    result.Add(hitInfo);
                }

                currentAngle += anglePerCast;
                castDirection = MathUtils.RotateAroundY(Vector3.zero, castDirection, currentAngle);
            }

            return result;
        }
    }
}