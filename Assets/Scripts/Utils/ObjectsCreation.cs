using System;
using System.Linq;
using GeometRi;
using Illumination.Entities;
using Illumination.Entities.Basic;
using TMPro;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

namespace Utils
{
    public static class ObjectsCreation
    {
        public static GameObject CreateText(string text, Transform parent, Vector3 position, Vector2 size, Quaternion rotation)
        {
            var go = new GameObject(text)
            {
                transform =
                {
                    parent = parent,
                    position = position,
                    rotation = rotation
                }
            };

            var tmp = go.AddComponent<TextMeshPro>();
            tmp.text = text;
            tmp.color = Color.black;
            tmp.verticalAlignment = VerticalAlignmentOptions.Middle;
            tmp.enableAutoSizing = true;
            tmp.fontSizeMin = 0.1f;

            var rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = size;

            return go;
        }

        public static GameObject CreateText(Polygon textPolygon, string text, Transform textParent)
        {
            var normal = textPolygon.Plane3d.Normal;
            //text position
            var position = textPolygon.Center.ToUnity();
            position += normal.ToUnity().normalized * 0.005f;
            //text size
            var verticesDiff = textPolygon.Vertices.Select(v => v - textPolygon.Center).ToList();
            //var maxCoord = verticesDiff.Select(v => Math.Max(Math.Max(v.X, v.Y), v.Z)).Max();
            //var minCoord = verticesDiff.Select(v => Math.Min(Math.Min(v.X, v.Y), v.Z)).Min();
            //var size = new Vector2((float)(maxCoord - minCoord), (float)(maxCoord - minCoord));
            var minCoordAbs = verticesDiff
                .SelectMany(v => new []{Math.Abs(v.X), Math.Abs(v.Y), Math.Abs(v.Z)})
                .Where(v => v > double.Epsilon).Min();
            var size = new Vector2((float)Math.Abs(minCoordAbs * 2), (float)Math.Abs(minCoordAbs * 2));
            //text rotation
            var defaultRotVector = new Vector3d(0, 0, -1);
            var angle = defaultRotVector.AngleTo(normal);
            var rotation = Quaternion.identity;

            if (angle > 10e-6)
            {
                var rotAxis = Math.PI - angle <= 10e-6
                    ? normal.OrthogonalVector
                    : new Plane3d(new Point3d(0, 0, 0), defaultRotVector, normal).Normal;
                var rot = new Rotation(rotAxis, angle);
                rotation = rot.ToQuaternion.ToUnity();
            }

            return CreateText(text, textParent, position, size, rotation);
        }
    }
}