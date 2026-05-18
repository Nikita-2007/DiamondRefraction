using UnityEngine;
using UnityEngine.UI;

public class ObjectSelector : MonoBehaviour
{
    public GameObject settingsCanvas;
    public Text titleText;
    public RectTransform slidersContainer;

    private GameObject currentTarget;
    private DiamondGenerator diamond;
    private RaySource raySource;

    void Start()
    {
        settingsCanvas.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                SelectObject(hit.collider.gameObject);
            }
            else
            {
                settingsCanvas.SetActive(false);
            }
        }
    }

    void SelectObject(GameObject obj)
    {
        currentTarget = obj;
        diamond = obj.GetComponent<DiamondGenerator>();
        raySource = obj.GetComponent<RaySource>();

        if (diamond != null || raySource != null)
        {
            settingsCanvas.SetActive(true);
            UpdateUI();
        }
        else
        {
            settingsCanvas.SetActive(false);
        }
    }

    void UpdateUI()
    {
        // Очищаем старые слайдеры
        foreach (Transform child in slidersContainer)
            Destroy(child.gameObject);

        if (diamond != null)
        {
            titleText.text = "Бриллиант";
            AddFloatSlider("Радиус пояса", diamond.waistRadius, v => diamond.waistRadius = v);
            AddFloatSlider("Радиус стола", diamond.topRadius, v => diamond.topRadius = v);
            AddFloatSlider("Высота короны", diamond.crownHeight, v => diamond.crownHeight = v);
            AddFloatSlider("Глубина павильона", diamond.pavilionHeight, v => diamond.pavilionHeight = v);
        }
        else if (raySource != null)
        {
            titleText.text = "Источник лучей";
            AddIntSlider("Количество лучей", raySource.rayCount, v => raySource.rayCount = v);
            AddFloatSlider("Высота источника", raySource.sourceHeight, v => raySource.sourceHeight = v);
            AddFloatSlider("Ширина источника", raySource.sourceWidth, v => raySource.sourceWidth = v);
        }
    }

    void AddFloatSlider(string label, float value, System.Action<float> onChanged)
    {
        GameObject go = new GameObject(label);
        go.transform.SetParent(slidersContainer, false);

        HorizontalLayoutGroup layout = go.AddComponent<HorizontalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.spacing = 10;

        Text labelText = go.AddComponent<Text>();
        labelText.text = label;
        labelText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        labelText.fontSize = 14;
        labelText.color = Color.white;

        Slider slider = go.AddComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 3;
        slider.value = value;
        slider.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(onChanged));

        Text valueText = go.AddComponent<Text>();
        valueText.text = value.ToString("F2");
        valueText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        valueText.fontSize = 14;
        valueText.color = Color.white;

        slider.onValueChanged.AddListener(v => valueText.text = v.ToString("F2"));
    }

    void AddIntSlider(string label, int value, System.Action<int> onChanged)
    {
        AddFloatSlider(label, value, v => onChanged(Mathf.RoundToInt(v)));
    }
}