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
            ServerDTextRpc("狙击");
            targetP.TakeDamage(this, Attack());
            targetP = null;
        }
        else
        {
            var r = Random.value;
            if (r < 0.7f)
            {
                ServerDTextRpc("蓄力...");
                targetP = currentRoom.Value.roomBattleManager.GetRandomPlayer();
            }
            else
            {
                ServerDTextRpc("后撤步");
                dodge.Value = 1;
            }
        }


        TurnEnd();
    }
}
