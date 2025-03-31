using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadSceneUI : BasePanel
{
    [SerializeField] private Text loadValue;
    [SerializeField] private Text loadTip;
    [SerializeField] private Slider loadSlider;
    public float fadeDuration = 1.0f; // 淡出时长
    private CanvasGroup canvasGroup;
    private void Awake()
    {
        OpenPanel(UIConst.LoadScene);
        canvasGroup = GetComponent<CanvasGroup>();
        loadValue = transform.GetChild(1).GetChild(0).GetComponent<Text>();
        loadSlider = transform.GetChild(1).GetChild(1).GetComponent<Slider>();
        loadTip = transform.GetChild(1).GetChild(2).GetComponent<Text>();
       // StartCoroutine(Fade(1));
    }

   
    private void OnDestroy()
    {
        ClosePanel();
    }
    public void LoadLevels(AsyncOperationHandle<SceneInstance> handle)
    {
        StartCoroutine(LoadLevel(handle));
    }

    IEnumerator LoadLevel(AsyncOperationHandle<SceneInstance> handle)
    {
        Debug.Log("开始加载场景...");
       // yield return StartCoroutine(Fade(1));
        // 阶段1：等待资源加载完成
        while (!handle.IsDone)
        {
            // 计算映射后的进度（0.0~0.9 → 0%~90%）
            float progress = Mathf.Clamp01(handle.PercentComplete / 0.9f);
            loadSlider.value = progress;
            loadValue.text = (progress * 100).ToString("F0") + "%";
            Debug.Log($"资源加载进度: {handle.PercentComplete}");
            yield return null;
        }

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"资源加载失败: {handle.OperationException}");
            yield break;
        }

        // 阶段2：等待用户激活场景
        SceneInstance sceneInstance = handle.Result;
        AsyncOperation activationOp = sceneInstance.ActivateAsync();
        activationOp.allowSceneActivation = false;

        loadSlider.value = 1;
        loadValue.text = "100%";
        loadTip.text = "加载完成\n按任意键继续";
        Debug.Log("资源加载完成，等待用户输入...");

        // 添加超时机制（5秒后自动激活）
        float timeout = 5f;
        float timer = 0;
        bool isActivated = false;

        while (!isActivated && timer < timeout)
        {
            timer += Time.deltaTime;

            if (Input.anyKeyDown && !isActivated)
            {
                Debug.Log("检测到用户输入，激活场景");
                activationOp.allowSceneActivation = true;
                isActivated = true;
                // 设置活动场景
                SceneManager.SetActiveScene(sceneInstance.Scene);
                Debug.Log("场景已激活");
                //ClosePanel();
                //if(SceneLoadManager.Instance!=null)
                //{
                //    SceneLoadManager.Instance.fadeScreen.FadeIn();
                //}
                StartCoroutine(Fade(0));
            }
            yield return null;
        }

        // 超时处理
        if (timer >= timeout && !isActivated)
        {
            Debug.LogWarning("超时未操作，强制激活场景");
            activationOp.allowSceneActivation = true;
            yield return activationOp;
            // 设置活动场景
            SceneManager.SetActiveScene(sceneInstance.Scene);
            Debug.Log("场景已激活");
            //ClosePanel();
            StartCoroutine(Fade(0));
        }

       

    }
    IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0;

        while (time < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
        if (targetAlpha == 0)
        {
            ClosePanel();
        }
    }
}
