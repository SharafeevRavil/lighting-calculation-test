using UnityEngine;

namespace Examples._1_Hemicube
{
    public class CreateHemicube : MonoBehaviour
    {
        [SerializeField] private int cellsByHorizontal = 8;
        [SerializeField] private int cellsByVertical = 4;
        [SerializeField] private bool showDff = true;

        private void Start()
        {
            var hemicube = new Illumination.Entities.Hemicube.Hemicube(cellsByHorizontal: cellsByHorizontal, cellsByVertical: cellsByVertical);
            HemicubeUtils.HemicubeUtils.CreateHemicube(hemicube, transform, cellsByHorizontal, cellsByVertical, showDff);
        }
    }
}