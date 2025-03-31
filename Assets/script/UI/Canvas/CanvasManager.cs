using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class CanvasManager : MonoBehaviour
{
    public static CanvasManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }



    }
    void Update()
    {
        if (GameManager.Instance != null)
        {
            OpenUI();
        }
    }

    /// <summary>
    /// 打开各个界面(该界面未打开，且不存在其它界面时)
    /// </summary>
    private void OpenUI()
    {
        // 只可存在一个界面
        if (   !UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerBag)
            && !UIManager.Instance.panelDict.ContainsKey(UIConst.PlayerMission)
            && !UIManager.Instance.panelDict.ContainsKey(UIConst.Settings)
            && !UIManager.Instance.panelDict.ContainsKey(UIConst.Shop))
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                Debug.Log("打开背包");
                UIManager.Instance.OpenPanel(UIConst.PlayerBag);
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                UIManager.Instance.OpenPanel(UIConst.PlayerMission);
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                UIManager.Instance.OpenPanel(UIConst.Settings);
            }
        }
    }
    public void LoadSceneUI(AssetReference ScenceSo)
    {
        SaveManager.Instance.SaveGame();
        var load = Addressables.LoadSceneAsync(ScenceSo);
        // 启用加载UI
        LoadSceneUI loadUI = UIManager.Instance.OpenPanel(UIConst.LoadScene) as LoadSceneUI;
        loadUI.LoadLevels(load);
    }
}
