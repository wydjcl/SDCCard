using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "普通攻击", menuName = "SO/卡牌/普通攻击")]
public class NormalAttack : CardEffectSO
{
    public int damage = 6;
    public float percent = 1f;
    public override void ApplyEffect(Player caster, Character target, Card card)
    {
        target.TakeDamage(Mathf.CeilToInt(caster.Attack() * percent));
    }
}
