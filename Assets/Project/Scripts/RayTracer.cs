using UnityEngine;
using System.Collections.Generic;

public class RayTracer : MonoBehaviour
{
    public LayerMask diamondLayer;

    [Header("Scene")]
    public Collider diamondCollider;

    [Header("Rays")]
    public int rayCount = 20;
    public float startX = -5f;
    public float heightRange = 2f;
    public float maxDistance = 30f;

    [Header("Optics")]
    public float diamondIOR = 2.42f;
    public int maxBounces = 20;
    public float surfaceOffset = 0.01f;

    [Header("Rendering")]
    public float lineWidth = 0.02f;

    private readonly List<LineRenderer> renderers = new();

    void Start()
    {
        CreateRenderers(500);
    }

    void Update()
    {
        int rendererIndex = 0;

        for (int i = 0; i < rayCount; i++)
        {
            float t = rayCount == 1
                ? 0.5f
                : (float)i / (rayCount - 1);

            float y = Mathf.Lerp(
                -heightRange * 0.5f,
                heightRange * 0.5f,
                t
            );

            Vector3 origin = new Vector3(startX, y, 0f);
            Vector3 dir = Vector3.right;

            bool inside = false;

            for (int bounce = 0; bounce < maxBounces; bounce++)
            {
                Ray ray = new Ray(origin, dir);
                
                if (!Physics.Raycast(
                    ray,
                    out RaycastHit hit,
                    maxDistance,
                    diamondLayer
                ))
                {
                    DrawSegment(
                        rendererIndex++,
                        origin,
                        origin + dir * maxDistance,
                        inside ? Color.yellow : Color.white
                    );

                    break;
                }

                Vector3 normal = hit.normal.normalized;

                bool entering = Vector3.Dot(dir, normal) < 0f;

                // DEBUG COLOR
                Color segmentColor;

                if (entering)
                    segmentColor = Color.red;
                else
                    segmentColor = Color.blue;

                DrawSegment(
                    rendererIndex++,
                    origin,
                    hit.point,
                    segmentColor
                );

                float n1 = entering ? 1f : diamondIOR;
                float n2 = entering ? diamondIOR : 1f;

                Vector3 adjustedNormal =
                    entering
                    ? normal
                    : -normal;

                bool tir;

                Vector3 refracted = Refract(
                    dir,
                    adjustedNormal,
                    n1,
                    n2,
                    out tir
                );

                // Полное внутреннее отражение
                if (tir)
                {
                    Vector3 reflected =
                        Vector3.Reflect(dir, adjustedNormal).normalized;

                    DrawSegment(
                        rendererIndex++,
                        hit.point,
                        hit.point + reflected * 0.5f,
                        Color.green
                    );

                    dir = reflected;

                    origin = hit.point + dir * surfaceOffset;

                    inside = true;

                    continue;
                }

                dir = refracted.normalized;

                origin = hit.point + dir * surfaceOffset;

                inside = !entering;
            }
        }

        // Hide unused renderers
        for (int i = rendererIndex; i < renderers.Count; i++)
        {
            renderers[i].positionCount = 0;
        }
    }

    void DrawSegment(
        int index,
        Vector3 a,
        Vector3 b,
        Color color
    )
    {
        if (index >= renderers.Count)
            CreateRenderers(100);

        LineRenderer lr = renderers[index];

        lr.positionCount = 2;

        lr.SetPosition(0, a);
        lr.SetPosition(1, b);

        lr.startColor = color;
        lr.endColor = color;
    }

    void CreateRenderers(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject go = new GameObject($"Segment_{renderers.Count}");

            go.transform.SetParent(transform);

            LineRenderer lr = go.AddComponent<LineRenderer>();

            lr.material =
                new Material(Shader.Find("Sprites/Default"));

            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;

            renderers.Add(lr);
        }
    }

    Vector3 Refract(
        Vector3 incident,
        Vector3 normal,
        float n1,
        float n2,
        out bool tir
    )
    {
        incident.Normalize();
        normal.Normalize();

        float eta = n1 / n2;

        float cosI = -Vector3.Dot(normal, incident);

        float sinT2 =
            eta * eta * (1f - cosI * cosI);

        // Полное внутреннее отражение
        if (sinT2 > 1f)
        {
            tir = true;
            return Vector3.zero;
        }

        tir = false;

        float cosT = Mathf.Sqrt(1f - sinT2);

        Vector3 refracted =
            eta * incident +
            (eta * cosI - cosT) * normal;

        return refracted.normalized;
    }
}