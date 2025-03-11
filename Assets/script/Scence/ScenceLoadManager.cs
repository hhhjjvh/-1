using System.Collections;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class ScenceLoadManager : MonoBehaviour, ISaveManager
{
    public static ScenceLoadManager instance;
    public GameSceneSO firstLoadScence;
    public UIFadeScreen fadeScreen;
    public GameObject fade;

    public SceneLoadEventSO eventSo;

    private GameSceneSO currentScenceSo;
    private GameSceneSO ScenceToLoad;
    private Vector3 positionToLoad;
    private bool isLoad;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }
    void Start()
    {
        if (currentScenceSo == null)
        {
            currentScenceSo = firstLoadScence;
        }
        fadeScreen.FadeOut();   
        StartCoroutine(UnLoadStartScence());
    }
    private void OnEnable()
    {
        eventSo.OnSceneLoad += OnScenceLoad;
    }
    private void OnDisable()
    {
        eventSo.OnSceneLoad -= OnScenceLoad;
    }
    public GameSceneSO GetFirstLoadScence()
    {
        return firstLoadScence;
    }

    private void OnScenceLoad(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        ScenceToLoad = arg0;
        positionToLoad = arg1;
        isLoad = arg2;
        StartCoroutine(UnLoadScence());
    }

    IEnumerator UnLoadStartScence()
    {
        fade.SetActive(true);
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(0.3f);
        // Debug.Log("Unload"+currentScenceSo);
        yield return currentScenceSo.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        AudioMgr.Instance.PlayMusic(currentScenceSo.scenceBGM.ToString());
        //GameManager.instance.SetPlayerPosition();
        StartCoroutine(SetCameraBounds());
        yield return new WaitForSeconds(1.5f);
        fadeScreen.FadeIn();
        fade.SetActive(false);
    }

    IEnumerator UnLoadScence()
    {
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(0.5f);
        //SaveManager.instance.SaveGame();

        if (currentScenceSo != null)
        {
            yield return currentScenceSo.SceneReference.UnLoadScene();
        }
        LoadNewScence();
        // CameraControl.Instance.GetNewCameraBounds();
        // SaveManager.instance.LoadGame();
        StartCoroutine(SetCameraBounds());
        yield return new WaitForSeconds(1.2f);
        fadeScreen.FadeIn();
    }
    private void LoadNewScence()
    {
        var load = ScenceToLoad.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        load.Completed += OnLoadComplete;
    }

    private void OnLoadComplete(AsyncOperationHandle<SceneInstance> handle)
    {
        currentScenceSo = ScenceToLoad;
        PlayerManager.instance.player.transform.position = positionToLoad;
        AudioMgr.Instance.PlayMusic(ScenceToLoad.scenceBGM.ToString());
    }

    public void LoadData(GameData data)
    {
        if (data == null || data.LoadGameScene() == null)
        {
            currentScenceSo = firstLoadScence;
            return;
        }
        currentScenceSo = data.LoadGameScene();

    }

    public void SaveData(ref GameData data)
    {
        if (currentScenceSo == null || instance == null) return;
        data.SaveGameScene(currentScenceSo);
    }
    IEnumerator SetCameraBounds()
    {
        yield return new WaitForSeconds(0.5f);
        // CameraControl.Instance.GetNewCameraBounds();
    }
}
