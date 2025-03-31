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
    public float fadeDuration = 1.0f; // ����ʱ��
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
        Debug.Log("��ʼ���س���...");
       // yield return StartCoroutine(Fade(1));
        // �׶�1���ȴ���Դ�������
        while (!handle.IsDone)
        {
            // ����ӳ���Ľ��ȣ�0.0~0.9 �� 0%~90%��
            float progress = Mathf.Clamp01(handle.PercentComplete / 0.9f);
            loadSlider.value = progress;
            loadValue.text = (progress * 100).ToString("F0") + "%";
            Debug.Log($"��Դ���ؽ���: {handle.PercentComplete}");
            yield return null;
        }

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"��Դ����ʧ��: {handle.OperationException}");
            yield break;
        }

        // �׶�2���ȴ��û������
        SceneInstance sceneInstance = handle.Result;
        AsyncOperation activationOp = sceneInstance.ActivateAsync();
        activationOp.allowSceneActivation = false;

        loadSlider.value = 1;
        loadValue.text = "100%";
        loadTip.text = "�������\n�����������";
        Debug.Log("��Դ������ɣ��ȴ��û�����...");

        // ��ӳ�ʱ���ƣ�5����Զ����
        float timeout = 5f;
        float timer = 0;
        bool isActivated = false;

        while (!isActivated && timer < timeout)
        {
            timer += Time.deltaTime;

            if (Input.anyKeyDown && !isActivated)
            {
                Debug.Log("��⵽�û����룬�����");
                activationOp.allowSceneActivation = true;
                isActivated = true;
                // ���û����
                SceneManager.SetActiveScene(sceneInstance.Scene);
                Debug.Log("�����Ѽ���");
                //ClosePanel();
                //if(SceneLoadManager.Instance!=null)
                //{
                //    SceneLoadManager.Instance.fadeScreen.FadeIn();
                //}
                StartCoroutine(Fade(0));
            }
            yield return null;
        }

        // ��ʱ����
        if (timer >= timeout && !isActivated)
        {
            Debug.LogWarning("��ʱδ������ǿ�Ƽ����");
            activationOp.allowSceneActivation = true;
            yield return activationOp;
            // ���û����
            SceneManager.SetActiveScene(sceneInstance.Scene);
            Debug.Log("�����Ѽ���");
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
