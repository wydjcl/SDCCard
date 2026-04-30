using DG.Tweening;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fork : Enemy
{
    public override void OnStartServer()
    {
        base.OnStartServer();
        HP.Value = 100;
        maxHP.Value = 100;
        speed.Value = 150;
    }

    [Server]
    public override void ServerMove()
    {
        Debug.Log("服务端执行攻击逻辑");
        foreach (var c in currentRoom.Value.characters)
        {
            if (c is Player player)
            {
                c.TakeDamage(5);
            }
        }
        TurnEnd();
    }
}
