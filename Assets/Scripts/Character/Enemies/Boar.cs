using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boar : Enemy
{
    public override void OnStartServer()
    {
        base.OnStartServer();

    }

    [Server]
    public override void ServerMove()
    {
        Debug.Log("服务端执行攻击逻辑");
        foreach (var c in currentRoom.Value.characters)
        {
            if (c is Player player)
            {
                c.TakeDamage(attack.Value);
            }
        }
        TurnEnd();
    }
}
