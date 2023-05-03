using GeometRi;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

namespace Utils
{
    public static class GeometRiConverter
    {
        public static Vector3 ToUnity(this Point3d p) => new((float)p.X, (float)p.Y, (float)p.Z);
        public static Vector3 ToUnity(this Vector3d v) => new((float)v.X, (float)v.Y, (float)v.Z);
        public static Quaternion ToUnity(this GeometRi.Quaternion q) => new((float)q.X, (float)q.Y, (float)q.Z, (float)q.W);
    }
}