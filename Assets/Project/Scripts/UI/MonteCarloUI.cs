using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonteCarloUI : MonoBehaviour
{
    public Slider progressBar;
    public TMP_Text progressText;

    public void SetProgress(float t)
    {
        progressBar.value = t;

        progressText.text =
            Mathf.RoundToInt(t * 100f) + "%";
    }
}