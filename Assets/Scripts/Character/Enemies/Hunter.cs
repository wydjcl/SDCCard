using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : Enemy
{
    public Player targetP;
    [Server]
    public override void ServerMove()
    {
        if (targetP != null)
        {
            DTextManager.Instance.CreateText(this.aniUI.transform, "狙击");
            targetP.TakeDamage(this, Attack());
        }
        else
        {
            var r = Random.value;
            if (r < 0.7f)
            {
                DTextManager.Instance.CreateText(this.aniUI.transform, "蓄力...");
                targetP = currentRoom.Value.roomBattleManager.GetRandomPlayer();
            }
            else
            {
                DTextManager.Instance.CreateText(this.aniUI.transform, "后撤步");
                dodge.Value = 1;
            }
        }


        TurnEnd();
    }
}
