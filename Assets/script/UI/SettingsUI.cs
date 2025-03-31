using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsUI : BasePanel
{
    private float soundValue = 0;
    private float BGMValue = 0;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider BGMSlider;
    [SerializeField] private AudioMixer master;
    [SerializeField] private Text soundText;
    [SerializeField] private Text BGMText;
    [SerializeField] private Button mainMeneBtn;

    private void Awake()
    {
        OpenPanel(UIConst.Settings);
        soundSlider = transform.GetChild(0).GetChild(3).GetComponent<Slider>();
        BGMSlider = transform.GetChild(0).GetChild(4).GetComponent<Slider>();
        soundText = transform.GetChild(0).GetChild(3).GetChild(0).GetComponent<Text>();
        BGMText = transform.GetChild(0).GetChild(4).GetChild(0).GetComponent<Text>();
        mainMeneBtn = transform.GetChild(0).GetChild(5).GetComponent<Button>();

        //master.GetFloat("SFX", out soundValue);

        //soundSlider.value = soundValue;
        //master.GetFloat("BGM", out BGMValue);
        //Debug.Log(master.name);
        //BGMSlider.value = BGMValue;
    }

    private void OnEnable()
    {
        Loads();
        if (GameManager.Instance != null)
        {

            mainMeneBtn.onClick.AddListener(() => GameManager.Instance.ExitGame() );
            mainMeneBtn.onClick.AddListener(() => CloseSettings());
        }
    }

    private void Start()
    {
        if (GameManager.Instance!=null)
        { // 非主菜单
            mainMeneBtn.interactable = true;
           // GameManager.Instance.isPaused = true;
            GameManager.Instance.PauseGame(true);
            Time.timeScale = 0;
        }
    }

    private void Update()
    {
        soundText.text = ((int)(soundSlider.value + 80f)).ToString() + "%";
        BGMText.text = ((int)(BGMSlider.value + 80f)).ToString() + "%";
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSettings();
        }
    }

    /// <summary>
    /// 关闭设置面板
    /// </summary>
    private void CloseSettings()
    {
        if (!isRemove)
        {
            ClosePanel();
            if (GameManager.Instance==null) return;
            GameManager.Instance.PauseGame(false);
            Time.timeScale = 1f;
          
        }
    }

    private void OnDestroy()
    {
        Saves();
        CloseSettings();
        ClosePanel();
    }
     void Saves()
    {
        PlayerPrefs.SetFloat("SFX", soundSlider.value);
        PlayerPrefs.SetFloat("BGM", BGMSlider.value);
        PlayerPrefs.Save();
    }
    void Loads()
    {
        soundSlider.value = PlayerPrefs.GetFloat("SFX");
        BGMSlider.value = PlayerPrefs.GetFloat("BGM");
        SetSoundVolume(soundSlider);
        SetBGMVolume(BGMSlider);
    }


    /// <summary>
    /// 控制音效大小
    /// </summary>
    /// <param name="s"></param>
    public void SetSoundVolume(Slider s)
    {
       
       AudioMgr.Instance.SetSFXVolume(s.value);
      
    }

    /// <summary>
    /// 控制背景音乐大小
    /// </summary>
    /// <param name="s"></param>
    public void SetBGMVolume(Slider s)
    {
        AudioMgr.Instance.SetMusicVolume(s.value);
      
    }
}
