using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;


public class SceneLoadManagers : MonoBehaviour, ISaveManager
{
    public static SceneLoadManagers Instance;
    public GameSceneSO firstLoadScene;
    public UIFadeScreen fadeScreen;
    public GameObject fadeContainer;
    public SceneLoadEventSO sceneLoadEvent;

    [SerializeField] private GameSceneSO currentActiveScene;
    private GameSceneSO _targetScene;
    private Vector3 _spawnPosition;
    private bool _showLoadScreen;
    private bool _isSubscribed = false;
    private AsyncOperationHandle<GameSceneSO> _sceneHandle;
    private const string CatalogName = "catalog_name"; // Move to config if needed

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(InitializeAddressables());
    }
    void Update()
    {
      //  Debug.Log(_targetScene);
    }

    private IEnumerator InitializeAddressables()
    {
        fadeContainer.SetActive(true);
        //var initHandle = Addressables.InitializeAsync();
        //yield return initHandle;

        //if (!Addressables.ResourceLocators.Any())
        //{
        //    var catalogHandle = Addressables.LoadContentCatalogAsync(CatalogName);
        //    yield return catalogHandle;
        //}
        var initHandle = Addressables.InitializeAsync();
        yield return initHandle;

        // 检查资源定位器是否加载
        if (!Addressables.ResourceLocators.Any())
        {
            Debug.Log("Loading content catalog...");
            var catalogHandle = Addressables.LoadContentCatalogAsync(CatalogName);
            yield return catalogHandle;

            if (catalogHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError("Failed to load catalog!");
                yield break;
            }
        }
            if (currentActiveScene == null)
            {
                currentActiveScene = firstLoadScene;
            }

            fadeScreen.FadeOut();
            StartCoroutine(UnloadStartScene());
        
    }

    private void OnEnable()
    {
        // 避免重复订阅
        if (!_isSubscribed)
        {
            sceneLoadEvent.OnSceneLoad += HandleSceneLoadRequest;
            _isSubscribed = true;
        }
    }

    private void OnDisable()
    {
        if (_isSubscribed)
        {
            sceneLoadEvent.OnSceneLoad -= HandleSceneLoadRequest;
            _isSubscribed = false;
        }
        // 释放资源（在对象销毁时）
        if (_sceneHandle.IsValid())
        {
            Addressables.Release(_sceneHandle);
        }
    }

    private void HandleSceneLoadRequest(GameSceneSO scene, Vector3 position, bool showLoadScreen)
    {
       
        _targetScene = scene;
        _spawnPosition = position;
        _showLoadScreen = showLoadScreen;
       StartCoroutine(UnloadCurrentScene());
    }

    private IEnumerator UnloadStartScene()
    {
        fadeContainer.SetActive(true);
        yield return new WaitForSeconds(0.3f);

        var loadOperation = currentActiveScene.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        yield return loadOperation;

        AudioMgr.Instance.PlayMusic(currentActiveScene.sceneBGM.ToString());
        StartCoroutine(InitializeCameraBounds());

        yield return new WaitForSeconds(1.5f);
        fadeContainer.SetActive(false);
        fadeScreen.FadeIn();
    }

    private IEnumerator UnloadCurrentScene()
    {
        // 缓存当前目标场景
        GameSceneSO targetScene = _targetScene;
        fadeScreen.FadeOut();
        yield return new WaitForSeconds(0.5f);
        fadeContainer.SetActive(true);

        if (currentActiveScene != null)
        {
            
            yield return currentActiveScene.SceneReference.UnLoadScene();
        }
     
        // 加载新场景
        var loadOperation = targetScene.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        yield return loadOperation;     
        HandleSceneLoadComplete(loadOperation, targetScene);
       
        yield return CompleteTransition();
      
    }

    private void HandleSceneLoadComplete(AsyncOperationHandle<SceneInstance> operation, GameSceneSO targetScene)
    {
        if (operation.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"Scene load failed: {operation.DebugName}");
            return;
        }
        currentActiveScene = targetScene;

        PlayerManager.instance.player.transform.position = _spawnPosition;
        AudioMgr.Instance.PlayMusic(_targetScene.sceneBGM.ToString());

        StartCoroutine(CompleteTransition());
    }

    private IEnumerator CompleteTransition()
    {
        StartCoroutine(InitializeCameraBounds());
        yield return new WaitForSeconds(1.2f);
        fadeContainer.SetActive(false);
        fadeScreen.FadeIn();
    }

    public void LoadData(GameData data)
    {
      //  currentActiveScene = data?.LoadGameScene() ?? firstLoadScene;
    }

    public void SaveData(ref GameData data)
    {
        if (currentActiveScene != null)
        {
           // data.SaveGameScene(currentActiveScene);
        }
    }

    private IEnumerator InitializeCameraBounds()
    {
        yield return new WaitForSeconds(0.5f);
        // CameraController.Instance.UpdateCameraBounds();
    }
}
