using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    private float time1 = 1;
    private float time2 = 0;
    // Start is called before the first frame update
    void Start()
    {
        time2 = time1;
    }

    // Update is called once per frame
    void Update()
    {
        time2 -= Time.deltaTime;
    }
   
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (time2 <= 0)
            {
                time2 = time1;
                float x=Random.Range(0.08f,0.15f);
               // collision.gameObject.GetComponent<CharacterStats>().DoMaxDamage(x);
            }
        }
    }

}
