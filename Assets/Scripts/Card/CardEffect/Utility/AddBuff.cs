using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
[CreateAssetMenu(fileName = "给予Buff", menuName = "SO/卡牌/给予Buff")]
public class AddBuff : CardEffectSO
{
    public bool self;
    public bool isAOE;

    public string buffName;
    public int value;
    public int duration;
    public bool forever;
    public override void ApplyEffect(Player caster, Character target, Card card)
    {
        Buff buff = new Buff();
        buff.value = value;
        buff.buffName = buffName;
        buff.forever = forever;
        buff.duration = duration;
        if (self)
        {
            if (isAOE)
            {
                foreach (var c in caster.currentRoom.Value.characters)
                {
                    if (c is Player p)
                    {
                        if (!p.isDead.Value)
                        {
                            p.AddBuff(buff);
                        }
                    }
                }
            }
            else
            {
                caster.AddBuff(buff);
            }

        }
        else
        {
            if (isAOE)
            {
                foreach (var c in caster.currentRoom.Value.characters)
                {
                    if (c is Enemy e)
                    {
                        if (!e.isDead.Value)
                        {
                            e.AddBuff(buff);
                        }
                    }
                }
            }
            else
            {
                target.AddBuff(buff);
            }
        }
    }

}
