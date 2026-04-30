using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "弃置此牌", menuName = "SO/卡牌/弃置此牌")]
public class DiscardCard : CardEffectSO
{
    public override void ApplyEffect(Player caster, Character target, Card card)
    {
        caster.DiscardCard(card);
    }
}
