using UnityEngine;

namespace Sain.TougeRacer
{
    [AddComponentMenu("Arcade Car/Car Light")]
    public class CarLight : MonoBehaviour
    {
        Renderer rend;

        public bool on;
        public Light[] targetLights;
        public Material onMaterial;
        Material offMaterial;
        public Transform popupLight;
        public float popupOnAngle;
        public float popupOffAngle;
        float popupAngle;

        void Start()
        {
            rend = GetComponent<Renderer>();
            if (rend)
            {
                offMaterial = rend.sharedMaterial;
            }
        }

        void Update()
        {
            foreach (var light in targetLights)
            {
                light.enabled = on;
            }
            if (rend) rend.sharedMaterial = on ? onMaterial : offMaterial;
            if (popupLight)
            {
                popupAngle = Mathf.Lerp(popupAngle, on ? popupOnAngle : popupOffAngle, Time.deltaTime * 10f);
                popupLight.localRotation = Quaternion.Euler(popupAngle, popupLight.localEulerAngles.y, popupLight.localEulerAngles.z);
            }
        }
    } 
}
