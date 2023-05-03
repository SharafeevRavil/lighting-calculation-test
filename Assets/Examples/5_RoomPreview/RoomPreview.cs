using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeometRi;
using Illumination.Entities.Basic;
using Illumination.Entities.RealObjects;
using Illumination.Services;
using Illumination.Util;
using UnityEngine;
using Utils;
using Material = Illumination.Entities.RealObjects.Material;
using Mesh = Illumination.Entities.RealObjects.Mesh;
using Space = Illumination.Entities.RealObjects.Space;

namespace Examples._5_RoomPreview
{
    [Serializable]
    public class LightSource
    {
        [SerializeField] public Vector2 position;
        [SerializeField] public double lumen;
    }

    public class RoomPreview : MonoBehaviour
    {
        [Header("Hemicube")]
        [SerializeField] private int cellsByHorizontal = 8;
        [SerializeField] private int cellsByVertical = 4;
        
        [Header("Draw settings")]
        [SerializeField] private bool drawPolygonTwoSided = true;
        
        [Header("Scene")]
        [SerializeField] private List<LightSource> lightSources = new();
        [SerializeField] private double wallsReflectionCoefficient = 0.3d;
        [SerializeField] private double cubesReflectionCoefficient = 0.4d;
        
        [Header("ReMeshing")]
        [SerializeField] private bool useReMesh = true;
        [SerializeField] private double reMeshMaxArea = 1;
        [SerializeField] private double reMeshMaxEdgeLength = 1;
        
        [Header("Hemicube")]
        [SerializeField] private bool useRayCasting = true;
        
        [Header("Radiosity")]
        [SerializeField] private int steps = 10;
        [SerializeField] private bool useAmbient = true;
        
        
        private Space CreateSpace()
        {
            var wallsMaterial = new Material(reflectionCoefficient: wallsReflectionCoefficient);
            var cubesMaterial = new Material(reflectionCoefficient: cubesReflectionCoefficient);

            var reMeshingConfig = useReMesh
                ? new ReMeshingConfig(reMeshMaxArea, reMeshMaxEdgeLength)
                : null;

            const double roomHeight = 3d;
            var meshes = new List<Mesh>
            {
                new(MeshCreation.CreateWalls(new Point3d(7, roomHeight, 7)), wallsMaterial, reMeshingConfig), //стены
                new(MeshCreation.CreateCube(new Point3d(1, 1, 1), new Point3d(2, 2, 0.5)), cubesMaterial,
                    reMeshingConfig),
                new(MeshCreation.CreateCube(new Point3d(-2, 0.5, 0), new Point3d(1, 1, 1)), cubesMaterial,
                    reMeshingConfig),
            };

            meshes.AddRange(lightSources
                .Select(ls =>
                {
                    var v = ls.position;
                    return new Mesh(new[]
                    {
                        new[]
                        {
                            new Point3d(v.x - 0.25, roomHeight - 0.01, v.y - 0.25),
                            new Point3d(v.x + 0.25, roomHeight - 0.01, v.y - 0.25),
                            new Point3d(v.x + 0.25, roomHeight - 0.01, v.y + 0.25),
                            new Point3d(v.x - 0.25, roomHeight - 0.01, v.y + 0.25)
                        }
                    }, new Material(ls.lumen, reflectionCoefficient: cubesReflectionCoefficient), reMeshingConfig);
                }));//свет
            return new Space(meshes);
        }

        private async void Start()
        {
            IlluminationConfig.UseRayCastBetweenPatchAndCell = useRayCasting;

            var space = CreateSpace();

            Debug.Log($"Space was created. Patches count: {space.Patches.Count}.");
            var time = Time.realtimeSinceStartup;
            var reference = new Illumination.Entities.Hemicube.Hemicube(cellsByHorizontal: cellsByHorizontal,
                cellsByVertical: cellsByVertical);
            Debug.Log($"Creating reference hemicube. {Time.realtimeSinceStartup - time} seconds.\n" +
                      $"Hemicube cells: {reference.Faces.Select(x => x.Cells.Count).Sum()}");
            
            time = Time.realtimeSinceStartup;
            await space.Initialize(reference);
            Debug.Log($"Calculating form-factors. {Time.realtimeSinceStartup - time} seconds.");

            time = Time.realtimeSinceStartup;
            var states = space.CalculateRadiosity(new RadiosityExitCondition(steps: steps), useAmbient);
            Debug.Log($"Calculating radiosity. {Time.realtimeSinceStartup - time} seconds.");

            for (var i = 0; i < states.Count; i++)
            {
                //sum
                {
                    // ReSharper disable once UseObjectOrCollectionInitializer
                    var newTr = new GameObject().transform;
                    newTr.parent = transform;

                    var sumState = states.Take(i + 1).ToList().SumRadiosity();

                    var gradient = GetGradient();
                    for (var pI = 0; pI < space.Patches.Count; pI++)
                    {
                        var patch = space.Patches[pI];
                        var state = sumState[patch];
                        var color = gradient.Evaluate(state.Emitted > 0
                            ? 1f
                            : Mathf.Clamp01((float)(state.FluxPerSquareUnit / 400f)));
                        PolygonCreation.CreateMesh(newTr, $"polygon_{pI}", patch, drawPolygonTwoSided, color);
                        ObjectsCreation.CreateText(patch, $"e={state.Emitted:0.000}\n" +
                                                          $"r={state.Reflected:0.000}\n" +
                                                          $"s={state.Stored:0.000}\n" +
                                                          $"lux={state.FluxPerSquareUnit:0.000}", newTr);
                    }

                    newTr.Translate(Vector3.forward * 10 * i);
                }
                //single
                {
                    // ReSharper disable once UseObjectOrCollectionInitializer
                    var newTr = new GameObject().transform;
                    newTr.parent = transform;

                    var sumState = new  List<Dictionary<Patch,PatchValues>>{states[i]}.SumRadiosity();

                    var gradient = GetGradient();
                    for (var pI = 0; pI < space.Patches.Count; pI++)
                    {
                        var patch = space.Patches[pI];
                        var state = sumState[patch];
                        var color = gradient.Evaluate(state.Emitted > 0
                            ? 1f
                            : Mathf.Clamp01((float)(state.FluxPerSquareUnit / 400f)));
                        PolygonCreation.CreateMesh(newTr, $"polygon_{pI}", patch, drawPolygonTwoSided, color);
                        ObjectsCreation.CreateText(patch, $"e={state.Emitted:0.000}\n" +
                                                          $"r={state.Reflected:0.000}\n" +
                                                          $"s={state.Stored:0.000}\n" +
                                                          $"lux={state.FluxPerSquareUnit:0.000}", newTr);
                    }

                    newTr.Translate(Vector3.forward * 10 * i);
                    newTr.Translate(Vector3.right * 10);
                }
            }


            /*var mesh1 = new Mesh(new[] { polygon1.Vertices }, new Material());
            var otherPatches = polygons.Skip(1).Select(p => new Mesh(new[] { p.Vertices }, new Material()).Patches[0])
                .ToList();
            var ffs = mesh1.Patches[0].CalculateFormFactors(reference, otherPatches);
            for (var opI = 0; opI < otherPatches.Count; opI++)
            {
                var otherPatch = otherPatches[opI];
                Debug.LogWarning($"[Polygon {opI}] Form-factor = {ffs[otherPatch]}");
            }*/
        }

        private Gradient GetGradient()
        {
            var gradient = new Gradient();

            // Populate the color keys at the relative time 0 and 1 (0 and 100%)
            var colorKey = new GradientColorKey[3];
            colorKey[0].color = Color.black;
            colorKey[0].time = 0.0f;
            colorKey[1].color = Color.white;
            colorKey[1].time = 0.5f;
            colorKey[2].color = Color.red;
            colorKey[2].time = 1f;

            var alphaKey = new GradientAlphaKey[2];
            alphaKey[0].alpha = 1.0f;
            alphaKey[0].time = 0.0f;
            alphaKey[1].alpha = 0.0f;
            alphaKey[1].time = 1.0f;

            gradient.SetKeys(colorKey, alphaKey);
            return gradient;
        }
    }
}