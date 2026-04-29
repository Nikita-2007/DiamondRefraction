using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class DiamondGenerator : MonoBehaviour
{
    [Header("Основные параметры")]
    public float crownHeight = 0.35f;
    public float pavilionHeight = 0.8f;
    public float waistRadius = 1.0f;
    public float topRadius = 0.53f;

    [Header("Форма короны")]
    [Range(0.2f, 0.8f)]
    public float starFactor = 0.4f;

    private int segments = 8;

    private void Start() => Generate();
    private void OnValidate() => Generate();

    [ContextMenu("Generate")]
    private void Generate()
    {
        float angleStep = Mathf.PI * 2 / segments;

        Vector3[] table = new Vector3[segments];
        Vector3[] crownTips = new Vector3[segments];
        Vector3[] upperBelt = new Vector3[segments*2];
        Vector3[] belt = new Vector3[segments*2];

        // --- TABLE ---
        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep;
            float x = Mathf.Cos(angle);
            float z = Mathf.Sin(angle);

            table[i] = new Vector3(x * topRadius, crownHeight, z * topRadius);
        }

        // --- STAR RADIUS + HEIGHT ---
        float starRadius = Mathf.Lerp(topRadius, waistRadius, starFactor);
        float tipHeight = crownHeight * (1f - starFactor);

        // --- CROWN TIPS (жёлтые точки) ---
        for (int i = 0; i < segments; i++)
        {
            float midAngle = (i + 0.5f) * angleStep;

            float x = Mathf.Cos(midAngle);
            float z = Mathf.Sin(midAngle);

            crownTips[i] = new Vector3(
                x * starRadius,
                tipHeight,
                z * starRadius
            );
        }

        // --- UPPER BELT (верх пояса) ---
        float upperBeltHeight = crownHeight * 0.15f;

        for (int i = 0; i < segments*2; i++)
        {
            float angle = i * angleStep/2;

            float x = Mathf.Cos(angle);
            float z = Mathf.Sin(angle);

            upperBelt[i] = new Vector3(
                x * waistRadius,
                upperBeltHeight,
                z * waistRadius
            );
        }

        // --- LOWER BELT (низ пояса) ---
        for (int i = 0; i < segments*2; i++)
        {
            float angle = i * angleStep/2;

            float x = Mathf.Cos(angle);
            float z = Mathf.Sin(angle);

            belt[i] = new Vector3(
                x * waistRadius,
                0,
                z * waistRadius
            );
        }

        Vector3 pavilionTip = new Vector3(0, -pavilionHeight, 0);

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector3> norms = new List<Vector3>();

        void AddTriangle(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 normal = Vector3.Cross(b - a, c - a).normalized;

            int index = verts.Count;

            verts.Add(a);
            verts.Add(b);
            verts.Add(c);

            norms.Add(normal);
            norms.Add(normal);
            norms.Add(normal);

            tris.Add(index);
            tris.Add(index + 1);
            tris.Add(index + 2);
        }

        // --- 1. TABLE ---
        for (int i = 1; i < segments - 1; i++)
        {
            AddTriangle(table[0], table[i + 1], table[i]);
        }

        // --- 2. STAR FACETS ---
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;
            AddTriangle(table[i], table[next], crownTips[i]);
        }

        // --- 3.1 UPPER GIRDLE
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;

            Vector3 a = crownTips[i];
            Vector3 b = crownTips[next];
            Vector3 c = upperBelt[(i + 1)*2 % (segments * 2)];
            Vector3 d = table[next];

            AddTriangle(a, b, c);
            AddTriangle(a, d, b);
        }
        // --- 3.2 GIRDLE
        for (int i = 0; i < segments; i++)
        {
            int next = (i + 1) % segments;

            Vector3 a = crownTips[i];
            Vector3 b = upperBelt[i * 2];
            Vector3 c = upperBelt[(i * 2 + 1) % (segments * 2)];
            Vector3 d = upperBelt[(i * 2 + 2) % (segments * 2)];

            AddTriangle(a, c, b);
            AddTriangle(a, d, c);
        }
        // --- 4. ПЕРЕХОД К ПОЯСУ ---
        for (int i = 0; i < segments; i++)
        {
            int b0 = i * 2;
            int b1 = (i * 2 + 1) % (segments * 2);
            int b2 = (i * 2 + 2) % (segments * 2);

            // старая грань
            AddTriangle(upperBelt[b1], upperBelt[b2], belt[b2]);
            AddTriangle(upperBelt[b1], belt[b2], belt[b1]);

            AddTriangle(upperBelt[b0], upperBelt[b1], belt[b1]);
            AddTriangle(upperBelt[b0], belt[b1], belt[b0]);
        }

        // --- 5. ПАВИЛЬОН (пока простой) ---
        for (int i = 0; i < segments; i++)
        {
            Vector3 a = belt[i * 2];
            Vector3 b = belt[(i * 2 + 1) % (segments * 2)];
            Vector3 c = belt[(i * 2 + 2) % (segments * 2)];

            int next = (i + 1) % segments;
            AddTriangle(a, b, pavilionTip);
            AddTriangle(b, c, pavilionTip);
        }

        Mesh mesh = new Mesh();
        mesh.name = "Diamond";

        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.normals = norms.ToArray();

        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = mesh;
    }
}