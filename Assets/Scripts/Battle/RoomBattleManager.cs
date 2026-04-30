using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RoomBattleManager : NetworkBehaviour
{
    public Room room;
    public bool noBodyAction;

    public override void OnStartServer()
    {
        base.OnStartServer();

    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        noBodyAction = true;
    }

    public void Update()
    {
        if (!IsServerStarted)
        {
            return;
        }
        if (!noBodyAction)
        {
            return;
        }
        if (!room.isBattle.Value)
        {
            return;
        }
        foreach (var character in room.characters)
        {
            if (character.isDead.Value)
            {
                continue;
            }
            if (character.isSkip.Value)
            {
                continue;
            }
            character.progress += character.speed.Value * Time.deltaTime;
            if (character.progress >= 100f)
            {
                character.progress -= 100f;

                noBodyAction = false; // 锁住（有人开始行动）

                character.TurnStart();
                break; // ❗非常重要，只能一个人行动
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void StartBattle()
    {
        Debug.Log("开始战斗");
    }

    [ServerRpc(RequireOwnership = false)]
    public void EndBattle()
    {
        foreach (var character in room.characters)
        {
            //if (character is Player player)
            //{
            //    player.CreateCard();
            //}
            character.isAction.Value = false;
        }
    }
    [Server]
    public Player GetRandomPlayer()
    {
        List<Player> list = new List<Player>();
        foreach (var character in room.characters)
        {
            if (character is Player player)
            {
                if (!player.isDead.Value)
                {
                    list.Add(player);
                }
            }
        }
        if (list.Count > 0)
        {
            Player randomPlayer = list[Random.Range(0, list.Count)];
            return randomPlayer;
        }
        return null;
    }
    [Server]
    public Enemy GetRandomEnemy()
    {
        List<Enemy> list = new List<Enemy>();
        foreach (var character in room.characters)
        {
            if (character is Enemy enemy)
            {
                if (!enemy.isDead.Value)
                {
                    list.Add(enemy);
                }
            }
        }
        if (list.Count > 0)
        {
            Enemy randomEnemy = list[Random.Range(0, list.Count)];
            return randomEnemy;
        }
        return null;
    }


    [ServerRpc(RequireOwnership = false)]
    public void DeadCheck()
    {
        List<Player> playerList = new List<Player>();
        foreach (var character in room.characters)
        {
            if (character is Player player)
            {
                playerList.Add(player);
            }
        }
        bool playerAllDead = true;
        foreach (var p in playerList)
        {
            if (!p.isDead.Value)
            {
                playerAllDead = false;
            }
        }
        if (playerAllDead)
        {
            Debug.Log("该房间玩家全死亡");
            room.isBattle.Value = false;
            noBodyAction = true;
            EndBattle();
            foreach (var p in playerList)
            {
                p.progress = 0;
                ClientLoseBattle(p.Owner);
            }
            //TODO全角色死亡
            return;
        }

        List<Enemy> enemyList = new List<Enemy>();
        foreach (var character in room.characters)
        {
            if (character is Enemy enemy)
            {
                enemyList.Add(enemy);
            }

        }
        bool enemyAllDead = true;
        foreach (var e in enemyList)
        {
            if (!e.isDead.Value)
            {
                enemyAllDead = false;
            }

        }
        if (enemyAllDead)
        {
            Debug.Log("敌人全死亡");
            room.isBattle.Value = false;
            EndBattle();
            foreach (var p in playerList)
            {
                p.progress = 0;
                ClientWinBattle(p.Owner);
            }
            //敌人全死亡
            noBodyAction = true;
        }
    }

    [Server]

    public void NewPlayerIn(Player player)
    {
        Debug.Log($"{room.gridPos.Value}房间有新玩家*{player.playerName.Value}*加入");
        //charactersInBattle.Add(player);
        //SortBySpeed();
    }
    [TargetRpc]
    public void ClientWinBattle(NetworkConnection conn)
    {
        BattleSceneManager.Instance.turnButtom.SetActive(false);
        GameManager.Instance.player.CreateCard();
    }

    [TargetRpc]
    public void ClientLoseBattle(NetworkConnection conn)
    {
        BattleSceneManager.Instance.turnButtom.SetActive(false);
        GameManager.Instance.player.CreateCard();
    }


    [ContextMenu("输出Debug")]

    public void DebugLog()
    {
        //Debug.Log($"noBodyAction:{}");
        //Debug.Log($"noBodyAction:{noBodyAction.Value}");

    }


}
