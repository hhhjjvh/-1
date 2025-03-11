using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToNewScence : MonoBehaviour
{
    public SceneLoadEventSO loadEventSo;
    public GameSceneSO GameScenceSo;
    public Vector2 TeleportPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            loadEventSo.OnSceneLoad(GameScenceSo, TeleportPosition, true);
        }
    }
}
