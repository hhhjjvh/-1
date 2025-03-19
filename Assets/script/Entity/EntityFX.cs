using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EntityFX : MonoBehaviour
{
    [SerializeField] private float colorLooseRate=5;
    [SerializeField] private float AfterImageCooldown=0.01f;
    private float afterImageTimer;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        afterImageTimer -= Time.deltaTime;
    }
    public void CreatAfterImage()
    {
        if (afterImageTimer <= 0)
        {
            afterImageTimer = AfterImageCooldown;
            GameObject afterImage = PoolMgr.Instance.GetObj("AfterImage", transform.
                GetComponent<Entity>().anim.transform.position, transform.rotation);
            afterImage.GetComponent<AfterImageFX>().SetupAfterImage(colorLooseRate, spriteRenderer.sprite);
         //   Debug.Log("create after image");
        }
    }
}
