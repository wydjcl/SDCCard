using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wolf : Enemy
{
    public override int Attack()
    {
        int i = 0;
        i = Mathf.RoundToInt((attack.Value + attackEX.Value) * (attackPercent.Value + 1f)) + FindWolfNum() * 3;//因为浮点数不精确的原因,所以需要四舍五入而不是取上
        return i;
    }
    [Server]
    public override void ServerMove()
    {
        Player target = currentRoom.Value.roomBattleManager.GetRandomPlayer();
        if (target.HP.Value >= target.maxHP.Value / 2)
        {
            target.TakeDamage(this, Attack());
        }
        else
        {
            target.TakeDamage(this, Mathf.CeilToInt(Attack() * 1.5f));
        }
        DTextManager.Instance.CreateText(this.aniUI.transform, "撕咬");
        TurnEnd();
    }

    public int FindWolfNum()
    {
        int i = -1;
        foreach (var c in currentRoom.Value.characters)
        {
            if (c is Wolf && !c.isDead.Value)
            {
                i++;
            }
        }
        return i;
    }
    [ContextMenu("输出狼群数量")]
    public void test()
    {
        Debug.Log(FindWolfNum() + 1);
    }
}
