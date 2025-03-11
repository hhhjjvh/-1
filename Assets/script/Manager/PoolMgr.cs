
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;


[DefaultExecutionOrder(-100)]
public class SingletonMonoGlobal<T> : MonoBehaviour, ISingleton where T : MonoBehaviour
{
    private static T _instance;
    // <summary>
    // 线程锁
    // </summary>
    private static readonly object _lock = new object();
    // <summary>
    // 程序是否正在退出
    // </summary>
    protected static bool ApplicationIsQuitting { get; private set; }

    static SingletonMonoGlobal()
    {
        ApplicationIsQuitting = false;
    }

    public static T Instance
    {
        get
        {
            if (ApplicationIsQuitting)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.LogWarning("[Singleton] " + typeof(T) +
                                            " 已经销毁，不允许访问");
                }

                // return null;
                return _instance;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    // 先在场景中找寻
                    _instance = FindFirstObjectByType<T>();

                    T[] oldInstances = FindObjectsOfType<T>(true);
                    if (oldInstances.Length > 1)
                    {
                        if (Debug.isDebugBuild)
                        {
                            Debug.LogWarning("[Singleton] " + typeof(T).Name + " should never be more than 1 in scene!");
                        }

                        return _instance;
                    }

                    // 场景中找不到就创建新物体挂载
                    if (_instance == null)
                    {
                        GameObject singletonObj = new();
                        _instance = singletonObj.AddComponent<T>();
                        singletonObj.name = "(singleton) " + typeof(T);

                        if (Application.isPlaying)
                        {
                            DontDestroyOnLoad(singletonObj);
                        }

                        return _instance;
                    }
                }

                return _instance;
            }
        }
    }

    // <summary>
    // 当工程运行结束，在退出时，不允许访问单例
    // </summary>
    protected virtual void OnApplicationQuit()
    {
        ApplicationIsQuitting = true;
    }
    // protected virtual void OnDestroy()
    // {
    // ApplicationIsQuitting = true;
    // }
}
public interface ISingleton
{

}



public class PoolMgr : SingletonMonoGlobal<PoolMgr>
{
    // 新增的 Sprite 缓存字典
    private Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();
    // 对象池字典，按名字存储
    private Dictionary<string, ObjectPool<GameObject>> poolDic = new();
    private Dictionary<string, Transform> poolParents = new();
    private Dictionary<string, GameObject> prefabs = new();

    public Dictionary<string, PoolData> prePoolDic = new();
    public Transform prefabsParent;
    void Start()
    {
        //StartCoroutine(PreloadAsync("FX024_1", preloadCount: 20, onProgress: progress =>
        //{
        //    //Debug.Log($"FX024_1 pool preload progress: {progress * 100}%");
        //}));
      
    }
 
  protected override void OnApplicationQuit()
    {
        ClearSpriteCache();
        Clear();
    }
    /// <summary>
    /// 根据名称加载并获取 Sprite 资源
    /// </summary>
    public Sprite GetSprite(string spriteName)
    {
        // 检查缓存
        if (spriteCache.TryGetValue(spriteName, out Sprite cachedSprite))
        {
            return cachedSprite;
        }

        // 加载 Sprite
        try
        {
            Sprite loadedSprite = AAMgr.LoadAsset<Sprite>(spriteName);
            if (loadedSprite != null)
            {
                spriteCache.Add(spriteName, loadedSprite);
                return loadedSprite;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"加载图片失败：{spriteName}\n错误信息：{e.Message}");
        }

        // 加载失败时返回 null 或默认图片
        return null;
    }

    /// <summary>
    /// 清空 Sprite 缓存
    /// </summary>
    public void ClearSpriteCache()
    {
        spriteCache.Clear();
    }
    /// <summary>
    /// 创建或获取对象池
    /// </summary>
    private ObjectPool<GameObject> CreateOrGetPool(string objName, GameObject prefab, int initialCapacity = 10, int maxCapacity = 100, int preloadCount = 0)
    {
        if (poolDic.ContainsKey(objName))
        {
            return poolDic[objName];
        }

        // 创建父对象容器
        Transform parent = new GameObject($"Pool_{objName}").transform;
        parent.SetParent(transform);
        poolParents[objName] = parent;

        // 创建对象池
        var newPool = new ObjectPool<GameObject>(
            createFunc: () =>
            {
                var newObj = Instantiate(prefab, parent);
                newObj.name = objName;
                return newObj;
            },
            actionOnGet: obj =>
            {
                obj.SetActive(true);
            },
            actionOnRelease: obj =>
            {
                obj.SetActive(false);
                obj.transform.SetParent(parent);
            },
            actionOnDestroy: obj =>
            {
                Destroy(obj);
            },
            defaultCapacity: initialCapacity,
            maxSize: maxCapacity
        );

        // 预加载
        for (int i = 0; i < preloadCount; i++)
        {
            var obj = newPool.Get();
            newPool.Release(obj);
        }

        poolDic.Add(objName, newPool);
        // Debug.Log($"Created pool for {objName} with initial capacity {initialCapacity} and max capacity {maxCapacity}."+ newPool);
        return newPool;
    }

    public void Preload(string objName, int preloadCount, int initialCapacity = 10, int maxCapacity = 100)
    {
        var prefab = GetPrefab(objName);
        if (prefab == null)
        {
            Debug.LogError($"Prefab {objName} not found.");
            return;
        }

        CreateOrGetPool(objName, prefab, initialCapacity, maxCapacity, preloadCount);
    }


    /// <summary>
    /// 获取对象
    /// </summary>
    public GameObject GetObj(string objName, Vector3 position = default, Quaternion rotation = default, int initialCapacity = 10, int maxCapacity = 100)
    {
        // 确保预制体已加载
        var prefab = GetPrefab(objName);
        if (prefab == null)
        {
            Debug.LogError($"Prefab {objName} not found.");
            return null;
        }

        // 创建或获取池
        var pool = CreateOrGetPool(objName, prefab, initialCapacity, maxCapacity);
        //if (pool == null|| pool.CountAll == 0) return null;
        GameObject obj = pool.Get();
        obj.transform.SetPositionAndRotation(position, rotation);
        return obj;
    }

    /// <summary>
    /// 回收对象
    /// </summary>
    public void Release(GameObject obj)
    {
        if (poolDic.ContainsKey(obj.name))
        {
            //Debug.Log("对象数量" + poolDic[obj.name].CountAll);
            if (obj.activeSelf == false)
            {
                string objName = obj.name;
                Debug.LogWarning($"对象'{objName}' 已经回收.");
                return;
            }
            //Transform parent = new GameObject(obj.name).transform;
            //parent.SetParent(transform);
            obj.transform.SetParent(poolParents[obj.name]);
            poolDic[obj.name].Release(obj);
        }
        else if (prePoolDic.ContainsKey(obj.name))
        {
            //Debug.Log("对象数量2213" + prePoolDic[obj.name]);
            //Transform parent = new GameObject(obj.name).transform;
            //parent.SetParent(transform);
            prePoolDic[obj.name].Release(obj);
        }
        else
        {
            Destroy(obj);
        }
    }
    public void Release(GameObject obj, float delay)
    {
        if (obj == null)
        {
            Debug.LogWarning("Attempted to release a null object.");
            return;
        }
        // 开启协程处理延迟回收
        StartCoroutine(DelayedReleaseCoroutine(obj, delay));
    }

    // 协程：延迟回收
    private IEnumerator DelayedReleaseCoroutine(GameObject obj, float delay)
    {

        // 等待指定的延迟时间
        yield return new WaitForSeconds(delay);
        // 检查对象是否有效
        if (obj == null)
        {
           // Debug.LogWarning("Attempted to release a null object.");
            yield break;
        }
        if (poolDic.ContainsKey(obj.name))
        {
            //Debug.Log("对象数量" + poolDic[obj.name].CountAll);
            if (obj.activeSelf == false)
            {
                string objName = obj.name;
                Debug.LogWarning($"对象'{objName}' 已经回收.");
                yield break;

            }
            obj.transform.SetParent(poolParents[obj.name]);
            poolDic[obj.name].Release(obj);
        }
        else if (prePoolDic.ContainsKey(obj.name))
        {
            prePoolDic[obj.name].Release(obj);
        }
        else
        {
            Destroy(obj);
        }
    }


    /// <summary>
    /// 清理所有对象池
    /// </summary>
    public void Clear()
    {
        foreach (var pool in poolDic)
        {
            pool.Value.Clear();
        }
        poolDic.Clear();

        foreach (var parent in poolParents.Values)
        {
            Destroy(parent.gameObject);
        }
        poolParents.Clear();

        prefabs.Clear();

        foreach (var pool in prePoolDic)
        {
            pool.Value.Clear();
        }
        prePoolDic.Clear();
    }

    /// <summary>
    /// 加载预制体
    /// </summary>
    private GameObject GetPrefab(string objName)
    {
        GameObject prefab = null;
        //如果已经加载过该预设体
        if (prefabs.ContainsKey(objName))
        {
            prefab = prefabs[objName];
        }
        else     //如果没有加载过该预设体
        {
            //加载预设体
            // prefab = Resources.Load<GameObject>(objName);
            try
            {
                prefab = AAMgr.LoadAsset<GameObject>(objName);
                prefab.name = objName;
                //更新字典
                prefabs.Add(objName, prefab);
                // prefab.SetActive(false);
                prefab.gameObject?.SetActive(false);
                prefab.transform.SetParent(prefabsParent);

            }
            catch
            {
                Debug.LogError("加载预设体失败：" + objName);
            }
        }
        return prefab;
    }

    /// <summary>
    /// 获取池的统计信息
    /// </summary>
    public void PrintPoolStats()
    {
        foreach (var pool in poolDic)
        {
            Debug.Log($"Pool: {pool.Key}, Active: {pool.Value.CountActive}, Inactive: {pool.Value.CountInactive}, Total: {pool.Value.CountAll}");
        }
    }
    public IEnumerator PreloadAsync(string objName, int preloadCount, int initialCapacity = 10, int maxCapacity = 100, Action<float> onProgress = null)
    {
        var prefab = GetPrefab(objName);
        if (prefab == null)
        {
            Debug.LogError($"Prefab {objName} not found.");
            yield break;
        }

        var pool = CreateOrGetPool(objName, prefab, initialCapacity, maxCapacity);

        for (int i = 0; i < preloadCount; i++)
        {
            var obj = pool.Get();
            pool.Release(obj);

            onProgress?.Invoke((i + 1) / (float)preloadCount);
            yield return null;
        }

        onProgress?.Invoke(1.0f);
       // Debug.Log($"PreloadAsync for {objName} completed.");
    }

}

#region  小对象池
public class PoolData
{
    /// <summary>
    /// 预制体的父节点，不是顶层
    /// </summary>
    public GameObject fatherObj;
    // 容器
    public Queue<GameObject> poolQueue;

    public int Count => poolQueue.Count;

    public PoolData(GameObject prefab, GameObject ancestor)
    {
        fatherObj = new GameObject(prefab.name);
        fatherObj.transform.SetParent(ancestor.transform);
        poolQueue = new Queue<GameObject>();
    }

    // 添加
    public void Push(GameObject obj)
    {
        obj.SetActive(false);
        poolQueue.Enqueue(obj);
        obj.transform.parent = fatherObj.transform;
    }

    //   释放
    public void Release(GameObject obj)
    {
        obj.SetActive(false);
        poolQueue.Enqueue(obj);
    }

    public GameObject Get()
    {
        GameObject obj = poolQueue.Dequeue();
        obj.SetActive(true);
        return obj;
    }

    public void Clear()
    {
        poolQueue.Clear();
    }
}
#endregion
// public class PoolMgr : SingletonMono<PoolMgr>
// {

// public Dictionary<String, PoolData> poolDic = new();

// private GameObject poolObj;

// public void GetObject(string name, UnityAction<GameObject> callback = null)
// {
//     if (poolDic.ContainsKey(name) && poolDic[name].poolList.Count > 0)
//     {
//         //池子有东西，可拿
//         callback(poolDic[name].GetObj());
//     }
//     else
//     {
//         //池子没有东西，创建
//         ResMgr.Instance.LoadAsync<GameObject>(name, (obj) =>
//         {
//             obj.name = name;
//             callback(obj);
//         });
//     }
// }

//存入
// public void PushObject(string name, GameObject obj)
// {
//     if (poolObj == null)
//         poolObj = new GameObject("PoolObj");

//     if (poolDic.ContainsKey(name))
//     {
//         poolDic[name].PushObj(obj);
//     }
//     else
//     {
//         poolDic.Add(name, new PoolData(obj, poolObj));
//     }
// }

// <summary>
// 场景切换，清空缓存池
// </summary>
// public void Clear()
// {
//     poolDic.Clear();
//     poolObj = null;
// }

