using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToNewScence : MonoBehaviour
{
    public SceneLoadEventSO loadEventSo;
    public GameSceneSO GameScenceSo;
    public Vector2 TeleportPosition;
  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            loadEventSo.OnSceneLoad(GameScenceSo, TeleportPosition, true);
        }
    }
}
