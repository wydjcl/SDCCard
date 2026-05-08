using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 跟荣耀有关的攻击
/// </summary>
[CreateAssetMenu(fileName = "荣耀攻击", menuName = "SO/卡牌/荣耀攻击")]
public class AttackByGlory : CardEffectSO
{
    [Header("修正百分比")]
    public float percent = 1f;
    [Header("每多少荣耀值")]
    public int gloryCount = 1;//每多少荣耀值
    [Header("提升多少点伤害")]
    public int attack = 0;//提升多少伤害

    [Header("是否单单通过荣耀值计算伤害")]
    public bool byGlory;//通过荣耀值计算伤害,例如2荣'
    [Header("每一点荣耀值伤害")]
    public int gloryAttack;
    [Header("通过荣耀值计算伤害修正百分比")]
    public float byGloryPercent = 1f;

    [Header("是否是AOE攻击")]
    public bool isAOE;
    public override void ApplyEffect(Player caster, Character target, Card card)
    {
        if (isAOE)
        {
            foreach (var c in caster.currentRoom.Value.characters)
            {
                if (c is Enemy e)
                {
                    if (!e.isDead.Value)
                    {
                        if (byGlory)
                        {
                            e.TakeDamage(caster, Mathf.CeilToInt((gloryAttack * caster.glory.Value) * byGloryPercent));
                        }
                        else
                        {
                            e.TakeDamage(caster, Mathf.CeilToInt((caster.Attack()) * percent) + (attack * (caster.glory.Value / gloryCount)));
                        }
                    }
                }
            }
        }
        else
        {
            if (byGlory)
            {
                target.TakeDamage(caster, Mathf.CeilToInt((gloryAttack * caster.glory.Value) * byGloryPercent));
            }
            else
            {
                target.TakeDamage(caster, Mathf.CeilToInt((caster.Attack()) * percent) + (attack * (caster.glory.Value / gloryCount)));
            }
        }

    }

}
