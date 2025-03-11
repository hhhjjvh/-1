using System.Collections;
using UnityEngine;

public class UIFadeScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeSpeed = 1f;


    public void FadeOut(float duration = 1f)
    {
        canvasGroup.gameObject.SetActive(true);
        StartCoroutine(FadeOutCoroutine(duration));
    }
    IEnumerator FadeOutCoroutine(float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0;
        while (elapsed < duration)
        {

            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;

        }
        canvasGroup.alpha = 1;
    }


    public void FadeIn(float duration = 1f)
    {
        StartCoroutine(FadeInCoroutine(duration));
       
    }
    IEnumerator FadeInCoroutine(float duration)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0;
        while (elapsed < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
        canvasGroup.gameObject.SetActive(false);
    }
}
