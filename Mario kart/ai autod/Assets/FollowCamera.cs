using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;              // Машинка
    public Vector3 offset = new Vector3(0f, 4f, -8f); // вверх/назад
    public float positionSmooth = 8f;     // плавность позиции
    public float rotationSmooth = 8f;     // плавность поворота
    public bool lookAtTarget = true;      // смотреть на машину

    void LateUpdate()
    {
        if (target == null) return;

        // offset относительно машины (чтобы камера была "сзади" по направлению машины)
        Vector3 desiredPos = target.TransformPoint(offset);

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPos,
            1f - Mathf.Exp(-positionSmooth * Time.deltaTime)
        );

        if (lookAtTarget)
        {
            Vector3 dir = (target.position - transform.position);
            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion desiredRot = Quaternion.LookRotation(dir.normalized, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    desiredRot,
                    1f - Mathf.Exp(-rotationSmooth * Time.deltaTime)
                );
            }
        }
        else
        {
            // либо следуем за поворотом машины
            Quaternion desiredRot = Quaternion.LookRotation(target.forward, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                desiredRot,
                1f - Mathf.Exp(-rotationSmooth * Time.deltaTime)
            );
        }
    }
}
