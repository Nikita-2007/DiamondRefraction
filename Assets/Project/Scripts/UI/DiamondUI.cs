using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiamondUI : MonoBehaviour
{
    public DiamondGenerator diamond;
    public RayTracer tracer;

    [Header("Sliders")]
    public Slider crownSlider;
    public Slider pavilionSlider;
    public Slider waistSlider;
    public Slider topSlider;

    [Header("Value Text")]
    public TMP_Text crownValue;
    public TMP_Text pavilionValue;
    public TMP_Text waistValue;
    public TMP_Text topValue;

    void Start()
    {
        crownSlider.value = diamond.crownHeight;
        pavilionSlider.value = diamond.pavilionHeight;
        waistSlider.value = diamond.waistRadius;
        topSlider.value = diamond.topRadius;

        crownSlider.onValueChanged.AddListener(OnCrownChanged);
        pavilionSlider.onValueChanged.AddListener(OnPavilionChanged);
        waistSlider.onValueChanged.AddListener(OnWaistChanged);
        topSlider.onValueChanged.AddListener(OnTopChanged);

        RefreshText();
    }

    void OnCrownChanged(float v)
    {
        diamond.crownHeight = v;

        diamond.Generate();

        tracer.MarkDirty();

        RefreshText();
    }

    void OnPavilionChanged(float v)
    {
        diamond.pavilionHeight = v;

        diamond.Generate();

        tracer.MarkDirty();

        RefreshText();
    }

    void OnWaistChanged(float v)
    {
        diamond.waistRadius = v;

        diamond.Generate();

        tracer.MarkDirty();

        RefreshText();
    }

    void OnTopChanged(float v)
    {
        diamond.topRadius = v;

        diamond.Generate();

        tracer.MarkDirty();

        RefreshText();
    }

    void RefreshText()
    {
        if (crownValue != null)
            crownValue.text =
                diamond.crownHeight.ToString("0.00");

        if (pavilionValue != null)
            pavilionValue.text =
                diamond.pavilionHeight.ToString("0.00");

        if (waistValue != null)
            waistValue.text =
                diamond.waistRadius.ToString("0.00");

        if (topValue != null)
            topValue.text =
                diamond.topRadius.ToString("0.00");
    }
}