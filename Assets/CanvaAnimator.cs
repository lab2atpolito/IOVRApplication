using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvaAnimator : MonoBehaviour
{
    private CanvasGroup _canvasGroup;
    public float duration = 0.2f;
    public bool visibleOnStart; 

    void Start()
    {
        _canvasGroup = this.GetComponent<CanvasGroup>();
        if( !visibleOnStart )
        {
            _canvasGroup.alpha = 0f;
        }
        else
        {
            _canvasGroup.alpha = 1f;
        }
    }

    public void ActivateCanvas()
    {
        StartCoroutine(FadeInCoroutine());
    }

    public void HideCanvas()
    {
        StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        float elapsedTime = 0f;
        float startAlpha = _canvasGroup.alpha;
        float targetAlpha = 0f;

        while (elapsedTime < duration)
        {
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            _canvasGroup.alpha = newAlpha;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _canvasGroup.alpha = targetAlpha;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    private IEnumerator FadeInCoroutine()
    {
        float elapsedTime = 0f;
        float startAlpha = _canvasGroup.alpha;
        float targetAlpha = 1f;

        while (elapsedTime < duration)
        {
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            _canvasGroup.alpha = newAlpha;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _canvasGroup.alpha = targetAlpha;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }
}
