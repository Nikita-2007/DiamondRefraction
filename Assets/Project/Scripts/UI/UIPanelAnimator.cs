using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class UIPanelAnimator : MonoBehaviour
{
    public float speed = 8f;

    private CanvasGroup cg;
    private RectTransform rt;

    private Vector2 visiblePos;
    private Vector2 hiddenPos;

    void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        rt = GetComponent<RectTransform>();

        visiblePos = rt.anchoredPosition;
        hiddenPos = visiblePos + new Vector2(-350f, 0f);

        HideInstant();
    }

    public void Show()
    {
        StopAllCoroutines();

        cg.interactable = true;
        cg.blocksRaycasts = true;

        StartCoroutine(Animate(true));
    }

    public void Hide()
    {
        StopAllCoroutines();

        cg.interactable = false;
        cg.blocksRaycasts = false;

        StartCoroutine(Animate(false));
    }

    void HideInstant()
    {
        cg.alpha = 0f;

        cg.interactable = false;
        cg.blocksRaycasts = false;

        rt.anchoredPosition = hiddenPos;
    }

    IEnumerator Animate(bool show)
    {
        float targetAlpha = show ? 1f : 0f;
        Vector2 targetPos = show ? visiblePos : hiddenPos;

        while (
            Mathf.Abs(cg.alpha - targetAlpha) > 0.01f ||
            Vector2.Distance(rt.anchoredPosition, targetPos) > 0.5f
        )
        {
            cg.alpha = Mathf.Lerp(
                cg.alpha,
                targetAlpha,
                Time.deltaTime * speed
            );

            rt.anchoredPosition = Vector2.Lerp(
                rt.anchoredPosition,
                targetPos,
                Time.deltaTime * speed
            );

            yield return null;
        }

        cg.alpha = targetAlpha;
        rt.anchoredPosition = targetPos;
    }
}