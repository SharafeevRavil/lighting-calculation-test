using Illumination.Entities.Hemicube;
using UnityEngine;
using Utils;

namespace HemicubeUtils
{
    public static class HemicubeUtils
    {
        public static void CreateHemicube(Hemicube hemicube, Transform transform, int cellsByHorizontal, int cellsByVertical, bool showDff, bool drawTwoSided = false)
        {
            var dfSum = 0d;
            for (var fi = 0; fi < hemicube.Faces.Count; fi++)
            {
                var dfFace = 0d;

                var face = hemicube.Faces[fi];
                var faceGo = new GameObject($"Face_{fi}");
                faceGo.transform.parent = transform;
                for (var ci = 0; ci < face.Cells.Count; ci++)
                {
                    var cell = face.Cells[ci];
                    dfFace += cell.DeltaFf;

                    //mesh
                    var mesh = PolygonCreation.CreateMesh(faceGo.transform,
                        $"{ci / (fi == 0 ? cellsByHorizontal : cellsByVertical)}, {ci % (fi == 0 ? cellsByHorizontal : cellsByVertical)}", cell.Polygon, drawTwoSided);
                    //text
                    if (showDff)
                        ObjectsCreation.CreateText(cell.Polygon, $"Dff = {cell.DeltaFf:0.000000}", mesh.transform);
                }

                Debug.Log($"Face#{fi} sum dff = {dfFace}");
                dfSum += dfFace;
            }

            Debug.Log($"Hemicube sum dff = {dfSum}");
        }
    }
}