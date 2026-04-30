using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "消耗此牌", menuName = "SO/卡牌/消耗此牌")]
public class RemoveCard : CardEffectSO
{
    public override void ApplyEffect(Player caster, Character target, Card card)
    {
        caster.RemoveCard(card);
    }
}
