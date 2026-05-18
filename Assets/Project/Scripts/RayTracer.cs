using UnityEngine;
using System.Collections.Generic;

public class RayTracer : MonoBehaviour
{
    [Header("Diamond")]
    public Collider diamondCollider;

    [Header("Ray Source")]
    [Range(1, 100)]
    public int rayCount = 5;

    public bool enableGrid = false;

    [Range(0.1f, 10f)]
    public float spread = 2f;

    [Header("Optics")]
    public float diamondIOR = 2.42f;

    [Range(1, 50)]
    public int maxBounces = 15;

    public float surfaceOffset = 0.001f;

    [Header("Visual")]
    public float lineWidth = 0.02f;
    public Material lineMaterial;

    [Header("Ray Direction")]
    public Transform rayDirection;

    private bool dirty = true;
    private int lastRayCount;
    private bool lastEnableGrid;
    private float lastSpread;
    private float lastIOR;
    private int lastBounces;

    private Vector3 lastDiamondPos;
    private Quaternion lastDiamondRot;
    private Vector3 lastDiamondScale;

    private Vector3 lastEmitterPos;
    private Quaternion lastEmitterRot;

    private readonly List<RaySegment> rootSegments = new();

    class RaySegment
    {
        public GameObject gameObject;
        public LineRenderer line;
        public RaySegment child;
    }

    void Start()
    {
        Physics.queriesHitBackfaces = true;
        MarkDirty();
    }

    void Update()
    {
        CheckChanges();

        if (dirty)
        {
            RebuildAllRays();
            dirty = false;
        }
    }

    void CheckChanges()
    {
        bool changed = false;

        // --- Diamond transform ---
        if (diamondCollider != null)
        {
            Transform t = diamondCollider.transform;

            if (
                t.position != lastDiamondPos ||
                t.rotation != lastDiamondRot ||
                t.localScale != lastDiamondScale
            )
            {
                lastDiamondPos = t.position;
                lastDiamondRot = t.rotation;
                lastDiamondScale = t.localScale;

                changed = true;
            }
        }

        // --- Emitter transform ---
        if (rayDirection != null)
        {
            if (
                rayDirection.position != lastEmitterPos ||
                rayDirection.rotation != lastEmitterRot
            )
            {
                lastEmitterPos = rayDirection.position;
                lastEmitterRot = rayDirection.rotation;

                changed = true;
            }
        }

        // --- Parameters ---
        if (
            rayCount != lastRayCount ||
            enableGrid != lastEnableGrid ||
            spread != lastSpread ||
            diamondIOR != lastIOR ||
            maxBounces != lastBounces
        )
        {
            lastRayCount = rayCount;
            lastEnableGrid = enableGrid;
            lastSpread = spread;
            lastIOR = diamondIOR;
            lastBounces = maxBounces;

            changed = true;
        }

        if (changed)
            MarkDirty();
    }

    public void MarkDirty()
    {
        dirty = true;
    }

    void RebuildAllRays()
    {
        ClearAll();

        int xCount = enableGrid ? rayCount : 1;

        for (int ix = 0; ix < xCount; ix++)
        {
            float tx = xCount == 1
                ? 0.5f
                : (float)ix / (xCount - 1);

            float zOffset = Mathf.Lerp(
                -spread * 0.5f,
                spread * 0.5f,
                tx
            );

            for (int iy = 0; iy < rayCount; iy++)
            {
                float ty = rayCount == 1
                    ? 0.5f
                    : (float)iy / (rayCount - 1);

                float yOffset = Mathf.Lerp(
                    -spread * 0.5f,
                    spread * 0.5f,
                    ty
                );

                Vector3 localOffset =
                    rayDirection.up * yOffset +
                    rayDirection.right * zOffset;

                Vector3 startPos;

                if (rayDirection != null)
                {
                    startPos =  rayDirection.position + localOffset;
                }
                else
                {
                    startPos = new Vector3(0, yOffset, zOffset);
                }

                Vector3 dir =
                    rayDirection != null
                    ? rayDirection.forward
                    : Vector3.right;

                RaySegment root = CreateSegment(
                    null,
                    startPos,
                    startPos
                );

                rootSegments.Add(root);

                TraceRayRecursive(
                    root,
                    startPos,
                    dir.normalized,
                    false,
                    0
                );
            }
        }
    }

    void TraceRayRecursive(
        RaySegment seg,
        Vector3 origin,
        Vector3 dir,
        bool insideDiamond,
        int depth
    )
    {
        if (depth >= maxBounces)
        {
            seg.line.SetPosition(0, origin);
            seg.line.SetPosition(1, origin + dir * 100f);
            return;
        }

        Ray ray = new Ray(origin, dir);

        if (
            diamondCollider.Raycast(
                ray,
                out RaycastHit hit,
                100f
            )
        )
        {
            seg.line.SetPosition(0, origin);
            seg.line.SetPosition(1, hit.point);

            Vector3 normal = hit.normal;

            if (Vector3.Dot(dir, normal) > 0f)
                normal = -normal;

            float n1 = insideDiamond ? diamondIOR : 1f;
            float n2 = insideDiamond ? 1f : diamondIOR;

            Vector3 newDir =
                Refract(
                    dir,
                    normal,
                    n1,
                    n2,
                    out bool tir
                );

            if (tir)
            {
                newDir =
                    Vector3.Reflect(dir, normal).normalized;
            }

            bool newInside =
                tir ? insideDiamond : !insideDiamond;

            RaySegment child = CreateSegment(
                seg,
                hit.point,
                hit.point
            );

            seg.child = child;

            TraceRayRecursive(
                child,
                hit.point + newDir * surfaceOffset,
                newDir,
                newInside,
                depth + 1
            );
        }
        else
        {
            seg.line.SetPosition(0, origin);
            seg.line.SetPosition(1, origin + dir * 100f);
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

        float cosI =
            -Vector3.Dot(normal, incident);

        float sinT2 =
            eta * eta * (1f - cosI * cosI);

        if (sinT2 > 1f)
        {
            tir = true;
            return Vector3.zero;
        }

        tir = false;

        float cosT =
            Mathf.Sqrt(1f - sinT2);

        return (
            eta * incident +
            (eta * cosI - cosT) * normal
        ).normalized;
    }

    RaySegment CreateSegment(
        RaySegment parent,
        Vector3 start,
        Vector3 end
    )
    {
        GameObject go =
            new GameObject("RaySegment");

        go.transform.SetParent(
            parent != null
            ? parent.gameObject.transform
            : transform
        );

        LineRenderer lr =
            go.AddComponent<LineRenderer>();

        lr.material =
            lineMaterial != null
            ? lineMaterial
            : new Material(
                Shader.Find("Sprites/Default")
            );

        lr.positionCount = 2;

        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;

        lr.useWorldSpace = true;

        lr.SetPosition(0, start);
        lr.SetPosition(1, end);

        lr.startColor = Color.white;
        lr.endColor = Color.white;

        return new RaySegment
        {
            gameObject = go,
            line = lr
        };
    }

    void ClearAll()
    {
        foreach (var seg in rootSegments)
        {
            if (seg != null && seg.gameObject != null)
            {
                Destroy(seg.gameObject);
            }
        }

        rootSegments.Clear();
    }
}