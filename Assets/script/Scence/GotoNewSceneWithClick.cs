using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GotoNewSceneWithClick : MonoBehaviour
{
    [SerializeField] private SceneLoadEventSO loadEventSo;
    public GameSceneSO GameScenceSo;
    public Vector2 TeleportPosition;
   
    public void GotoNewScene()
    {
        SceneLoadManager.Instance.sceneLoadEvent.OnSceneLoad(GameScenceSo, TeleportPosition, true);
    }
}
