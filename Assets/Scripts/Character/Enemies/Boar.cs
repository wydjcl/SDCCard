using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boar : Enemy
{
    public bool isAOE = false;
    public override void OnStartServer()
    {
        base.OnStartServer();

    }

    [Server]
    public override void ServerMove()
    {
        if (isAOE)
        {
            foreach (var c in currentRoom.Value.characters)
            {
                if (c is Player player)
                {
                    c.TakeDamage(this, Attack());
                }
            }
        }
        //Debug.Log("服务端执行攻击逻辑");
        else
        {
            var target = currentRoom.Value.roomBattleManager.GetRandomPlayer();
            if (target != null)
            {
                target.TakeDamage(this, Attack());
            }
        }
        ServerDTextRpc("撞击");
        TurnEnd();
    }
}
