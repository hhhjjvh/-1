using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SceneLoadEventSo", menuName = "Event/SceneLoadEventSo")]
public class SceneLoadEventSO : ScriptableObject
{
    public UnityAction<GameSceneSO,Vector3,bool> OnSceneLoad;

    public void LoadEvent(GameSceneSO gameScenceSo, Vector3 pos, bool isLoad)
    {
        OnSceneLoad?.Invoke(gameScenceSo, pos, isLoad);
     //   Debug.Log("LoadEvent");
    }
}
