using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OpticaUI : MonoBehaviour
{
    public RayTracer tracer;

    [Header("Sliders")]
    public Slider rayCountSlider;
    public Toggle gridToggle;
    public Slider bouncesSlider;
    public Slider spreadSlider;
    public Slider iorSlider;
    public Toggle —hromaticAberration;

    [Header("Value Text")]
    public TMP_Text rayCountValue;
    public TMP_Text bouncesValue;
    public TMP_Text spreadValue;
    public TMP_Text iorValue;

    void Start()
    {
        rayCountSlider.value = tracer.rayCount;
        bouncesSlider.value = tracer.maxBounces;
        spreadSlider.value = tracer.spread;
        iorSlider.value = tracer.diamondIOR;

        rayCountSlider.onValueChanged.AddListener(OnRayCountChanged);
        gridToggle.onValueChanged.AddListener(OnGridChanged);
        —hromaticAberration.onValueChanged.AddListener(On—hromaticAberrationChanged);
        bouncesSlider.onValueChanged.AddListener(OnBouncesChanged);
        spreadSlider.onValueChanged.AddListener(OnSpreadChanged);
        iorSlider.onValueChanged.AddListener(OnIORChanged);

        RefreshText();
    }

    void OnRayCountChanged(float v)
    {
        tracer.rayCount = (int)v;
        tracer.MarkDirty();
        RefreshText();
    }

    void OnGridChanged(bool v)
    {
        tracer.enableGrid = v;
        tracer.MarkDirty();
    }

    void On—hromaticAberrationChanged(bool v)
    {
        tracer.chromaticAberration = v;
        tracer.MarkDirty();
    }

    void OnBouncesChanged(float v)
    {
        tracer.maxBounces = (int)v;
        tracer.MarkDirty();
        RefreshText();
    }

    void OnSpreadChanged(float v)
    {
        tracer.spread = v;
        tracer.MarkDirty();
        RefreshText();
    }

    void OnIORChanged(float v)
    {
        tracer.diamondIOR = v;
        tracer.MarkDirty();
        RefreshText();
    }

    void RefreshText()
    {
        if (rayCountValue != null)
            rayCountValue.text =
                tracer.rayCount.ToString("0");

        if (bouncesValue != null)
            bouncesValue.text =
                tracer.maxBounces.ToString("0");

        if (spreadValue != null)
            spreadValue.text =
                tracer.spread.ToString("0.00");

        if (iorValue != null)
            iorValue.text =
                tracer.diamondIOR.ToString("0.00");
    }
}