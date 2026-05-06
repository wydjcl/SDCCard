using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "给予格挡", menuName = "SO/卡牌/给予格挡")]
public class GiveBlock : CardEffectSO
{
    [Header("格挡值")]
    public int amount = 5;
    [Header("是否给予自己")]
    public bool self;
    public override void ApplyEffect(Player caster, Character target, Card card)
    {
        if (self)
        {
            caster.TakeBlock(amount);
        }
        else
        {
            target.TakeBlock(amount);
        }
    }
}
