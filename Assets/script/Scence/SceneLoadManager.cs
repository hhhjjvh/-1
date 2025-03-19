using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using System.Linq;

public class SceneLoadManager : MonoBehaviour, ISaveManager
{
    public static SceneLoadManager Instance;
    public GameSceneSO firstLoadScene;
    public UIFadeScreen fadeScreen;
    public GameObject fade;

    public SceneLoadEventSO sceneLoadEvent;

    [SerializeField] private GameSceneSO currentSceneSo;
    private GameSceneSO ScenceToLoad;
    private  GameSceneSO _pendingSceneToLoad; // 新增字段
    private Vector3 positionToLoad;
    private bool isLoad;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null); // 清除父对象
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        fade.SetActive(true);
    }
   
    private void Start()
    {
        //  StartCoroutine(InitializeAddressables());
        if (currentSceneSo == null)
        {
            currentSceneSo = firstLoadScene;
        }

        StartCoroutine(UnLoadStartScene());
    }

    IEnumerator InitializeAddressables()
    {
        fade.SetActive(true);
        // 初始化 Addressables（如果未自动初始化）
        var initHandle = Addressables.InitializeAsync();
        yield return initHandle;

        // 确保资源定位表已加载
        if (!Addressables.ResourceLocators.Any())
        {
            var catalogHandle = Addressables.LoadContentCatalogAsync("catalog_name");
            yield return catalogHandle;
        }

        // 继续原有逻辑
        if (currentSceneSo == null)
        {
            currentSceneSo = firstLoadScene;
        }
        
        StartCoroutine(UnLoadStartScene());
    }
    private void OnEnable()
    {
        sceneLoadEvent.OnSceneLoad += OnScenceLoad;
       // Debug.Log("OnEnable");
    }
    private void OnDisable()
    {
        sceneLoadEvent.OnSceneLoad -= OnScenceLoad;
    }
    public GameSceneSO GetFirstLoadScence()
    {
        return firstLoadScene;
    }

    private void OnScenceLoad(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        ScenceToLoad = arg0;
        _pendingSceneToLoad = arg0; // 缓存目标场景
        Debug.Log(ScenceToLoad);
        positionToLoad = arg1;
        isLoad = arg2;
        StartCoroutine(UnLoadScence());
    }

    IEnumerator UnLoadStartScene()
    {
        fade.SetActive(true);
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(0.3f);
        // Debug.Log("Unload"+currentScenceSo);
        yield return currentSceneSo.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        AudioMgr.Instance.PlayMusic(currentSceneSo.sceneBGM.ToString());
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
        fade.SetActive(true);
        if (currentSceneSo != null)
        {
            yield return currentSceneSo.SceneReference.UnLoadScene();
        }
        LoadNewScence();
        StartCoroutine(SetCameraBounds());
        yield return new WaitForSeconds(1.2f);
        fade.SetActive(false);
        fadeScreen.FadeIn();

    }
    private void LoadNewScence()
    {
        GameSceneSO _pendingSceneToLoad = new GameSceneSO { }; 
        _pendingSceneToLoad = ScenceToLoad;
        if (_pendingSceneToLoad == null)
        {
            Debug.LogError("Scene to load is null!");
            return;
        }

        var load = _pendingSceneToLoad.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        load.Completed += OnLoadComplete;
    }

    private void OnLoadComplete(AsyncOperationHandle<SceneInstance> handle)
    {
        Debug.Log(_pendingSceneToLoad);
        currentSceneSo = _pendingSceneToLoad;
        PlayerManager.instance.player.transform.position = positionToLoad;
        AudioMgr.Instance.PlayMusic(ScenceToLoad.sceneBGM.ToString());
    }

    public void LoadData(GameData data)
    {
        if (data == null || data.LoadGameScene() == null)
        {
            currentSceneSo = firstLoadScene;
            return;
        }
        currentSceneSo = data.LoadGameScene();

    }

    public void SaveData(ref GameData data)
    {
        if (currentSceneSo == null || Instance == null) return;
        data.SaveGameScene(currentSceneSo);
    }
    IEnumerator SetCameraBounds()
    {
        yield return new WaitForSeconds(0.5f);
        // CameraControl.Instance.GetNewCameraBounds();
    }
}
