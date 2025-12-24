using Unity.Cinemachine;
using UnityEditor;
using UnityEngine;

namespace Sain.TougeRacer
{
    [CreateAssetMenu(fileName = "CameraDatabase", menuName = "Touge Racer/Camera Database")]
    public class CameraDatabase : ScriptableObject
    {
        private static CameraDatabase instance;
        public static CameraDatabase Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<CameraDatabase>("CameraDatabase");
#if UNITY_EDITOR
                    if (instance == null)
                    {
                        instance = CreateInstance<CameraDatabase>();
                        // Optionally save the new instance as an asset in the editor
                        AssetDatabase.CreateAsset(instance, "Assets/Resources/CameraDatabase.asset");
                    }
#endif
                }

                return instance;
            }
        }

        public int cameraIndex;

        public CinemachineCamera[] cameraPrefabs;
        public CinemachineCamera finishCamera;
        public CinemachineCamera lookbackCamera;

        public void SetCameraIndex(int cameraIndex)
        {
            this.cameraIndex = cameraIndex;
        }
    }
}
