using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "护盾猛击", menuName = "SO/卡牌/护盾猛击")]
public class BlockAttack : CardEffectSO
{

    public override void ApplyEffect(Player caster, Character target, Card card)
    {
        target.TakeDamage(caster, caster.block.Value);
    }
}
