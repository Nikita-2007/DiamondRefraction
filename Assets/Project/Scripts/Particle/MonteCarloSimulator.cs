using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MonteCarloSimulator : MonoBehaviour
{
    [Header("References")]
    public RayTracer tracer;
    public Collider diamondCollider;
    public GameObject photonPrefab;
    public Transform emitter;

    [Header("UI")]
    public Slider progressBar;
    public TMP_Text progressText;

    [Header("Settings")]
    public int photonCount = 5000;

    public float spread = 1f;

    public int maxBounces = 8;

    public float ior = 2.42f;

    public PhotonMode mode =
        PhotonMode.Collimated;

    public PhotonPattern pattern =
        PhotonPattern.White;

    public bool chromaticAberration = false;

    private List<GameObject> photons =
        new List<GameObject>();

    private Transform container;

    private bool cancelled;

    void Awake()
    {
        CreateContainer();
    }

    void CreateContainer()
    {
        GameObject go =
            new GameObject("PhotonContainer");

        container = go.transform;
    }

    public void StartSimulation()
    {
        StopAllCoroutines();

        ClearPhotons();

        cancelled = false;

        StartCoroutine(Simulate());
    }

    public void CancelSimulation()
    {
        cancelled = true;
    }

    IEnumerator Simulate()
    {
        if (progressBar != null)
            progressBar.value = 0;

        for (int i = 0; i < photonCount; i++)
        {
            if (cancelled)
                yield break;

            SpawnPhoton(i);

            float progress =
                (float)i / photonCount;

            if (progressBar != null)
                progressBar.value = progress;

            if (progressText != null)
            {
                progressText.text =
                    Mathf.RoundToInt(
                        progress * 100f
                    ) + "%";
            }

            if (i % 100 == 0)
                yield return null;
        }

        if (progressBar != null)
            progressBar.value = 1;

        if (progressText != null)
            progressText.text = "100%";
    }

    void SpawnPhoton(int index)
    {
        Vector2 uv =
            GeneratePatternUV(index);

        Vector3 localOffset =
            emitter.right * uv.x * spread +
            emitter.up * uv.y * spread;

        Vector3 origin;
        Vector3 dir;

        // ================= COLLIMATED

        if (mode == PhotonMode.Collimated)
        {
            origin =
                emitter.position +
                localOffset;

            dir =
                emitter.forward;
        }
        else
        {
            // ================= PROJECTOR

            origin =
                emitter.position;

            Vector3 target =
                emitter.position +
                emitter.forward * 5f +
                localOffset;

            dir =
                (target - origin).normalized;
        }

        Color color =
            GetPatternColor(uv);

        // ================= CHROMATIC

        if (chromaticAberration)
        {
            Color r =
                new Color(
                    color.r,
                    0,
                    0
                );

            Color g =
                new Color(
                    0,
                    color.g,
                    0
                );

            Color b =
                new Color(
                    0,
                    0,
                    color.b
                );

            SpawnSingle(
                origin,
                Quaternion.AngleAxis(
                    -0.8f,
                    emitter.up
                ) * dir,
                r,
                2.40f
            );

            SpawnSingle(
                origin,
                dir,
                g,
                2.42f
            );

            SpawnSingle(
                origin,
                Quaternion.AngleAxis(
                    0.8f,
                    emitter.up
                ) * dir,
                b,
                2.44f
            );
        }
        else
        {
            SpawnSingle(
                origin,
                dir,
                color,
                ior
            );
        }
    }

    void SpawnSingle(
        Vector3 origin,
        Vector3 dir,
        Color color,
        float localIOR
    )
    {
        Vector3 pos =
            TracePhoton(
                origin,
                dir,
                localIOR
            );

        GameObject p =
            Instantiate(
                photonPrefab,
                pos,
                Quaternion.identity,
                container
            );

        Renderer r =
            p.GetComponent<Renderer>();

        if (r != null)
        {
            r.material =
                new Material(r.material);

            r.material.color = color;
        }

        photons.Add(p);
    }

    Vector3 TracePhoton(
        Vector3 origin,
        Vector3 dir,
        float localIOR
    )
    {
        bool inside = false;

        Vector3 currentOrigin = origin;

        Vector3 currentDir = dir;

        for (int i = 0; i < maxBounces; i++)
        {
            Ray ray =
                new Ray(
                    currentOrigin,
                    currentDir
                );

            if (
                diamondCollider.Raycast(
                    ray,
                    out RaycastHit hit,
                    100f
                )
            )
            {
                Vector3 normal =
                    hit.normal;

                if (
                    Vector3.Dot(
                        currentDir,
                        normal
                    ) > 0
                )
                {
                    normal = -normal;
                }

                float n1 =
                    inside ? localIOR : 1f;

                float n2 =
                    inside ? 1f : localIOR;

                Vector3 newDir =
                    Refract(
                        currentDir,
                        normal,
                        n1,
                        n2,
                        out bool tir
                    );

                if (tir)
                {
                    newDir =
                        Vector3.Reflect(
                            currentDir,
                            normal
                        ).normalized;
                }
                else
                {
                    inside = !inside;
                }

                currentOrigin =
                    hit.point +
                    newDir * 0.01f;

                currentDir =
                    newDir;
            }
            else
            {
                return currentOrigin +
                       currentDir * 8f;
            }
        }

        return currentOrigin;
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

        float eta =
            n1 / n2;

        float cosI =
            -Vector3.Dot(
                normal,
                incident
            );

        float sinT2 =
            eta * eta *
            (1f - cosI * cosI);

        if (sinT2 > 1f)
        {
            tir = true;
            return Vector3.zero;
        }

        tir = false;

        float cosT =
            Mathf.Sqrt(
                1f - sinT2
            );

        return (
            eta * incident +
            (eta * cosI - cosT) * normal
        ).normalized;
    }

    Vector2 GeneratePatternUV(int index)
    {
        switch (pattern)
        {
            // ================= WHITE

            case PhotonPattern.White:
            {
                return new Vector2(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)
                );
            }

            // ================= GRADIENT

            case PhotonPattern.Gradient:
            {
                return new Vector2(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)
                );
            }

            // ================= RAINBOW

            case PhotonPattern.Rainbow:
            {
                return new Vector2(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)
                );
            }

            // ================= TRIANGLE

            case PhotonPattern.Triangle:
            {
                while (true)
                {
                    float x =
                        Random.Range(-1f, 1f);

                    float y =
                        Random.Range(-1f, 1f);

                    if (y < (-Mathf.Abs(x)*2 + 1f)
                    )
                    {
                        return new Vector2(x, y);
                    }
                }
            }

            // ================= LETTER F

            case PhotonPattern.LetterF:
            {
                while (true)
                {
                    float x =
                        -Random.Range(-1f, 1f);

                    float y =
                        Random.Range(-1f, 1f);

                    bool vertical =
                        x < -0.5f;

                    bool top =
                        y > 0.4f &&
                        x < 0.5f;

                    bool middle =
                        y > -0.1f &&
                        y < 0.2f &&
                        x < 0.1f;

                    if (
                        vertical ||
                        top ||
                        middle
                    )
                    {
                        return new Vector2(x, y);
                    }
                }
            }
        }

        return Vector2.zero;
    }

    Color GetPatternColor(Vector2 uv)
    {
        switch (pattern)
        {
            // ================= RAINBOW

            case PhotonPattern.Rainbow:
                {
                    float hue =
                        Mathf.InverseLerp(
                            -1f,
                            1f,
                            uv.y
                        );

                    return Color.HSVToRGB(
                        hue,
                        1f,
                        1f
                    );
                }

            // ================= GRADIENT

            case PhotonPattern.Gradient:
                {
                    float t =
                        Mathf.InverseLerp(
                            -1f,
                            1f,
                            uv.y * 0.9f
                        );

                    return new Color(
                        t,
                        t,
                        t,
                        1f
                    );
                }
        }

        return Color.white;
    }

    public void ClearPhotons()
    {
        foreach (GameObject p in photons)
        {
            if (p != null)
                Destroy(p);
        }

        photons.Clear();
    }
}