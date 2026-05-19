using UnityEngine;

[ExecuteAlways]
public class RayEmitterVisualizer : MonoBehaviour
{
    public RayTracer rayTracer;

    [Header("Visual")]
    public float thickness = 0.02f;

    void Update()
    {
        if (rayTracer == null)
            return;

        int count = Mathf.Max(1, rayTracer.rayCount);

        float size = rayTracer.spread;

        Vector3 scale = transform.localScale;

        scale.z = thickness;

        scale.y = size + 0.1f;

        scale.x = rayTracer.enableGrid
            ? size + 0.1f
            : thickness;

        transform.localScale = scale;
        BoxCollider col = GetComponent<BoxCollider>();

        if (col != null)
        {
            col.size = Vector3.one;
        }
    }
}