using System.Collections;
using UnityEngine;
using TMPro;

public class ScorePopup : MonoBehaviour
{
    public TMP_Text popupText;
    public CanvasGroup canvasGroup;

    public float floatDistance = 80f;
    public float duration = 0.8f;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Setup(int points)
    {
        popupText.text = "+" + points.ToString();

        StartCoroutine(FloatRoutine());
    }

    private IEnumerator FloatRoutine()
    {
        float elapsed = 0f;

        Vector2 startPosition = rectTransform.anchoredPosition;
        Vector2 endPosition = startPosition + Vector2.up * floatDistance;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / duration;

            rectTransform.anchoredPosition =
                Vector2.Lerp(startPosition, endPosition, t);

            canvasGroup.alpha = 1f - t;

            yield return null;
        }

        Destroy(gameObject);
    }
}