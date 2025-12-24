using UnityEngine;

namespace Sain.Utils
{
    public class FlipbookMeshAnimation : MonoBehaviour
    {
        [SerializeField] private Mesh[] meshes;
        [SerializeField] private float flipbookRate = .2f;
        [SerializeField] private bool loop = true;

        private MeshFilter meshFilter;
        private float flipbookTime;
        private int index;

        void Awake()
        {
            meshFilter = GetComponent<MeshFilter>();
        }

        void Update()
        {
            flipbookTime += Time.deltaTime;

            if (flipbookTime > flipbookRate)
            {
                flipbookTime = 0;
                ChangeMesh();
            }
        }

        void ChangeMesh()
        {
            if (loop)
            {
                index = (index + 1) % meshes.Length;
            }
            else
            {
                if (index >= meshes.Length - 1) return;
                index++;
            }

            meshFilter.mesh = meshes[index];
        }
    }
}
