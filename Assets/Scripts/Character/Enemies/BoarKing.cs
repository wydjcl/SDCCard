using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarKing : Enemy
{
    public int skill0Count = 0;//TODO做成Buff便于显示
    public override void ServerMove()
    {
        base.ServerMove();
        int value = Random.Range(0, 3);
        if (value == 0)
        {
            Skill0(currentRoom.Value.roomBattleManager.GetRandomPlayer());
        }
        else if (value == 1)
        {
            Skill1();
        }
        else if (value == 2)
        {
            Skill2();
        }

        TurnEnd();
    }
    /// <summary>
    /// 獠牙撕咬
    /// </summary>
    /// <param name="target"></param>
    public void Skill0(Player target)
    {
        target.TakeDamage(this, Attack() + skill0Count * 5);
        skill0Count++;
        if (skill0Count >= 6)
        {
            skill0Count = 6;
        }
    }

    public void Skill1()
    {
        TakeBlock(10);
    }
    public void Skill2()
    {
        foreach (var c in currentRoom.Value.characters)
        {
            if (!c.isDead.Value)
            {
                if (c is Player p)
                {
                    p.TakeDamage(this, Attack());
                }
            }
        }
    }
}
