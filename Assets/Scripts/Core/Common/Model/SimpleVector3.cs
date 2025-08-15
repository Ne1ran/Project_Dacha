using UnityEngine;

namespace Core.Common.Model
{
    public readonly struct SimpleVector3
    {
        public float x { get; }
        public float y { get; }
        public float z { get; }

        public SimpleVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3 ToVector3()
        {
            return new(x, y, z);
        }
    }
}