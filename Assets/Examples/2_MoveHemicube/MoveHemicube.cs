using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Examples._2_MoveHemicube
{
    public class MoveHemicube : MonoBehaviour
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


        private void Start()
        {
            var reference = new Illumination.Entities.Hemicube.Hemicube(cellsByHorizontal: cellsByHorizontal, cellsByVertical: cellsByVertical);
            
            var polygons = useRandom
                ? PolygonInitHelper.CreatePolygons(randomAmount)
                : PolygonInitHelper.CreatePolygons(polygonsToCreate);

            for (var i = 0; i < polygons.Count; i++)
            {
                var polygon = polygons[i];
                PolygonCreation.CreateMesh(transform, $"polygon_{i}", polygon, drawPolygonTwoSided);

                var hemicube = reference.Copy(polygon.Center.ToVector, polygon.Normal);
                HemicubeUtils.HemicubeUtils.CreateHemicube(hemicube, transform, cellsByHorizontal, cellsByVertical, showDff,
                    drawHemicubesTwoSided);
            }
        }
    }
}