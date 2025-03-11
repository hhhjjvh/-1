using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System;
using System.Linq; // 如果使用的是TextMeshPro


// 定义按键记录结构
public struct KeyPressEntry
{
    public string actionName;
    public float pressTime;

    public KeyPressEntry(string name, float time)
    {
        actionName = name;
        pressTime = time;
    }
}

public class InputManager : MonoBehaviour
{
    // public InputActionAsset playerInput;
    public PlayerInput playerInput;
    public static InputManager Instance;

    public Vector2 moveInput;

    // private Dictionary<string, float> keyPressTimes = new Dictionary<string, float>(); // 记录按键按下时间
    private List<KeyPressEntry> keyPressEntries = new List<KeyPressEntry>();
    private float comboTimeWindow = 1.2f; // 允许松开按键后维持的时间（秒）
  //  public TextMeshProUGUI keyStatusText; // 如果使用 TextMeshPro
   // private float continuousKeyWindow = 0.2f; // 连续按键时间窗口，控制连续按下时的间隔
    private bool lastUp, lastDown, lastLeft, lastRight;
    private string keyStatus = "按键状态: ";
    public bool canInteract { get; set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        playerInput = new PlayerInput();
    }
    void OnEnable()
    {
        playerInput.Enable();
        LoadBindings();      
    }

  
    void OnDisable()
    {
        playerInput.Disable();
     
    }

    public void RestInput()
    {     
        LoadBindings();     
    }

    void Update()
    {
        moveInput = playerInput.Player.Move.ReadValue<Vector2>();
      
        if(moveInput.y > 0.5f!=lastUp)
        {
            if (moveInput.y > 0.5f) RegisterKey("Move_Up");
           // else UnregisterKey("Move_Up");
            lastUp = moveInput.y > 0.5f;
        }
        if (moveInput.y < -0.5f != lastDown)
        {
            if (moveInput.y < -0.5f) RegisterKey("Move_Down");
           // else UnregisterKey("Move_Down");
            lastDown = moveInput.y < -0.5f;
        }
        if (moveInput.x < -0.5f != lastLeft)
        {
            if (moveInput.x < -0.5f) RegisterKey("Move_Left");
            //else UnregisterKey("Move_Left");
            lastLeft = moveInput.x < -0.5f;
        }
        if (moveInput.x > 0.5f != lastRight)
        {
            if (moveInput.x > 0.5f) RegisterKey("Move_Right");
            //else UnregisterKey("Move_Right");
            lastRight = moveInput.x > 0.5f;
        }
        // 处理超时按键
        RemoveExpiredKeys();
        keyStatus = "按键状态: " + string.Join(", ", GetPressedKeys());
       // keyStatusText.text = keyStatus; // 更新UI文本
    }
    private void RegisterKeyWithEvents(string actionName, InputAction action)
    {
        action.started += ctx => RegisterKey(actionName);
        //action.canceled += ctx => UnregisterKey(actionName);
    }
    private void RemoveKeyWithEvents(string actionName, InputAction action)
    {
        action.started -= ctx => RegisterKey(actionName);
    }

    // 注册按键
    private void RegisterKey(string actionName)
    {
        keyPressEntries.Add(new KeyPressEntry(actionName, Time.time));
        ComboSystem.RegisterKey(actionName);  // 注册按键
    }
    // 取消注册按键
    
    private void RemoveExpiredKeys()
    {
        keyPressEntries.RemoveAll(entry => Time.time - entry.pressTime > comboTimeWindow);
    }
    // 获取当前按下的按键
    public List<string> GetPressedKeys()
    {     
        return keyPressEntries.Where(entry => Time.time - entry.pressTime <= comboTimeWindow)
                              .Select(entry => entry.actionName)
                              .ToList();
    }
    public void RestKeyPressEntries()
    {
        keyPressEntries.Clear();
    }
    private void SaveBindings()
    {
        //InputManager.Instance.RestInput();
        try
        {
            string bindings = playerInput.asset.SaveBindingOverridesAsJson();
            //inputActions.SaveBindingOverridesAsJson();
            PlayerPrefs.SetString("Bindings", bindings);
            PlayerPrefs.Save();
            // Debug.Log("Bindings saved successfully.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to save bindings: {ex.Message}");
        }
    }

    /// <summary>
    /// 从 PlayerPrefs 加载绑定
    /// </summary>
    private void LoadBindings()
    {

        if (PlayerPrefs.HasKey("Bindings"))
        {
            try
            {
                string bindings = PlayerPrefs.GetString("Bindings");
                playerInput.asset.LoadBindingOverridesFromJson(bindings);
                // Debug.Log("Bindings loaded successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to load bindings: {ex.Message}");
            }
        }
        else
        {
            foreach (var action in playerInput.asset)
            {
                action.RemoveAllBindingOverrides();
            }
        }
    }
}
