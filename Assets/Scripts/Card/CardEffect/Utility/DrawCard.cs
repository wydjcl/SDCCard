using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "抽牌", menuName = "SO/卡牌/抽牌")]
public class DrawCard : CardEffectSO
{
    [Header("抽牌数")]
    public int drawValue = 2;
    public override void ApplyEffect(Player caster, Character target, Card card)
    {
        caster.DrawCard(drawValue);
    }

}
