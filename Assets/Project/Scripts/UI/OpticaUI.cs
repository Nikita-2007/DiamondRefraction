using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OpticaUI : MonoBehaviour
{
    [Header("Ray Mode")]
    public RayTracer tracer;

    [Header("Monte Carlo")]
    public MonteCarloSimulator simulator;

    [Header("Tabs")]
    public GameObject raysPanel;
    public GameObject particlesPanel;

    public Button raysTabButton;
    public Button particlesTabButton;

    [Header("Ray Settings")]
    public Slider rayCountSlider;
    public Toggle gridToggle;
    public Toggle chromaticToggle;

    public Slider bouncesSlider;
    public Slider spreadSlider;
    public Slider iorSlider;

    [Header("Particle Settings")]
    public TMP_Dropdown modeDropdown;
    public TMP_Dropdown patternDropdown;

    public Slider photonSlider;
    public Slider particleSpreadSlider;
    public Slider particleIORSlider;
    public Slider particleBounceSlider;
    public Toggle ParticleChromaticToggle;

    public Button simulateButton;
    public Button cancelButton;

    [Header("Ray Text")]
    public TMP_Text rayCountValue;
    public TMP_Text bouncesValue;
    public TMP_Text spreadValue;
    public TMP_Text iorValue;

    [Header("Particle Text")]
    public TMP_Text photonValue;
    public TMP_Text particleSpreadValue;
    public TMP_Text particleIORValue;
    public TMP_Text particleBounceValue;

    void Start()
    {
        // ---------- RAY ----------

        rayCountSlider.value = tracer.rayCount;
        spreadSlider.value = tracer.spread;
        iorSlider.value = tracer.diamondIOR;
        bouncesSlider.value = tracer.maxBounces;

        gridToggle.isOn = tracer.enableGrid;
        chromaticToggle.isOn = tracer.chromaticAberration;
        

        rayCountSlider.onValueChanged
            .AddListener(OnRayCountChanged);

        spreadSlider.onValueChanged
            .AddListener(OnSpreadChanged);

        iorSlider.onValueChanged
            .AddListener(OnIORChanged);

        bouncesSlider.onValueChanged
            .AddListener(OnBouncesChanged);

        gridToggle.onValueChanged
            .AddListener(OnGridChanged);

        chromaticToggle.onValueChanged
            .AddListener(OnChromaticChanged);

        // ---------- PARTICLES ----------

        photonSlider.value =
            simulator.photonCount;

        particleSpreadSlider.value =
            tracer.spread;

        particleIORSlider.value =
            simulator.ior;

        particleBounceSlider.value =
            simulator.maxBounces;
        ParticleChromaticToggle.isOn = simulator.chromaticAberration;

        photonSlider.onValueChanged
            .AddListener(OnPhotonChanged);

        particleSpreadSlider.onValueChanged
            .AddListener(OnParticleSpreadChanged);

        particleIORSlider.onValueChanged
            .AddListener(OnParticleIORChanged);

        particleBounceSlider.onValueChanged
            .AddListener(OnParticleBounceChanged);

        ParticleChromaticToggle.onValueChanged
            .AddListener(OnParticleChromaticChanged);

        modeDropdown.onValueChanged
            .AddListener(OnModeChanged);

        patternDropdown.onValueChanged
            .AddListener(OnPatternChanged);

        simulateButton.onClick
            .AddListener(simulator.StartSimulation);

        cancelButton.onClick
            .AddListener(simulator.CancelSimulation);

        raysTabButton.onClick
            .AddListener(OpenRaysTab);

        particlesTabButton.onClick
            .AddListener(OpenParticlesTab);

        RefreshText();

        OpenRaysTab();
    }

    // =========================
    // TABS
    // =========================

    void OpenRaysTab()
    {
        raysPanel.SetActive(true);
        particlesPanel.SetActive(false);

        simulator.ClearPhotons();

        tracer.visible = true;
        tracer.MarkDirty();
    }

    void OpenParticlesTab()
    {
        raysPanel.SetActive(false);
        particlesPanel.SetActive(true);

        tracer.visible = false;
        tracer.MarkDirty();
    }

    // =========================
    // RAY EVENTS
    // =========================

    void OnRayCountChanged(float v)
    {
        tracer.rayCount = (int)v;
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

    void OnBouncesChanged(float v)
    {
        tracer.maxBounces = (int)v;
        tracer.MarkDirty();
        RefreshText();
    }

    void OnGridChanged(bool v)
    {
        tracer.enableGrid = v;
        tracer.MarkDirty();
    }

    void OnChromaticChanged(bool v)
    {
        tracer.chromaticAberration = v;
        tracer.MarkDirty();
    }

    // =========================
    // PARTICLE EVENTS
    // =========================

    void OnPhotonChanged(float v)
    {
        simulator.photonCount = (int)v;
        RefreshText();
    }

    void OnParticleSpreadChanged(float v)
    {
        simulator.spread = v;
        RefreshText();
    }

    void OnParticleIORChanged(float v)
    {
        simulator.ior = v;
        RefreshText();
    }

    
    void OnParticleChromaticChanged(bool v)
    {
        simulator.chromaticAberration = v;
        RefreshText();
    }

    void OnParticleBounceChanged(float v)
    {
        simulator.maxBounces = (int)v;
        RefreshText();
    }

    void OnModeChanged(int v)
    {
        simulator.mode =
            (PhotonMode)v;
    }

    void OnPatternChanged(int v)
    {
        simulator.pattern =
            (PhotonPattern)v;
    }

    // =========================
    // TEXT
    // =========================

    void RefreshText()
    {
        rayCountValue.text =
            tracer.rayCount.ToString();

        spreadValue.text =
            tracer.spread.ToString("0.00");

        iorValue.text =
            tracer.diamondIOR.ToString("0.00");

        bouncesValue.text =
            tracer.maxBounces.ToString();

        photonValue.text =
            simulator.photonCount.ToString();

        particleSpreadValue.text =
            simulator.spread.ToString("0.00");

        particleIORValue.text =
            simulator.ior.ToString("0.00");

        particleBounceValue.text =
            simulator.maxBounces.ToString();
    }
}