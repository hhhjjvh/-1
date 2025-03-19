using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemEffect : ScriptableObject
{
    [TextArea]
    public string itemEffectDescription;
    public virtual void ExecuteEffect(Transform enemyPosition)
    {

    }
}
