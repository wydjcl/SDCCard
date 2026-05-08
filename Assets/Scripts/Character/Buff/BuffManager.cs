using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : SingletonMono<BuffManager>
{
    public List<BuffEffect> effects = new List<BuffEffect>();
    public BuffEffect GetBuffEffect(Buff buff)
    {
        foreach (var effect in effects)
        {
            if (buff.buffName == effect.buffName)
            {
                return effect;
            }
        }
        Debug.LogWarning("没找到对应Buff!!!");
        return null;
    }
}
