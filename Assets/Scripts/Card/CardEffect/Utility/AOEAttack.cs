using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "AOE攻击", menuName = "SO/卡牌/AOE攻击")]
public class AOEAttack : CardEffectSO
{
    public int damage = 6;
    public float percent = 1f;
    public override void ApplyEffect(Player caster, Character target, Card card)
    {
        foreach (var c in caster.currentRoom.Value.characters)
        {
            if (c is Enemy e)
            {
                if (!e.isDead.Value)
                {
                    e.TakeDamage(Mathf.CeilToInt(caster.Attack() * percent));
                }
            }
        }
    }
}
