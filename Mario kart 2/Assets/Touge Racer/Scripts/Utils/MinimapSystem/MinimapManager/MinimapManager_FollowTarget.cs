using UnityEngine;

namespace Sain.Utils
{
    [AddComponentMenu("Minimap/Minimap Manager/Minimap Manager (Follow Target)")]
    public class MinimapManager_FollowTarget : MinimapManager
    {
        [SerializeField] Transform targetToFollow;
        [SerializeField] Transform targetToRotate;
        [SerializeField] bool rotateMap = false;

        [Header("Center Positions")]
        [Tooltip("The center position of the minimap on the canvas (Parent's position is used as the center).")]
        Vector3 minimapCenterPos;

        [Tooltip("The center position of the game world (This (Minimap Manager) gameObject's position is used as the center).")]
        Vector3 worldCenterPos;

        private Quaternion initialRotation;
        private float xPivot;
        private float yPivot;


        protected override void Awake()
        {
            base.Awake();
            if (minimapImage == null) return;
            initialRotation = minimapImage.rectTransform.rotation;
        }

        // Override the FixedUpdate method from the base class.
        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            SetWorldCenterPosition();
            SetMinimapCenterPosition();

            FollowTarget();
        }

        // Set the target that the minimap will follow.
        public void SetTargetToFollow(Transform target)
        {
            targetToFollow = target;
        }

        private void SetWorldCenterPosition()
        {
            // Set the center of the world to the position of this gameObject.
            worldCenterPos = transform.position;
        }

        private void SetMinimapCenterPosition()
        {
            // Set the center of the minimap.
            minimapCenterPos = minimapImage.rectTransform.parent.position;
        }

        private void FollowTarget()
        {
            // Ensure there's a target to follow.
            if (targetToFollow == null) return;

            // Calculate the distance between the world center and the target's position.
            Vector3 DistanceOfWorldCenter = worldCenterPos - targetToFollow.position;

            // Calculate ratios based on the distances.
            float xRatio = DistanceOfWorldCenter.x / worldBounds.XTotalSize;
            float zRatio = DistanceOfWorldCenter.z / worldBounds.ZTotalSize;

            // Calculate shifts on the minimap based on the ratios.
            float xMinimapShift = minimapBounds.XTotalSize * xRatio;
            float yMinimapShift = minimapBounds.YTotalSize * zRatio;

            xPivot = (targetToFollow.position.x - worldBounds.MinX) / worldBounds.XTotalSize;
            yPivot = (targetToFollow.position.z - worldBounds.MinZ) / worldBounds.ZTotalSize;

            // Calculate the new position for the minimap image.
            Vector3 newMinimapPosition = new Vector3(minimapCenterPos.x + xMinimapShift, minimapCenterPos.y + yMinimapShift, 0);

            // Set the minimap image's position.
            // minimapImage.rectTransform.position = newMinimapPosition;
            minimapImage.rectTransform.pivot = new Vector2(xPivot, yPivot);

            if (rotateMap)
            {
                Transform rotateTarget = targetToRotate == null ? targetToFollow : targetToRotate;
                Quaternion mapRotation = Quaternion.Euler(initialRotation.x, initialRotation.y, initialRotation.z + rotateTarget.eulerAngles.y);
                minimapImage.rectTransform.rotation = mapRotation;
            }
        }
    }
}
