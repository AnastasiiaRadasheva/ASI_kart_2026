using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class CarRespawn : MonoBehaviour
{
    public string checkpointTag = "chek";

    public Transform lastCheckpoint;

    private Vector3 lastCheckpointPos;
    private Quaternion lastCheckpointRot;

    public Vector3 spawnOffset = new Vector3(0, 1.2f, 0);

    public float stuckSeconds = 5f;
    public float minSpeed = 0.5f;
    public float groundRayDistance = 1.2f;

    public InputAction respawnAction = new InputAction(
        name: "Respawn",
        type: InputActionType.Button
    );

    private Rigidbody rb;
    private float stuckTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        respawnAction.Enable();
    }

    void OnDisable()
    {
        respawnAction.Disable();
    }

    void Start()
    {
        if (lastCheckpoint == null)
        {
            lastCheckpointPos = transform.position;
            lastCheckpointRot = transform.rotation;
        }
        else
        {
            lastCheckpointPos = lastCheckpoint.position;
            lastCheckpointRot = lastCheckpoint.rotation;
        }
    }

    void Update()
    {
        if (respawnAction.WasPressedThisFrame())
            Respawn();

        float speed = rb.linearVelocity.magnitude;

        bool inAir = !Physics.Raycast(transform.position, Vector3.down, groundRayDistance);

        if (inAir && speed < minSpeed)
            stuckTimer += Time.deltaTime;
        else
            stuckTimer = 0f;

        if (stuckTimer >= stuckSeconds)
            Respawn();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(checkpointTag)) return;

        lastCheckpointPos = other.transform.position;

        lastchek chk = other.GetComponent<lastchek>();
        if (chk != null && chk.direction != null)
        {
            lastCheckpointRot = chk.direction.rotation;
        }
        else
        {
            lastCheckpointRot = transform.rotation;
        }

        lastCheckpoint = other.transform;
    }

    public void Respawn()
{
    rb.linearVelocity = Vector3.zero;
    rb.angularVelocity = Vector3.zero;

    transform.position = lastCheckpointPos + spawnOffset;

    stuckTimer = 0f;
}

}
