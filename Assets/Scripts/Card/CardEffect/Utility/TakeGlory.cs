using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "获得荣耀", menuName = "SO/卡牌/获得荣耀")]
public class TakeGlory : CardEffectSO
{
    public int amount = 1;
    public override void ApplyEffect(Player caster, Character target, Card card)
    {
        caster.TakeGlory(amount);
    }
}
