using UnityEngine;

public class RuntimeTransformGizmo : MonoBehaviour
{
    public static RuntimeTransformGizmo Instance;

    public Transform target;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position;
        }
    }

    public void SetTarget(Transform t)
    {
        target = t;

        gameObject.SetActive(true);

        foreach (var axis in GetComponentsInChildren<GizmoAxis>())
        {
            axis.SetTarget(t);
        }
    }

    public void Hide()
    {
        target = null;
        gameObject.SetActive(false);
    }
}