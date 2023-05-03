using System;
using System.Collections.Generic;
using System.Linq;
using GeometRi;
using Illumination.Entities.Basic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;

namespace Utils
{
    [Serializable]
    public class PolygonByRectangle
    {
        [SerializeField] private Vector3 mPosition;
        [SerializeField] private Vector2 mSize;
        [SerializeField] private Vector3 mEulerRotation;

        public PolygonByRectangle(Vector3 mPosition, Vector2 mSize, Vector3 mEulerRotation)
        {
            this.mPosition = mPosition;
            this.mSize = mSize;
            this.mEulerRotation = mEulerRotation;
        }
        
        public Vector3 MPosition => mPosition;
        public Vector2 MSize => mSize;
        public Vector3 MEulerRotation => mEulerRotation;
    }
    
    public static class PolygonInitHelper
    {
        public static List<Polygon> CreatePolygons(int randomAmount)
        {
            var rectangles = Enumerable.Range(0, randomAmount)
                .Select(x => new PolygonByRectangle(
                    new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), Random.Range(-10f, 10f)),
                    new Vector2(Random.Range(0.5f, 3f), Random.Range(0.5f, 3f)),
                    new Vector3(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f))));

            return CreatePolygons(rectangles);
        }
        
        
        public static List<Polygon> CreatePolygons(IEnumerable<PolygonByRectangle> rectangles) =>
            rectangles
                .Select(r => (
                    r,
                    p: new Polygon(new List<Point3d>()
                    {
                        new(-r.MSize.x / 2, 0, -r.MSize.y / 2),
                        new(-r.MSize.x / 2, 0, r.MSize.y / 2),
                        new(r.MSize.x / 2, 0, r.MSize.y / 2),
                        new(r.MSize.x / 2, 0, -r.MSize.y / 2),
                    })
                ))
                .Select(x =>
                {
                    var (r, p) = x;
                    var q = Quaternion.Euler(r.MEulerRotation.x, r.MEulerRotation.y, r.MEulerRotation.z);
                    return p.TranslateAndRotate(new Vector3d(r.MPosition.x, r.MPosition.y, r.MPosition.z),
                        new Rotation(new GeometRi.Quaternion(q.w, q.x, q.y, q.z)),
                        new Point3d(0, 0, 0));
                })
                .ToList();
    }
}