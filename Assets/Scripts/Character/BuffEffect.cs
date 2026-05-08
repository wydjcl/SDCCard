using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Buff效果", menuName = "SO/Buff/Buff效果")]
public class BuffEffect : ScriptableObject
{
    public string buffName;
    public Sprite buffSprite;
    public virtual void Apply(Character character, int value)
    {
    }
    public virtual void Remove(Character character, int value)
    {
    }
    public virtual void TurnStart(Character character, int value)
    {
    }
    public virtual void TurnEnd(Character character, int value)
    {
    }
}

public class Buff
{
    public string buffName;
    public int value;
    public int duration;
    public bool forever;
}
