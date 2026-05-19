using UnityEngine;

public class GizmoAxis : MonoBehaviour
{
    public enum Axis
    {
        X,
        Y,
        Z
    }

    public Axis axis;

    private bool dragging;

    private Vector3 lastMouse;

    private Transform target;

    public void SetTarget(Transform t)
    {
        target = t;
    }

    void OnMouseDown()
    {
        dragging = true;
        lastMouse = Input.mousePosition;
    }

    void OnMouseUp()
    {
        dragging = false;
    }

    void Update()
    {
        if (!dragging || target == null)
            return;

        Vector3 delta =
            Input.mousePosition - lastMouse;

        float move =
            (delta.x + delta.y) * 0.01f;

        Vector3 dir = Vector3.right;

        switch (axis)
        {
            case Axis.X:
                dir = Vector3.right;
                break;

            case Axis.Y:
                dir = Vector3.up;
                break;

            case Axis.Z:
                dir = Vector3.forward;
                break;
        }

        target.position += dir * move;

        lastMouse = Input.mousePosition;
    }
}