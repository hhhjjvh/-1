using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ScenceLoadEventSo", menuName = "Event/ScenceLoadEventSo")]
public class SceneLoadEventSO : ScriptableObject
{
    public UnityAction<GameSceneSO,Vector3,bool> OnSceneLoad;

    public void Invoke(GameSceneSO gameScenceSo, Vector3 pos, bool isLoad)
    {
        OnSceneLoad?.Invoke(gameScenceSo, pos, isLoad);
    }
}
