using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
/// <summary>
/// 角色类
/// </summary>
public class Character : NetworkBehaviour
{
    public readonly SyncVar<Room> currentRoom = new SyncVar<Room>();
    [Header("基础属性")]
    public readonly SyncVar<int> HP = new SyncVar<int>();
    public readonly SyncVar<int> maxHP = new SyncVar<int>();
    public readonly SyncVar<int> attack = new SyncVar<int>();//攻击力
    public readonly SyncVar<int> defense = new SyncVar<int>();//防御力
    public readonly SyncVar<int> speed = new SyncVar<int>();//速度
    [Header("额外属性")]
    public readonly SyncVar<int> attackEX = new SyncVar<int>();//攻击力
    public readonly SyncVar<int> defenseEX = new SyncVar<int>();//防御力
    public readonly SyncVar<int> speedEX = new SyncVar<int>();//速度
    [Header("其他属性")]
    public readonly SyncVar<int> block = new SyncVar<int>();//格挡值,回合开始时候清零

    [Header("战斗结算")]
    public readonly SyncVar<bool> isAction = new SyncVar<bool>();//正在行动
    public readonly SyncVar<bool> isDead = new SyncVar<bool>();//死亡
    public readonly SyncVar<bool> isSkip = new SyncVar<bool>();//为真时不进入回合计算

    public float progress = 0;//行动条,由服务端计算,本地不同步减少网络压力

    public int _HP;
    #region 生命周期和状态机
    public override void OnStartClient()
    {
        base.OnStartClient();
        HP.OnChange += HP_OnChange;
    }
    //这里写的子类并不能直接调用,请复制粘贴
    //按理来说,rpc方法应该直接在父类里面调用外部方法,然后子类不去重写,但是我已经写了所以懒得改了
    [ServerRpc(RequireOwnership = false)]
    public virtual void TurnStart()
    {
        ServerTurnStart();
    }
    [ServerRpc(RequireOwnership = false)]
    public virtual void TurnEnd()
    {
        ServerTurnEnd();
    }
    public virtual void ServerTurnStart()
    {
        block.Value = 0;//回合开始格挡值清零
        currentRoom.Value.roomBattleManager.noBodyAction = false;
        isAction.Value = true;
    }
    public virtual void ServerTurnEnd()
    {
        if (!isAction.Value)
        {
            return;
        }
        currentRoom.Value.roomBattleManager.noBodyAction = true;
        isAction.Value = false;
    }
    #endregion

    #region 一些网络方法

    [ServerRpc(RequireOwnership = false)]
    public virtual void TakeDamage(int damage)
    {
        HP.Value -= damage;
        if (HP.Value <= 0)
        {
            Debug.Log(name + "死亡");
            isDead.Value = true;


            if (this is Enemy enemy)
            {
                ServerEnemyDead();
                ClientEnemyDead();
            }
            if (this is Player player)
            {
                GameManager.Instance.player.CheckExit();
            }
            currentRoom.Value.roomBattleManager.DeadCheck();
        }

        ServerTakeDamageAni();
    }
    public virtual void ServerEnemyDead()//服务器处理敌人死后
    {
    }

    [ObserversRpc]
    public virtual void ClientEnemyDead()
    {
        transform.position = new Vector3(0, 100, 0);//服务器处理敌人死后,例如拉到屏幕外
    }
    [ObserversRpc]
    public virtual void ServerTakeDamageAni()//服务器处理受伤
    {
        ClientTakeDamageAni();
    }
    [Client]
    public virtual void ClientTakeDamageAni()//客户端处理受伤
    {

    }
    [ServerRpc(RequireOwnership = false)]
    public virtual void Heal(int heal)
    {
        if (HP.Value + heal > maxHP.Value)
        {
            HP.Value = maxHP.Value;
        }
        else
        {
            HP.Value += heal;
        }
    }
    public virtual void Heal(float healPercent)
    {
        int heal = Mathf.CeilToInt(maxHP.Value * healPercent);
        if (HP.Value + heal > maxHP.Value)
        {
            HP.Value = maxHP.Value;
        }
        else
        {
            HP.Value += heal;
        }
    }

    #endregion

    #region 客户端获取数值
    [Client]
    public virtual int Attack()//攻击里获得接口
    {
        int i = 0;
        i = attack.Value + attackEX.Value;
        return i;
    }

    #endregion

    #region 回调
    public virtual void HP_OnChange(int prev, int next, bool asServer)
    {
        _HP = next;
    }
    #endregion


    [ContextMenu("测试")]
    public void Test()
    {
        TakeDamage(5);
    }
}
