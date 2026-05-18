using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float boostMultiplier = 4f;

    [Header("Mouse")]
    public float mouseSensitivity = 2f;

    [Header("Speed")]
    public float speedChange = 10f;
    public float minSpeed = 1f;
    public float maxSpeed = 100f;

    private float yaw;
    private float pitch;
    private bool locker = false;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;

        yaw = angles.y;
        pitch = angles.x;

        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        Look();
        Move();
        ChangeSpeed();

        if (Input.GetMouseButtonDown(1))
            locker = !locker;
            
        if (locker)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }

    void Look()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;

        pitch = Mathf.Clamp(pitch, -89f, 89f);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    void Move()
    {
        Vector3 dir = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            dir += transform.forward;

        if (Input.GetKey(KeyCode.S))
            dir -= transform.forward;

        if (Input.GetKey(KeyCode.A))
            dir -= transform.right;

        if (Input.GetKey(KeyCode.D))
            dir += transform.right;

        if (Input.GetKey(KeyCode.E))
            dir += transform.up;

        if (Input.GetKey(KeyCode.Q))
            dir -= transform.up;

        float currentSpeed = moveSpeed;

        if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed *= boostMultiplier;

        transform.position +=
            dir.normalized *
            currentSpeed *
            Time.deltaTime;
    }

    void ChangeSpeed()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        moveSpeed += scroll * speedChange;

        moveSpeed = Mathf.Clamp(
            moveSpeed,
            minSpeed,
            maxSpeed
        );
    }
}