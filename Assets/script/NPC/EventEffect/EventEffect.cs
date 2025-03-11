// EventEffect.cs
using UnityEditor;
using UnityEngine;

public abstract class EventEffect : ScriptableObject
{
    public abstract void ApplyEffect(GameObject target = null);
    public virtual bool ApplyTrigger()
    {
        Debug.Log("ApplyTrigger");
        return true;
    }
    public virtual string GetEffectDescription()
    {
        return "Î´Öª½±Àø";
    }

}

