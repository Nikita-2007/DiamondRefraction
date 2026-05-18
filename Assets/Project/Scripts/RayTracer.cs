using UnityEngine;
using System.Collections.Generic;

public class RayTracer : MonoBehaviour
{
    [Header("Diamond")]
    public Collider diamondCollider;

    [Header("Ray Source")]
    public RaySource raySource;          // Ссылка на источник лучей

    [Header("Optics")]
    public float diamondIOR = 2.42f;
    public int maxBounces = 20;
    public float surfaceOffset = 0.005f;

    [Header("Visual")]
    public float lineWidth = 0.02f;
    public Material lineMaterial;

    // Состояние иерархии
    private List<RaySegment> rootSegments = new List<RaySegment>();
    private Vector3 lastDiamondPos;
    private Quaternion lastDiamondRot;
    private Vector3 lastDiamondScale;
    private Vector3 lastSourcePos;
    private Quaternion lastSourceRot;
    private int lastRayCount;
    private float lastSourceHeight;
    private float lastSourceWidth;
    private bool dirty = true;

    class RaySegment
    {
        public GameObject gameObject;
        public LineRenderer line;
        public RaySegment parent;
        public RaySegment child;
    }

    void Start()
    {
        Physics.queriesHitBackfaces = true;
        MarkDirty();
    }

    void OnValidate()
    {
        if (Application.isPlaying)
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
        if (diamondCollider != null)
        {
            Transform t = diamondCollider.transform;
            if (t.position != lastDiamondPos || t.rotation != lastDiamondRot || t.localScale != lastDiamondScale)
            {
                lastDiamondPos = t.position;
                lastDiamondRot = t.rotation;
                lastDiamondScale = t.localScale;
                MarkDirty();
            }
        }

        if (raySource != null)
        {
            Transform s = raySource.transform;
            if (s.position != lastSourcePos || s.rotation != lastSourceRot ||
                raySource.rayCount != lastRayCount ||
                raySource.sourceHeight != lastSourceHeight ||
                raySource.sourceWidth != lastSourceWidth)
            {
                lastSourcePos = s.position;
                lastSourceRot = s.rotation;
                lastRayCount = raySource.rayCount;
                lastSourceHeight = raySource.sourceHeight;
                lastSourceWidth = raySource.sourceWidth;
                MarkDirty();
            }
        }
    }

    public void MarkDirty() => dirty = true;

    void RebuildAllRays()
    {
        ClearAll();

        if (raySource == null) return;

        // Создаём лучи в 2D сетке (по ширине и высоте источника)
        for (int i = 0; i < raySource.rayCount; i++)
        {
            float tX = raySource.rayCount == 1 ? 0.5f : (float)i / (raySource.rayCount - 1);
            float x = Mathf.Lerp(-raySource.sourceWidth * 0.5f, raySource.sourceWidth * 0.5f, tX);

            for (int j = 0; j < raySource.rayCount; j++)
            {
                float tY = raySource.rayCount == 1 ? 0.5f : (float)j / (raySource.rayCount - 1);
                float y = Mathf.Lerp(-raySource.sourceHeight * 0.5f, raySource.sourceHeight * 0.5f, tY);

                Vector3 localStart = new Vector3(x, y, 0f);
                Vector3 start = raySource.transform.TransformPoint(localStart);
                Vector3 dir = raySource.transform.right; // лучи летят вперёд по красной оси

                RaySegment root = CreateSegment(null, start, start);
                rootSegments.Add(root);
                TraceRayRecursive(root, start, dir, false, 0);
            }
        }
    }

    void TraceRayRecursive(RaySegment seg, Vector3 origin, Vector3 dir, bool insideDiamond, int depth)
    {
        if (depth >= maxBounces)
        {
            seg.line.SetPosition(0, origin);
            seg.line.SetPosition(1, origin + dir * 100f);
            return;
        }

        Ray ray = new Ray(origin, dir);
        if (diamondCollider != null && diamondCollider.Raycast(ray, out RaycastHit hit, 100f))
        {
            seg.line.SetPosition(0, origin);
            seg.line.SetPosition(1, hit.point);

            Vector3 normal = hit.normal;
            if (Vector3.Dot(dir, normal) > 0f) normal = -normal;

            float n1 = insideDiamond ? diamondIOR : 1f;
            float n2 = insideDiamond ? 1f : diamondIOR;

            Vector3 newDir = Refract(dir, normal, n1, n2, out bool tir);
            if (tir) newDir = Vector3.Reflect(dir, normal).normalized;

            bool newInside = tir ? insideDiamond : !insideDiamond;

            RaySegment child = CreateSegment(seg, hit.point, hit.point);
            seg.child = child;
            TraceRayRecursive(child, hit.point + newDir * surfaceOffset, newDir, newInside, depth + 1);
        }
        else
        {
            seg.line.SetPosition(0, origin);
            seg.line.SetPosition(1, origin + dir * 100f);
        }
    }

    Vector3 Refract(Vector3 incident, Vector3 normal, float n1, float n2, out bool tir)
    {
        incident.Normalize();
        normal.Normalize();
        float eta = n1 / n2;
        float cosI = -Vector3.Dot(normal, incident);
        float sin2 = eta * eta * (1f - cosI * cosI);
        if (sin2 > 1f)
        {
            tir = true;
            return Vector3.zero;
        }
        tir = false;
        float cosT = Mathf.Sqrt(1f - sin2);
        return (eta * incident + (eta * cosI - cosT) * normal).normalized;
    }

    RaySegment CreateSegment(RaySegment parent, Vector3 start, Vector3 end)
    {
        GameObject go = new GameObject("RaySegment");
        go.transform.SetParent(parent != null ? parent.gameObject.transform : transform);
        LineRenderer lr = go.AddComponent<LineRenderer>();
        lr.material = lineMaterial ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.useWorldSpace = true;
        lr.startColor = Color.white;
        lr.endColor = Color.white;

        return new RaySegment { gameObject = go, line = lr, parent = parent };
    }

    void ClearAll()
    {
        foreach (var seg in rootSegments)
        {
            if (seg?.gameObject != null)
            {
                if (Application.isPlaying)
                    Destroy(seg.gameObject);
                else
                    DestroyImmediate(seg.gameObject);
            }
        }
        rootSegments.Clear();

        // Удаляем всех детей на всякий случай
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }
    }
}