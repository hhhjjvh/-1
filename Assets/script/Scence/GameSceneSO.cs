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
    房间,
    厨房,
    商场大门口,
    商场一层,
    小游戏1
}
[CreateAssetMenu(fileName = "GameScenceSo", menuName = "GameScence/GameScenceSo")]
public class GameSceneSO : ScriptableObject
{

    public AssetReference SceneReference;
    public GameScenceType gameScenceType;
    public ScenceBGM sceneBGM;
    public SceneName sceneName;
}
