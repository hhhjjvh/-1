using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class TeleporPoint : MonoBehaviour, IInteractable
{
    [SerializeField] private SceneLoadEventSO loadEventSo;
    public GameSceneSO GameScenceSo;
    public Vector2 TeleportPosition;
    public GameObject Name;

    public bool IsInteractable()
    {
        return true;
    }

    public void TiggerAction()
    {
       SceneLoadManager.Instance.sceneLoadEvent.OnSceneLoad(GameScenceSo, TeleportPosition, true);
    }

    void Start()
    {
        Name = transform.Find("Name")?.gameObject;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Name?.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Name?.SetActive(false);
        }
    }
}
