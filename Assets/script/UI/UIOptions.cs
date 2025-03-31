using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIOptions : MonoBehaviour
{
    [SerializeField] private Button ExitButton;
    [SerializeField] private Button SetButton;
    public GameObject Set;
    // Start is called before the first frame update
    void Start()
    {
        
        ExitButton.onClick.AddListener(() => SaveManager.Instance.SaveGame());
        ExitButton.onClick.AddListener(() => Application.Quit());

       SetButton.onClick.AddListener(() => Set.SetActive(true));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
