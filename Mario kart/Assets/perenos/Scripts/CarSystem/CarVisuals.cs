using UnityEngine;

namespace Sain.TougeRacer
{
    [AddComponentMenu("Arcade Car/Car Visuals")]
    public class CarVisuals : MonoBehaviour
    {
        [Header("Body Paint")]
        [SerializeField] private MeshRenderer[] meshRenderers;
        [SerializeField] private Material material;

        [Header("Auto Setup")]
        [SerializeField] private bool autoFindRenderers = true;
        [SerializeField] private Transform searchRoot; // если пусто — будет искать в детях этого объекта
        [SerializeField] private bool includeInactive = true;

        private void Awake()
        {
            if (autoFindRenderers && (meshRenderers == null || meshRenderers.Length == 0))
            {
                var root = searchRoot != null ? searchRoot : transform;
                meshRenderers = root.GetComponentsInChildren<MeshRenderer>(includeInactive);
            }

            SetMaterial(material);
        }

        public void SetMaterial(Material mat)
        {
            material = mat;

            if (material == null) return;
            if (meshRenderers == null || meshRenderers.Length == 0) return;

            for (int i = 0; i < meshRenderers.Length; i++)
            {
                var mesh = meshRenderers[i];
                if (mesh == null) continue;
                mesh.material = material;
            }
        }

#if UNITY_EDITOR
        // Удобно: кнопка автозаполнения в инспекторе через контекстное меню
        [ContextMenu("Auto Fill Mesh Renderers")]
        private void AutoFill()
        {
            var root = searchRoot != null ? searchRoot : transform;
            meshRenderers = root.GetComponentsInChildren<MeshRenderer>(true);
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
