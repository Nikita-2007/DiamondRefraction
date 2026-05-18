using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using System.Collections.Generic;

public class DynamicSettingsUI : MonoBehaviour
{
    public GameObject settingsPanel;    // главная панель
    public Text titleText;              // заголовок
    public GameObject sliderPrefab;     // префаб слайдера (текст + слайдер + значение)
    public GameObject panelPrefab;      // префаб для группировки (можно null)

    private GameObject currentContent;
    private object selectedObject;

    void Start()
    {
        settingsPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                ShowSettings(hit.collider.gameObject);
            }
            else
            {
                settingsPanel.SetActive(false);
            }
        }
    }

    void ShowSettings(GameObject obj)
    {
        selectedObject = obj.GetComponent<object>();
        if (selectedObject == null) return;

        settingsPanel.SetActive(true);
        titleText.text = obj.name;

        // Удаляем старый контент
        if (currentContent != null) Destroy(currentContent);

        // Создаём новую панель для контента
        currentContent = new GameObject("Content");
        currentContent.transform.SetParent(settingsPanel.transform, false);

        var layout = currentContent.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.spacing = 10;
        layout.padding = new RectOffset(15, 15, 15, 15);

        // Добавляем ContentSizeFitter для вертикального растяжения
        var fitter = currentContent.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Создаём ползунки для всех полей типа float
        foreach (FieldInfo field in selectedObject.GetType().GetFields())
        {
            if (field.FieldType == typeof(float))
            {
                CreateSlider(field.Name, (float)field.GetValue(selectedObject),
                    value => field.SetValue(selectedObject, value));
            }
        }

        // Создаём ползунки для всех свойств типа float
        foreach (PropertyInfo prop in selectedObject.GetType().GetProperties())
        {
            if (prop.PropertyType == typeof(float) && prop.CanRead && prop.CanWrite)
            {
                CreateSlider(prop.Name, (float)prop.GetValue(selectedObject),
                    value => prop.SetValue(selectedObject, value));
            }
        }
    }

    void CreateSlider(string label, float initialValue, System.Action<float> onValueChanged)
    {
        GameObject row = new GameObject($"Slider_{label}");
        row.transform.SetParent(currentContent.transform, false);

        var hLayout = row.AddComponent<HorizontalLayoutGroup>();
        hLayout.childAlignment = TextAnchor.MiddleCenter;
        hLayout.spacing = 10;

        // Текст
        Text labelText = CreateText(label);
        labelText.transform.SetParent(row.transform, false);

        // Слайдер
        Slider slider = CreateSliderComponent(initialValue);
        slider.transform.SetParent(row.transform, false);
        slider.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(onValueChanged));

        // Значение
        Text valueText = CreateText(initialValue.ToString("F2"));
        valueText.transform.SetParent(row.transform, false);
        slider.onValueChanged.AddListener(v => valueText.text = v.ToString("F2"));
    }

    Text CreateText(string content)
    {
        GameObject go = new GameObject("Text");
        Text text = go.AddComponent<Text>();
        text.text = content;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 14;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleLeft;
        return text;
    }

    Slider CreateSliderComponent(float initialValue)
    {
        GameObject go = new GameObject("Slider");
        Slider slider = go.AddComponent<Slider>();
        slider.minValue = 0;
        slider.maxValue = 5;
        slider.value = initialValue;

        // Добавляем фон
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(go.transform, false);
        var bgImg = bg.AddComponent<Image>();
        bgImg.color = Color.gray;

        // Добавляем ручку
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(go.transform, false);
        var handleImg = handle.AddComponent<Image>();
        handleImg.color = Color.white;
        slider.handleRect = handle.transform as RectTransform;
        slider.targetGraphic = handleImg;

        return slider;
    }
}