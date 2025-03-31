using UnityEngine;
using UnityEngine.AddressableAssets;


public enum GameScenceType
{
    MainScence = 0,
    GameScence = 1
}

public enum ScenceBGM
{
    None,
    Gentle_Morning_Glow,
    In_the_Whispering_Moonlight

}
public enum SceneName
{
    MainScence,
    ����,
    ����,
    �̳����ſ�,
    �̳�һ��,
    С��Ϸ1
}
[CreateAssetMenu(fileName = "GameScenceSo", menuName = "GameScence/GameScenceSo")]
public class GameSceneSO : ScriptableObject
{

    public AssetReference SceneReference;
    public GameScenceType gameScenceType;
    public ScenceBGM sceneBGM;
    public SceneName sceneName;
}
