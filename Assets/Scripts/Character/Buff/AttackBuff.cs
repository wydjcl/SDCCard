using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 力量相关的buff,增加攻击力EX
/// </summary>
[CreateAssetMenu(fileName = "力量Buff", menuName = "SO/Buff/力量Buff")]
public class AttackBuff : BuffEffect
{
    public AttackBuff()
    {
        buffName = "力量Buff";
    }
    public override void Apply(Character character, int v)
    {
        base.Apply(character, v);
        character.TakeAttackEX(v);
    }
    public override void Remove(Character character, int v)
    {
        base.Remove(character, v);
        character.TakeAttackEX(-v);
    }
}