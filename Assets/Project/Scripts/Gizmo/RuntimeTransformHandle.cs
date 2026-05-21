using UnityEngine;

public class RuntimeTransformHandle : MonoBehaviour
{
    public Transform target;

    [Header("Move Axes")]
    public Transform xAxis;
    public Transform yAxis;
    public Transform zAxis;

    [Header("Rotate Rings")]
    public Transform xRing;
    public Transform yRing;
    public Transform zRing;

    [Header("Visual")]
    public float gizmoScale = 0.15f;
    public float rotationSpeed = 0.3f;

    private Camera cam;

    // MOVE
    private bool dragging;
    private Transform activeAxis;
    private Plane dragPlane;
    private Vector3 axisDirection;
    private Vector3 startObjectPosition;
    private Vector3 startHitPoint;

    // ROTATE
    private bool rotating;
    private Transform activeRing;
    private Vector3 rotationAxis;
    private Vector3 lastMousePosition;

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

        // Ï‡Ò¯Ú‡· ÓÚÌÓÒËÚÂÎ¸ÌÓ Í‡ÏÂ˚
        float dist =
            Vector3.Distance(
                cam.transform.position,
                target.position
            );

        transform.localScale = Vector3.one * dist * gizmoScale;

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

                // ---------- MOVE ----------
                if (
                    hit.transform == xAxis ||
                    hit.transform == yAxis ||
                    hit.transform == zAxis
                )
                {
                    dragging = true;
                    rotating = false;

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

                    return;
                }

                // ---------- ROTATE ----------
                Transform root = hit.transform.root;

                if (
                    hit.transform == xRing ||
                    hit.transform == yRing ||
                    hit.transform == zRing ||

                    hit.transform.IsChildOf(xRing) ||
                    hit.transform.IsChildOf(yRing) ||
                    hit.transform.IsChildOf(zRing)
                )
                {

                    rotating = true;
                    dragging = false;

                    if (hit.transform.IsChildOf(xRing))
                        activeRing = xRing;
                    else if (hit.transform.IsChildOf(yRing))
                        activeRing = yRing;
                    else if (hit.transform.IsChildOf(zRing))
                        activeRing = zRing;
                    else
                        activeRing = hit.transform;

                    rotationAxis =
                        GetRotationAxis(activeRing);

                    lastMousePosition =
                        Input.mousePosition;

                    return;
                }
            }
        }

        
        // MOVE
        

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

        
        // ROTATE
        

        if (rotating && Input.GetMouseButton(0))
        {
            Vector3 mouseDelta =
                Input.mousePosition -
                lastMousePosition;

            float amount =
                mouseDelta.x + mouseDelta.y;

            target.Rotate(
                rotationAxis,
                amount * rotationSpeed,
                Space.World
            );

            lastMousePosition =
                Input.mousePosition;
        }

        
        // Œ“œ”— ¿Õ»≈
        

        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
            rotating = false;

            activeAxis = null;
            activeRing = null;
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

    Vector3 GetRotationAxis(Transform ring)
    {
        if (ring == xRing)
            return Vector3.right;

        if (ring == yRing)
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