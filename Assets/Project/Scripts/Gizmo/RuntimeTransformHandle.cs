using UnityEngine;

public class RuntimeTransformHandle : MonoBehaviour
{
    public Transform target;

    [Header("Axes")]
    public Transform xAxis;
    public Transform yAxis;
    public Transform zAxis;

    [Header("Visual")]
    public float gizmoScale = 0.15f;

    private Camera cam;

    private bool dragging;
    private Transform activeAxis;

    private Plane dragPlane;

    private Vector3 axisDirection;

    private Vector3 startObjectPosition;
    private Vector3 startHitPoint;

    void Start()
    {
        cam = Camera.main;
        Hide();
    }

    void Update()
    {
        if (target == null)
            return;

        transform.position = target.position;

        // Ï‡Ò¯Ú‡· ÓÚ Í‡ÏÂ˚
        float dist =
            Vector3.Distance(
                cam.transform.position,
                target.position
            );

        transform.localScale =
            Vector3.one * dist * gizmoScale;

        HandleInput();
    }

    void HandleInput()
    {
        // Õ¿∆¿“»≈
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray =
                cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (
                    hit.transform == xAxis ||
                    hit.transform == yAxis ||
                    hit.transform == zAxis
                )
                {
                    dragging = true;
                    activeAxis = hit.transform;

                    axisDirection =
                        GetAxisDirection(activeAxis);

                    startObjectPosition =
                        target.position;

                    dragPlane =
                        new Plane(
                            -cam.transform.forward,
                            target.position
                        );

                    if (dragPlane.Raycast(ray, out float enter))
                    {
                        startHitPoint =
                            ray.GetPoint(enter);
                    }
                }
            }
        }

        // ƒ¬»∆≈Õ»≈
        if (dragging && Input.GetMouseButton(0))
        {
            Ray ray =
                cam.ScreenPointToRay(Input.mousePosition);

            if (dragPlane.Raycast(ray, out float enter))
            {
                Vector3 currentHit =
                    ray.GetPoint(enter);

                Vector3 delta =
                    currentHit - startHitPoint;

                float moveAmount =
                    Vector3.Dot(delta, axisDirection);

                target.position =
                    startObjectPosition +
                    axisDirection * moveAmount;
            }
        }

        // Œ“œ”— ¿Õ»≈
        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
            activeAxis = null;
        }
    }

    Vector3 GetAxisDirection(Transform axis)
    {
        if (axis == xAxis)
            return Vector3.right;

        if (axis == yAxis)
            return Vector3.up;

        return Vector3.forward;
    }

    public void Show(Transform newTarget)
    {
        target = newTarget;

        transform.position =
            target.position;

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        target = null;
        gameObject.SetActive(false);
    }
}