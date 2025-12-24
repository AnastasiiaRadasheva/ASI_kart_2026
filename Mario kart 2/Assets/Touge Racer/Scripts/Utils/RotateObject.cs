using UnityEngine;

namespace Sain.Utils
{
    public class RotateObject : MonoBehaviour
    {
        public Vector3 rotate;
        public float rotateSpeed = .2f;

        void Update()
        {
            transform.Rotate(rotate * rotateSpeed);
            transform.eulerAngles += rotateSpeed * Time.deltaTime * rotate;
        }
    }
}
