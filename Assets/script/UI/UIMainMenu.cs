using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIMainMenu : MonoBehaviour
{
    [Header("Navigation Settings")]
    [SerializeField] private Selectable _firstSelected; // 初始选中项
    [SerializeField] private float _navDelay = 0.2f; // 导航响应间隔

    private Selectable _lastSelectedBeforePopup; // 弹窗前最后选中的对象
    [Header("Scene Settings")]
    // [SerializeField] private string sceneName;
    [SerializeField] private GameObject continueButton;
    //  [SerializeField] private UIFadeScreen fadeScreen;
    public AssetReference currentSceneSO;
    [Header("UI Components")]
    [SerializeField] private GameObject saveSlotPrefab;  // 存档槽位预制体
    [SerializeField] private Transform saveSlotContainer; // 存档列表容器
    [SerializeField] private Button newGameButton;       // 全局新建游戏按钮
    [SerializeField] private Button ReturnButton;
    [SerializeField] private Button SetButton;
    [SerializeField] private GameObject confirmDeletePanel; // 删除确认面板
    [SerializeField] private TMP_Text emptySlotHint;     // 空存档提示文本
    public int maxSaveSlots = 5; // 最大存档槽位数
                                 // 在UIMainMenu类中添加以下成员变量
    [Header("New Game Settings")]
    [SerializeField] private GameObject overwriteConfirmPanel; // 覆盖确认面板
    [SerializeField] private TMP_Text overwriteConfirmText;
    private int _newGameSlotIndex = 1; // 默认使用第一个存档位

    // public GameObject Saves;
    public GameObject Loads;


    private int selectedSlotIndex = -1; // 当前选中的存档槽位
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1;
        RefreshSaveSlots();
        confirmDeletePanel.SetActive(false);
        //  Saves?.SetActive(false);
        Loads.SetActive(false);
        // 替换原有EventSystem设置
        StartCoroutine(SetInitialFocus());

        // 添加导航配置
        SetupButtonNavigation();
        //if (!SaveManager.instance.HasSaveData())
        //{
        //    continueButton.SetActive(false);
        //}
        var inputModule = EventSystem.current.currentInputModule as StandaloneInputModule;
        if (inputModule != null)
        {
            inputModule.inputActionsPerSecond = 1f / _navDelay;
        }
        continueButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            ContinueGame();
        });
        ReturnButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            Loads.SetActive(false);
            EnableMainMenuNavigation(this.gameObject);
            StartCoroutine(SetPopupFocus(continueButton.GetComponent<Selectable>()));
        });

        if (CheckExistingSaves())
        {
            newGameButton.GetComponentInChildren<TMP_Text>().text = "清空存档";

        }
        SetButton.GetComponent<Button>().onClick.AddListener(() => { SettingsOpen(); });
    }
    public void SettingsOpen()
    {
        UIManager.Instance.OpenPanel(UIConst.Settings);
    }

    // 修改现有的NewGame方法
    public void NewGame()
    {
        bool hasAnySave = CheckExistingSaves();

        if (!hasAnySave)
        {
            // 没有存档直接创建新游戏
            CreateNewSave(_newGameSlotIndex);
        }
        else
        {
            // Saves.SetActive(true);
            // 显示覆盖确认面板
            ShowOverwriteConfirm();
        }
    }

    // 新增检查存档方法
    private bool CheckExistingSaves()
    {
        for (int i = 1; i <= maxSaveSlots; i++)
        {
            if (SaveManager.Instance.HasSaveData(i))
            {
                return true;
            }
        }
        return false;
    }

    // 新增覆盖确认方法
    private void ShowOverwriteConfirm()
    {
        var currentSelected = EventSystem.current.currentSelectedGameObject;
        _lastSelectedBeforePopup = (currentSelected != null) ?
            currentSelected.GetComponent<Selectable>() :
            _firstSelected; // 若当前无选中项，回退到初始项

        overwriteConfirmPanel.SetActive(true);
        overwriteConfirmText.text = "是否覆盖所有进度重新开始？";

        // 设置确认面板按钮
        Button btn_Yes = overwriteConfirmPanel.transform.Find("btn_ConfirmYes").GetComponent<Button>();
        Button btn_No = overwriteConfirmPanel.transform.Find("btn_ConfirmNo").GetComponent<Button>();
        btn_Yes.GetComponentInChildren<TMP_Text>().text = "确定";
        btn_No.GetComponentInChildren<TMP_Text>().text = "取消";
        btn_Yes.onClick.RemoveAllListeners();
        btn_Yes.onClick.AddListener(() =>
        {
            ForceNewGame();
            overwriteConfirmPanel.SetActive(false);
        });

        btn_No.onClick.RemoveAllListeners();
        btn_No.onClick.AddListener(() =>
        {
            overwriteConfirmPanel.SetActive(false);
            // 新增：恢复主菜单导航
            EnableMainMenuNavigation(this.gameObject);
            StartCoroutine(RestoreFocusAfterPopup());
        });
        DisableMainMenuNavigation(this.gameObject);
        // 设置导航
        SetupHorizontalNavigation(btn_Yes.GetComponent<Selectable>(), btn_No.GetComponent<Selectable>());
        StartCoroutine(SetPopupFocus(btn_Yes.GetComponent<Selectable>()));
        EnableMainMenuNavigation(overwriteConfirmPanel);
    }

    // 强制新建游戏方法
    private void ForceNewGame()
    {
        for (int i = 1; i <= maxSaveSlots; i++)
        {
            SaveManager.Instance.DeleteSavedData(i);
        }
        CreateNewSave(_newGameSlotIndex);
    }

    IEnumerator SetInitialFocus()
    {
        yield return new WaitForEndOfFrame();
        bool hasAnySave = CheckExistingSaves();

        if (!hasAnySave)
        {  //  StartCoroutine(SetPopupFocus(continueButton.GetComponent<Selectable>()));
            EventSystem.current.SetSelectedGameObject(_firstSelected?.gameObject);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(continueButton?.gameObject);
        }
    }

    private void SetupButtonNavigation()
    {
        // 主界面导航
        var newGameNav = new Navigation
        {
            mode = Navigation.Mode.Explicit,
            selectOnDown = continueButton.GetComponent<Selectable>(),
            selectOnUp = _firstSelected //GetComponentInChildren<Button>() // 根据实际布局调整
        };
        newGameButton.navigation = newGameNav;

        // 动态槽位导航在RefreshSaveSlots中处理
    }
    // 禁用主菜单导航
    private void DisableMainMenuNavigation(GameObject popup)
    {
        foreach (var selectable in popup.GetComponentsInChildren<Selectable>())
        {
            selectable.interactable = false;
        }
    }
    // 弹窗关闭后恢复交互
    private void EnableMainMenuNavigation(GameObject popup)
    {
        foreach (var selectable in popup.GetComponentsInChildren<Selectable>())
        {

            selectable.interactable = true;
        }

    }
    // 刷新所有存档槽位UI
    public void RefreshSaveSlots()
    {
        // 清空现有槽位
        foreach (Transform child in saveSlotContainer)
        {
            Destroy(child.gameObject);
        }

        bool hasAnySave = false;

        Selectable previousSlotButton = null;
        // 生成3个存档槽位（示例）
        for (int i = 0; i < maxSaveSlots; i++)
        {
            GameObject slot = Instantiate(saveSlotPrefab, saveSlotContainer);
            int slotIndex = i + 1; // 槽位从1开始

            // 获取元数据（需在GameData类中添加）
            bool hasSave = SaveManager.Instance.HasSaveData(slotIndex);
            string saveTime = hasSave ?
                SaveManager.Instance.GetSaveMetadata(slotIndex).saveTime :
                "空存档";
            TimeSpan timeSpan = hasSave ?
                TimeSpan.FromSeconds(SaveManager.Instance.GetSaveMetadata(slotIndex)._totalPlaytimeSeconds) :
                TimeSpan.Zero;
            string sceneName = hasSave ?
                SaveManager.Instance.GetSaveMetadata(slotIndex).LoadGameScene().sceneName.ToString() :
                "无";

            // 设置UI元素
            slot.transform.Find("txt_SlotNumber").GetComponent<TMP_Text>().text = $"存档位 {slotIndex}";
            slot.transform.Find("txt_SaveTime").GetComponent<TMP_Text>().text
                = $"保存时间: {saveTime}    游戏时间:{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            slot.transform.Find("txt_sceneName").GetComponent<TMP_Text>().text = $"场景名称: {sceneName}";

            // 按钮事件绑定
            Button btn_Load = slot.transform.Find("btn_Load").GetComponent<Button>();
            Button btn_Delete = slot.transform.Find("btn_Delete").GetComponent<Button>();

            if (hasSave)
            {
                btn_Load.GetComponentInChildren<TMP_Text>().text = $"加载";
                btn_Delete.GetComponentInChildren<TMP_Text>().text = $"删除";
                btn_Load.onClick.AddListener(() => OnSlotSelected(slotIndex));
                btn_Delete.onClick.AddListener(() => ShowDeleteConfirm(slotIndex));
                hasAnySave = true;
            }
            else
            {
                btn_Load.GetComponentInChildren<TMP_Text>().text = "开始新游戏";
                btn_Load.onClick.AddListener(() => CreateNewSave(slotIndex));

                btn_Delete.gameObject.SetActive(false);
                // 调整“加载”按钮的右导航
                var loadSelectable = btn_Load.GetComponent<Selectable>();
                var loadNav = loadSelectable.navigation;
                loadNav.selectOnRight = null; // 清除右导航
                loadSelectable.navigation = loadNav;
            }
            // 导航设置
            var slotSelectable = slot.GetComponent<Selectable>();
            if (previousSlotButton != null)
            {
                SetupVerticalNavigation(previousSlotButton, slotSelectable);
            }
            previousSlotButton = slotSelectable;

            // 按钮内部导航
            var loadButton = btn_Load.GetComponent<Selectable>();
            var deleteButton = btn_Delete.GetComponent<Selectable>();
            SetupHorizontalNavigation(loadButton, deleteButton);
        }
        // 首尾循环导航
        if (saveSlotContainer.childCount > 0)
        {
            var first = saveSlotContainer.GetChild(0).GetComponent<Selectable>();
            var last = saveSlotContainer.GetChild(saveSlotContainer.childCount - 1).GetComponent<Selectable>();
            SetupVerticalNavigation(last, first);
        }
        emptySlotHint.gameObject.SetActive(!hasAnySave);
    }
    private void SetupVerticalNavigation(Selectable upper, Selectable lower)
    {
        if (upper == null && lower == null) return;
        var upperNav = upper.navigation;
        upperNav.selectOnDown = lower;
        upper.navigation = upperNav;

        var lowerNav = lower.navigation;
        lowerNav.selectOnUp = upper;
        lower.navigation = lowerNav;
    }

    private void SetupHorizontalNavigation(Selectable left, Selectable right)
    {
        if (left == null && right == null) return;
        var leftNav = left.navigation;
        leftNav.selectOnRight = right;
        left.navigation = leftNav;

        var rightNav = right.navigation;
        rightNav.selectOnLeft = left;
        right.navigation = rightNav;
    }

    // 选中存档槽位
    private void OnSlotSelected(int slotIndex)
    {
        Debug.Log($"Selected slot {slotIndex}");
        selectedSlotIndex = slotIndex;
        StartCoroutine(LoadSceneWithFadeEffect(0.5f));
    }
    // 创建新存档
    private void CreateNewSave(int slotIndex)
    {
        SaveManager.Instance.DeleteSavedData(slotIndex); // 确保干净状态
                                                         // SaveManager.instance.SaveGame(slotIndex);         // 初始保存
        RefreshSaveSlots();
        OnSlotSelected(slotIndex);
    }

    // 显示删除确认面板
    private void ShowDeleteConfirm(int slotIndex)
    {
        selectedSlotIndex = slotIndex;
        confirmDeletePanel.SetActive(true);
        confirmDeletePanel.transform.Find("txt_Confirm").GetComponent<TMP_Text>().text =
            $"确定要删除存档位 {slotIndex} 的进度吗？";
        var currentSelected = EventSystem.current.currentSelectedGameObject;
        _lastSelectedBeforePopup = (currentSelected != null) ?
            currentSelected.GetComponent<Selectable>() :
            _firstSelected; // 若当前无选中项，回退到初始项
        // 新增：禁用主菜单导航
        DisableMainMenuNavigation(this.gameObject);
        EnableMainMenuNavigation(confirmDeletePanel);
        Button btn_Yes = confirmDeletePanel.transform.Find("btn_ConfirmYes").GetComponent<Button>();
        Button btn_No = confirmDeletePanel.transform.Find("btn_ConfirmNo").GetComponent<Button>();
        btn_Yes.GetComponentInChildren<TMP_Text>().text = "确定";
        btn_No.GetComponentInChildren<TMP_Text>().text = "取消";
        btn_Yes.onClick.AddListener(ConfirmDelete);
        btn_No.onClick.AddListener(CancelDelete);

        // _lastSelectedBeforePopup = EventSystem.current.currentSelectedGameObject?.GetComponent<Selectable>();

        // 设置确认面板导航
        var yesButton = confirmDeletePanel.transform.Find("btn_ConfirmYes").GetComponent<Selectable>();
        var noButton = confirmDeletePanel.transform.Find("btn_ConfirmNo").GetComponent<Selectable>();
        SetupHorizontalNavigation(yesButton, noButton);

        StartCoroutine(SetPopupFocus(yesButton));
    }
    IEnumerator SetPopupFocus(Selectable firstSelected)
    {
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(firstSelected.gameObject);
    }

    // 确认删除存档
    public void ConfirmDelete()
    {
        if (selectedSlotIndex != -1)
        {
            SaveManager.Instance.DeleteSavedData(selectedSlotIndex);
            RefreshSaveSlots();
            confirmDeletePanel.SetActive(false);
            // 新增：启用主菜单导航
            EnableMainMenuNavigation(Loads);
            // StartCoroutine(RestoreFocusAfterPopup());
            StartCoroutine(SetPopupFocus(ReturnButton.GetComponent<Selectable>()));
        }
    }

    // 取消删除
    public void CancelDelete()
    {

        confirmDeletePanel.SetActive(false);
        selectedSlotIndex = -1;
        // 新增：启用主菜单导航
        EnableMainMenuNavigation(Loads);
        StartCoroutine(RestoreFocusAfterPopup());
    }
    IEnumerator RestoreFocusAfterPopup()
    {
        yield return null;

        if (_lastSelectedBeforePopup != null)
        {
            EventSystem.current.SetSelectedGameObject(_lastSelectedBeforePopup.gameObject);
            _lastSelectedBeforePopup = null; // 新增置空操作
        }
    }

    IEnumerator LoadSceneWithFadeEffect(float fadeTime)
    {
        //Debug.Log("Loading Scene..." + currentSceneSO);
        //fadeScreen.gameObject.SetActive(true);
        // fadeScreen.FadeOut();
        // Debug.Log("Loading Scene..." + selectedSlotIndex);
        // 传递存档槽位索引给游戏场景
       // PlayerPrefs.DeleteKey("SelectedSaveSlot");
        PlayerPrefs.SetInt("SelectedSaveSlot", selectedSlotIndex);
        PlayerPrefs.Save();
        //Addressables.LoadSceneAsync(currentSceneSO);
        var load = Addressables.LoadSceneAsync(currentSceneSO);
        // 启用加载UI
        LoadSceneUI loadUI = UIManager.Instance.OpenPanel(UIConst.LoadScene) as LoadSceneUI;
        loadUI.LoadLevels(load);
        yield return new WaitForSeconds(fadeTime);
        Debug.Log("Loading Scene..." + currentSceneSO);
        
        //currentSceneSO.LoadSceneAsync(LoadSceneMode.Additive, true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }


    public void ContinueGame()
    {
        Loads.SetActive(true);
        var currentSelected = EventSystem.current.currentSelectedGameObject;
        _lastSelectedBeforePopup = (currentSelected != null) ?
            currentSelected.GetComponent<Selectable>() :
            _firstSelected; // 若当前无选中项，回退到初始项
        DisableMainMenuNavigation(this.gameObject);
        EnableMainMenuNavigation(Loads);
        StartCoroutine(SetPopupFocus(ReturnButton.GetComponent<Selectable>()));
        // StartCoroutine(SetPopupFocus(yesButton));
        //SceneManager.LoadScene(sceneName);
        //StartCoroutine(LoadScenceWithFadeEffect(0.1f));
    }

}
