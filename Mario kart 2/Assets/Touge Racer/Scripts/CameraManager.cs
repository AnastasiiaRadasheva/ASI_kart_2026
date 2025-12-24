using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Sain.TougeRacer
{
    public class CameraManager : MonoBehaviour
    {
        private CameraDatabase cameraDatabase;
        private CinemachineCamera[] cameras;
        private CinemachineCamera finishCam;
        private CinemachineCamera lookbackCam;
        private RaceManager raceManager;

        private int camIndex;

        public InputActionReference changeCamReference;
        public InputActionReference lookbackReference;
        private InputAction changeCamAction;
        private InputAction lookbackAction;

        void Start()
        {
            cameraDatabase = CameraDatabase.Instance;
            raceManager = RaceManager.Instance;
            cameras = new CinemachineCamera[cameraDatabase.cameraPrefabs.Length];
            for (int i = 0; i < cameraDatabase.cameraPrefabs.Length; i++)
            {
                cameras[i] = Instantiate(cameraDatabase.cameraPrefabs[i]);
                cameras[i].Target.TrackingTarget = raceManager.FocusCar.transform;
            }
            finishCam = Instantiate(cameraDatabase.finishCamera);
            finishCam.Target.TrackingTarget = raceManager.FocusCar.transform;
            finishCam.Priority = 0;

            lookbackCam = Instantiate(cameraDatabase.lookbackCamera);
            lookbackCam.Target.TrackingTarget = raceManager.FocusCar.transform;
            lookbackCam.Priority = 0;

            camIndex = GameManager.GetSelectedCameraIndex();

            if (changeCamReference == null) return;
            changeCamAction = changeCamReference.action;
            changeCamAction.Enable();

            if (lookbackReference == null) return;
            lookbackAction = lookbackReference.action;
            lookbackAction.Enable();
        }

        void Update()
        {
            for (int i = 0; i < cameras.Length; i++)
            {
                if (i == camIndex)
                {
                    cameras[i].Priority = 10;
                }
                else
                {
                    cameras[i].Priority = 0;
                }
            }

            if (changeCamAction.WasPressedThisFrame())
            {
                ChangeCamera();
            }

            LookbackCam(lookbackAction.IsPressed());
        }

        void OnEnable()
        {
            RaceEvents.RaceFinishEvent.AddListener(FinishCam);
        }

        void OnDisable()
        {
            RaceEvents.RaceFinishEvent.RemoveListener(FinishCam);
        }

        private void ChangeCamera()
        {
            camIndex = (camIndex + 1) % cameraDatabase.cameraPrefabs.Length;
            GameManager.SetSelectedCameraIndex(camIndex);
        }

        private void FinishCam()
        {
            finishCam.Priority = 13;
        }

        private void LookbackCam(bool lookback)
        {
            lookbackCam.Priority = lookback ? 12 : 0;
        }
    }
}
