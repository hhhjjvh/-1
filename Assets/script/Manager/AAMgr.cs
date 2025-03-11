using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AAMgr
{
    // <summary>
    // 需要确保key唯一
    // </summary>
    private static Dictionary<string, AsyncOperationHandle> loadedAssets = new();

    #region 单个加载
    public static void LoadAssetAsync<T>(string key, Action<T> onSuccess, Action<Exception> onFailure = null) where T : UnityEngine.Object
    {
        if (loadedAssets.ContainsKey(key))
        {
            // if (loadedAssets[key].Result is GameObject)
            //     onSuccess?.Invoke(GameObject.Instantiate((T)loadedAssets[key].Result));
            // else
            onSuccess?.Invoke((T)loadedAssets[key].Result);
            return;
        }

        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
        handle.Completed += op =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                loadedAssets[key] = op;
                // if (op.Result is GameObject)
                //     onSuccess?.Invoke(GameObject.Instantiate(op.Result));
                // else
                onSuccess?.Invoke(op.Result);
            }
            else
            {
                onFailure?.Invoke(op.OperationException);
                Debug.LogError($"Failed to load asset: {key}");
            }
        };
    }
    public static async Task<T> LoadAssetAsync<T>(string key) where T : UnityEngine.Object
    {
        if (loadedAssets.ContainsKey(key))
        {
            return (T)loadedAssets[key].Result;
        }

        try
        {
            // 异步加载资源
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
            await handle.Task;  // 等待异步任务完成

            // 资源加载成功后，缓存并返回
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                loadedAssets[key] = handle;

                // if (handle.Result is GameObject)
                //     return GameObject.Instantiate(handle.Result);
                // else
                return handle.Result;
            }
            else
            {
                Debug.LogError($"加载失败: {key}");
                return null;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"发生异常: {ex.Message}");
            return null;
        }
    }

    //同步
    public static T LoadAsset<T>(string key) where T : UnityEngine.Object
    {
       // Debug.Log("加载资源" + key);
        T res;
        if (loadedAssets.ContainsKey(key))
        {
            res = (T)loadedAssets[key].Result;
        }
        else
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
            loadedAssets[key] = handle;
            res = handle.WaitForCompletion();
        }

        if (res is GameObject)
        {
            return GameObject.Instantiate(res) as T;
        }
        //Debug.Log("加载资源" + res);

        return res;
    }
    #endregion

    #region  加载多个
    private static void BaseLoadAssetsAsync<T>(IEnumerable<string> keys, Action<IList<T>> onSuccess = null, Action<Exception> onFailure = null)
    {
        List<T> results = new List<T>();
        List<string> unloadedKeys = CheckUnloadedKeys(keys);

        if (unloadedKeys.Count == 0)
        {
            onSuccess?.Invoke(results);
            return;
        }

        AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(unloadedKeys, null, Addressables.MergeMode.Union);

        handle.Completed += op =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                foreach (var key in unloadedKeys)
                {
                    loadedAssets[key] = op;
                }
                results.AddRange(op.Result);
                onSuccess?.Invoke(results);
            }
            else
            {
                onFailure?.Invoke(op.OperationException);
                Debug.LogError("AA异步资源加载失败");
            }
        };
    }

    public static void LoadAssetsAsync<T>(IEnumerable<string> keys, Action<IList<T>> onSuccess, Action<Exception> onFailure = null)
    {
        BaseLoadAssetsAsync<T>(keys, onSuccess, onFailure);
    }

    public static void LoadAssetsWithTagAsync<T>(string tag, Action<IList<T>> onSuccess, Action<Exception> onFailure = null)
    {
        var locationHandle = Addressables.LoadResourceLocationsAsync(tag);

        locationHandle.Completed += locOp =>
        {
            if (locOp.Status == AsyncOperationStatus.Succeeded)
            {
                var keys = locOp.Result.Select(location => location.PrimaryKey);
                BaseLoadAssetsAsync<T>(keys, onSuccess, onFailure);
            }
            else
            {
                onFailure?.Invoke(locOp.OperationException);
                Debug.LogError("Failed to load resource locations with tag: " + tag);
            }
        };
    }

    #region  async

    public static async Task<IList<T>> BaseLoadAssetsAsync<T>(IEnumerable<string> keys)
    {
        List<T> results = new List<T>();
        List<string> unloadedKeys = CheckUnloadedKeys(keys);

        if (unloadedKeys.Count == 0)
        {
            return results;
        }

        var handle = Addressables.LoadAssetsAsync<T>(unloadedKeys, null, Addressables.MergeMode.Union);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var key in unloadedKeys)
            {
                loadedAssets[key] = handle;
            }
            results.AddRange(handle.Result);
            return results;
        }
        else
        {
            Debug.LogError("AA异步资源加载失败");
            throw handle.OperationException;
        }
    }

    public static async Task<IList<T>> LoadAssetsWithTagAsync<T>(string tag)
    {
        var resourceLocations = await Addressables.LoadResourceLocationsAsync(tag).Task;
        var keys = resourceLocations.Select(location => location.PrimaryKey);
        return await BaseLoadAssetsAsync<T>(keys);
    }

    #endregion

    //同步
    public static List<T> BaseLoadAssets<T>(IEnumerable<string> keys)
    {
        List<T> results = new List<T>();
        List<string> unloadedKeys = CheckUnloadedKeys(keys);

        // 如果所有资源都已加载，直接返回结果
        if (unloadedKeys.Count == 0)
        {
            return results;
        }

        // 加载未加载的资源
        var handle = Addressables.LoadAssetsAsync<T>(unloadedKeys, null, Addressables.MergeMode.Union);
        handle.WaitForCompletion(); // 同步等待加载完成

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            foreach (var key in unloadedKeys)
            {
                loadedAssets[key] = handle;
            }
            results.AddRange(handle.Result);
        }
        else
        {
            Debug.LogError("AA同步资源加载失败.");
        }

        return results;
    }

    public static List<T> LoadAssets<T>(IEnumerable<string> keys)
    {
        return BaseLoadAssets<T>(keys);
    }

    public static List<T> LoadAssetsWithTag<T>(string tag)
    {
        var resourceLocations = Addressables.LoadResourceLocationsAsync(tag).Result;
        var keys = resourceLocations.Select(location => location.PrimaryKey);
        return BaseLoadAssets<T>(keys);
    }

    // Unload a single asset
    public static void UnloadAsset(string key)
    {
        if (loadedAssets.TryGetValue(key, out AsyncOperationHandle handle))
        {
            Addressables.Release(handle);
            loadedAssets.Remove(key);
        }
    }
    #endregion

    private static List<string> CheckUnloadedKeys(IEnumerable<string> keys)
    {
        List<string> unloadedKeys = new List<string>();
        foreach (var key in keys)
        {
            if (!loadedAssets.ContainsKey(key))
            {
                unloadedKeys.Add(key);
            }
        }
        return unloadedKeys;
    }


    private static void OnDestroy()
    {
        foreach (var handle in loadedAssets.Values)
        {
            Addressables.Release(handle);
        }
        loadedAssets.Clear();
    }
}
