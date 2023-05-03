using System.Collections.Generic;
using System.Linq;
using Illumination.Entities.Basic;
using Illumination.Entities.RealObjects;
using Illumination.Services;
using Illumination.Util;
using UnityEngine;
using Utils;
using Material = Illumination.Entities.RealObjects.Material;
using Mesh = Illumination.Entities.RealObjects.Mesh;

namespace Examples._3_ConicProjection
{
    public class ConicProjection : MonoBehaviour
    {
        [SerializeField] private int cellsByHorizontal = 8;
        [SerializeField] private int cellsByVertical = 4;
        [SerializeField] private bool showDff = true;
        [SerializeField] private bool drawPolygonTwoSided = true;
        [SerializeField] private bool drawHemicubesTwoSided = true;

        [Space(20)] [SerializeField] private bool useRandom = true;

        [Header(" ===== NOT RANDOM ===== ")] [SerializeField]
        private List<PolygonByRectangle> polygonsToCreate;

        [Header(" ===== RANDOM ===== ")] [SerializeField]
        private int randomAmount = 10;

        void Start()
        {
            var reference = new Illumination.Entities.Hemicube.Hemicube(cellsByHorizontal: cellsByHorizontal,
                cellsByVertical: cellsByVertical);

            var polygons = new List<Polygon>();
            if (useRandom)
            {
                for (var i = 0; i < randomAmount; i++)
                {
                    while (true)
                    {
                        var polys = PolygonInitHelper.CreatePolygons(2);
                        if (!polys[0].IsVisibleFrom(polys[1])) continue;
                        if (polys[0].GetDistance(polys[1]) > 3d) continue;
                        polygons.AddRange(polys);
                        break;
                    }
                }
            }
            else
            {
                polygons = PolygonInitHelper.CreatePolygons(polygonsToCreate);
            }


            for (var pairI = 0; pairI < polygons.Count / 2; pairI++)
            {
                var parent = new GameObject($"Pair_{pairI}").transform;
                parent.parent = transform;

                var polygon1 = polygons[pairI * 2];
                var polygon2 = polygons[pairI * 2 + 1];
                PolygonCreation.CreateMesh(parent, $"polygon_{pairI * 2}", polygon1, drawPolygonTwoSided);
                PolygonCreation.CreateMesh(parent, $"polygon_{pairI * 2 + 1}", polygon2, drawPolygonTwoSided);

                var center = polygon1.Center;

                //hemicube for 1st polygon
                var hemicube = reference.Copy(center.ToVector, polygon1.Normal);
                HemicubeUtils.HemicubeUtils.CreateHemicube(hemicube, parent, cellsByHorizontal, cellsByVertical,
                    showDff,
                    drawHemicubesTwoSided);

                for (var fI = 0; fI < hemicube.Faces.Count; fI++)
                {
                    var face = hemicube.Faces[fI];
                    var projection = polygon2.ConicProjection(center, face.Polygon.Plane3d);
                    if (projection == null)
                    {
                        Debug.Log($"[Pair {pairI}] Projection not found for face {fI}");
                        continue;
                    }

                    //draw projection
                    PolygonCreation.CreateMesh(parent, $"projection_{fI}", projection, true);

                    //line center-proj-poly or center-poly-proj(if poly is inside of hemicube)
                    for (var pI = 0; pI < projection.Vertices.Count; pI++)
                    {
                        var v = projection.Vertices[pI];
                        Debug.DrawLine(center.ToUnity(), v.ToUnity(), ColorHelper.Random(), 60 * 60); //center-proj
                        Debug.DrawLine(center.ToUnity(), polygon2.Vertices[pI].ToUnity(), ColorHelper.Random(),
                            60 * 60); //proj-polygonVertex
                    }
                }

                var mesh1 = new Mesh(new[] { polygon1.Vertices }, new Material());
                var mesh2 = new Mesh(new[] { polygon2.Vertices }, new Material());
                var ffs = mesh1.Patches[0].CalculateFormFactors(reference,
                    new[] { mesh2.Patches[0] });
                Debug.LogWarning($"[Pair {pairI}] Form-factor = {ffs.Values.First()}");
            }
        }
    }
}