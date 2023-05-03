using System.Linq;
using Illumination.Entities;
using Illumination.Entities.Basic;
using UnityEngine;
using Mesh = UnityEngine.Mesh;

namespace Utils
{
    public static class PolygonCreation
    {
        public static GameObject CreateMesh(Transform parent, string goName, Polygon polygon, bool drawTwoSided = false, Color? color = null)
        {
            //Polygon is clockwise in left-handed coordinate system, Unity is left-handed as well 
            var go = new GameObject(goName);
            go.transform.parent = parent;

            var meshFilter = go.AddComponent<MeshFilter>();
            var meshRenderer = go.AddComponent<MeshRenderer>();

            //vertices are reused in this version. replaced it below
            /*var mesh = new Mesh
            {
                vertices = polygon.Vertices
                    .Select(x => x.ToUnity())
                    .ToArray(),
                triangles = polygon.Vertices
                    .Select((v, i) => i)
                    .Skip(2)
                    .SelectMany(i => drawTwoSided ? new[] { 0, i - 1, i, 0, i, i - 1 } : new[] { 0, i - 1, i })
                    .ToArray(),
            };*/

            var mesh = new Mesh()
            {
                vertices = polygon.Triangulation
                    .SelectMany(tri => new[] { tri.A.ToUnity(), tri.B.ToUnity(), tri.C.ToUnity() })
                    .ToArray(),
                triangles = polygon.Triangulation
                    .SelectMany((_, i) => drawTwoSided 
                        ? new[] { i * 3, i * 3 + 1, i * 3 + 2, i * 3, i * 3 + 2, i * 3 + 1 }
                        : new[] { i * 3, i * 3 + 1, i * 3 + 2 })
                    .ToArray()
            };

            meshFilter.mesh = mesh;
            meshRenderer.material = new Material(Shader.Find("Standard"))
            {
                color = color ?? ColorHelper.Random()
            };

            return go;
        }
    }
}