using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CGManager : MonoBehaviour
{
    public static CGManager Instance;
    [SerializeField] private Image cgImage;
    [SerializeField] private CanvasGroup cgCanvasGroup;
    public GameObject bl ;


    private void Awake()
    {
        Instance = this;
        cgImage.gameObject.SetActive(false);
    }

    // 基础显示方法
    public void ShowCG(string imagePath)
    {
        cgCanvasGroup.alpha = 1;
        Sprite icon = PoolMgr.Instance.GetSprite(imagePath);
        if (icon != null)
        {
            cgImage.sprite = icon;
        }
       
        cgImage.gameObject.SetActive(true);
    }

    public void HideCG()
    {
        cgImage.gameObject.SetActive(false);
        bl.SetActive(false);
    }
    public void ShowCGWithFade(string imagePath, float fadeTime = 1.0f,UnityAction onComplete = null)
    {
        bl.SetActive(true);
        StartCoroutine(FadeIn(imagePath, fadeTime, onComplete));
    }

    private IEnumerator FadeIn(string imagePath, float fadeTime, UnityAction onComplete = null)
    {
        ShowCG(imagePath);
        cgCanvasGroup.alpha = 0;
        float timer = 0;
        while (timer < fadeTime)
        {
            cgCanvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeTime);
            timer += Time.deltaTime;
            yield return null;
        }
        cgCanvasGroup.alpha = 1;
     onComplete?.Invoke();
        bl.SetActive(true);
    }
}