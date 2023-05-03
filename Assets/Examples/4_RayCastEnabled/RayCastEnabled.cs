using System.Collections.Generic;
using System.Linq;
using Illumination.Entities.Basic;
using Illumination.Services;
using Illumination.Util;
using UnityEngine;
using Utils;
using Material = Illumination.Entities.RealObjects.Material;
using Mesh = Illumination.Entities.RealObjects.Mesh;

namespace Examples._4_RayCastEnabled
{
    public class RayCastEnabled : MonoBehaviour
    {
        [SerializeField] private int cellsByHorizontal = 8;
        [SerializeField] private int cellsByVertical = 4;
        [SerializeField] private bool showDff = true;
        [SerializeField] private bool drawPolygonTwoSided = true;
        [SerializeField] private bool drawHemicubesTwoSided = true;


        [Space(10)] [SerializeField] private bool useRayCasting = true;
        
        [Space(10)] [SerializeField] private bool useRandom = true;

        [Header(" ===== NOT RANDOM ===== ")] [SerializeField]
        private List<PolygonByRectangle> polygonsToCreate;

        [Header(" ===== RANDOM ===== ")] [SerializeField]
        private int randomAmount = 10;

        void Start()
        {
            IlluminationConfig.UseRayCastBetweenPatchAndCell = useRayCasting;
            
            var reference = new Illumination.Entities.Hemicube.Hemicube(cellsByHorizontal: cellsByHorizontal, cellsByVertical: cellsByVertical);

            List<Polygon> polygons;
            if (useRandom)
            {
                polygons = PolygonInitHelper.CreatePolygons(1);
                for (var i = 0; i < randomAmount; i++)
                {
                    while (true)
                    {
                        var poly = PolygonInitHelper.CreatePolygons(1).First();
                        if (!poly.IsVisibleFrom(polygons[0])) continue;
                        if(poly.GetDistance(polygons[0]) > 4d) continue;
                        polygons.Add(poly);
                        break;
                    }
                }
            }
            else
            {
                polygons = PolygonInitHelper.CreatePolygons(polygonsToCreate);
            }


            var polygon1 = polygons[0];
            var center = polygon1.Center;
            //hemicube for 1st polygon
            var hemicube = reference.Copy(center.ToVector, polygon1.Normal);
            HemicubeUtils.HemicubeUtils.CreateHemicube(hemicube, transform, cellsByHorizontal, cellsByVertical, showDff,
                drawHemicubesTwoSided);

            for (var pI = 0; pI < polygons.Count; pI++)
            {
                PolygonCreation.CreateMesh(transform, $"polygon_{pI}", polygons[pI], drawPolygonTwoSided);
            }
            
            for (var pI = 1; pI < polygons.Count; pI++)
            {
                var polygon2 = polygons[pI];
                for (var fI = 0; fI < hemicube.Faces.Count; fI++)
                {
                    var face = hemicube.Faces[fI];
                    var projection = polygon2.ConicProjection(center, face.Polygon.Plane3d);
                    if (projection == null)
                    {
                        Debug.Log($"[Polygon {pI}] Projection not found for face {fI}");
                        continue;
                    }

                    //draw projection
                    PolygonCreation.CreateMesh(transform, $"projection_{fI}", projection, true);

                    //line center-proj-poly or center-poly-proj(if poly is inside of hemicube)
                    for (var prI = 0; prI < projection.Vertices.Count; prI++)
                    {
                        var v = projection.Vertices[prI];
                        Debug.DrawLine(center.ToUnity(), v.ToUnity(), ColorHelper.Random(), 60 * 60); //center-proj
                        Debug.DrawLine(center.ToUnity(), polygon2.Vertices[prI].ToUnity(), ColorHelper.Random(),
                            60 * 60); //proj-polygonVertex
                    }
                }

            }

            var mesh1 = new Mesh(new[] { polygon1.Vertices }, new Material());
            var otherPatches = polygons.Skip(1).Select(p => new Mesh(new[] {p.Vertices }, new Material()).Patches[0]).ToList();
            var ffs = mesh1.Patches[0].CalculateFormFactors(reference, otherPatches);
            for (var opI = 0; opI < otherPatches.Count; opI++)
            {
                var otherPatch = otherPatches[opI];
                Debug.LogWarning($"[Polygon {opI}] Form-factor = {ffs[otherPatch]}");
            }
        }
    }
}