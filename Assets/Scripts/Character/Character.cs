using FishNet.Object;
using FishNet.Object.Synchronizing;
using GameKit.Dependencies.Utilities;
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
    //public readonly SyncVar<float> defense = new SyncVar<float>();//防御力
    public readonly SyncVar<int> speed = new SyncVar<int>();//速度
    [Header("额外属性")]
    public readonly SyncVar<int> attackEX = new SyncVar<int>();//攻击力固定数值增加
    //public readonly SyncVar<float> defenseEX = new SyncVar<float>();//防御力固定数值增加
    public readonly SyncVar<int> speedEX = new SyncVar<int>();//速度固定数值增加

    //public readonly SyncVar<float> AP = new SyncVar<float>();//穿甲数值

    public readonly SyncVar<float> attackPercent = new SyncVar<float>();//攻击力百分比数值增加
    //public readonly SyncVar<float> defensePercent = new SyncVar<float>();//防御力百分比数值增加
    public readonly SyncVar<float> speedPercent = new SyncVar<float>();//速度百分比数值增加
    [Header("其他属性")]
    public readonly SyncList<Buff> buffs = new SyncList<Buff>();
    public readonly SyncVar<int> block = new SyncVar<int>();//格挡值,回合开始时候清零
    public readonly SyncVar<int> blockEX = new SyncVar<int>();//格挡值增量,每次增加格挡值时多增长的量
    [Header("特殊词条")]
    public readonly SyncVar<int> dodge = new SyncVar<int>();
    public readonly SyncVar<int> glory = new SyncVar<int>();//荣耀,暂定每层1%攻击和防御力,上限20层,TODO技能树增加每层数值和上限
    public readonly SyncVar<int> gloryBlockEX = new SyncVar<int>();//荣耀的格挡值增量,每五层荣耀+1
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
        block.OnChange += Block_OnChange;
        attackEX.OnChange += AttackEX_OnChange;
        attackPercent.OnChange += AttackPercent_OnChange;
        glory.OnChange += Glory_OnChange;
    }



    public override void OnStopClient()
    {
        base.OnStopClient();
        HP.OnChange -= HP_OnChange;
        block.OnChange -= Block_OnChange;
        attackEX.OnChange -= AttackEX_OnChange;
        attackPercent.OnChange -= AttackPercent_OnChange;
        glory.OnChange -= Glory_OnChange;
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
                        // Debug.Log("nobody false");
        currentRoom.Value.roomBattleManager.noBodyAction = false;
        isAction.Value = true;

        BuffTurnStart();
    }
    public virtual void ServerTurnEnd()
    {
        if (!isAction.Value)
        {
            //return;
        }
        //Debug.Log("nobody true");
        currentRoom.Value.roomBattleManager.noBodyAction = true;
        isAction.Value = false;
        BuffTurnEnd();
    }
    #endregion

    #region 一些网络方法

    [ServerRpc(RequireOwnership = false)]
    public virtual void TakeDamage(Character caster, int damage, int count = 1)
    {
        //先减去防御力数值,如果比防御力低就变成1点伤害
        if (dodge.Value > 0)
        {
            dodge.Value -= 1;
            if (this is Enemy)
            {
                //DTextManager.Instance.CreateText(transform, "闪避");
                ServerDTextRpc("闪避");
            }
            if (count > 1)
            {
                TakeDamage(caster, damage, count - 1);
            }
            return;
        }

        if (damage > block.Value)
        {
            damage -= block.Value;
            block.Value = 0;
            //if (this is Enemy)
            //{
            //    //  DTextManager.Instance.CreateHurtText(transform, -damage);
            //    ServerDHurtTextRpc(-damage, transform.position);
            //}
            //else if (this is Player p)
            //{
            //    // DTextManager.Instance.CreateHurtText(p.player_B.AniUI.transform, -damage);
            //    ServerDHurtTextRpc(-damage, p.player_B.AniUI.transform.position);
            //}
        }
        else
        {
            block.Value -= damage;
            damage = 0;
        }
        HP.Value -= damage;
        if (HP.Value <= 0)
        {
            // Debug.Log(name + "死亡");
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
        if (count > 1)
        {
            // StartCoroutine(RepeatDamage(caster, damage, count - 1));
            TakeDamage(caster, damage, count - 1);
        }
    }

    private IEnumerator RepeatDamage(Character caster, int damage, int count)
    {
        yield return new WaitForSeconds(0.3f);
        TakeDamage(caster, damage, count);
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
    [ServerRpc(RequireOwnership = false)]
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
    [ServerRpc(RequireOwnership = false)]
    public virtual void TakeBlock(int i)
    {
        i = i + BlockEX();
        block.Value += i;
    }

    [ServerRpc(RequireOwnership = false)]
    public virtual void TakeAttackEX(int i)
    {
        // Debug.Log("力量EX增加" + i);
        attackEX.Value += i;
    }
    [ServerRpc(RequireOwnership = false)]
    public virtual void TakeAttackPercent(float i)
    {
        //Debug.Log("力量EX增加" + i);
        attackPercent.Value += i;
    }

    [ServerRpc(RequireOwnership = false)]
    public virtual void TakeGlory(int i)
    {
        if (glory.Value + i > 20)
        {
            Debug.Log("荣耀超过20,调整");
            glory.Value = 20;
        }
        else
        {
            glory.Value += i;
        }
    }


    #endregion

    #region Buff方法

    [ServerRpc(RequireOwnership = false)]
    public void AddBuff(Buff newBuff)
    {
        bool isNew = true;
        for (int i = 0; i < buffs.Count; i++)
        {
            if (buffs[i].buffName == newBuff.buffName)
            {
                isNew = false;
                buffs[i].value += newBuff.value;
                buffs[i].duration = newBuff.duration;
                BuffManager.Instance.GetBuffEffect(newBuff).Apply(this, newBuff.value);
                //newBuff.Apply(this, newBuff.value);
                //Debug.Log("这是老buff" + newBuff.buffName);
                break;
            }
        }
        if (isNew)
        {
            // Debug.Log("这是新buff" + newBuff.buffName);
            buffs.Add(newBuff);
            BuffManager.Instance.GetBuffEffect(newBuff).Apply(this, newBuff.value);
        }

    }
    [ServerRpc(RequireOwnership = false)]
    public void RemoveAllBuff()
    {
        foreach (var b in buffs)
        {
            BuffManager.Instance.GetBuffEffect(b).Remove(this, b.value);
        }
        buffs.Clear();
    }
    [ServerRpc(RequireOwnership = false)]
    public void BuffTurnStart()
    {
        foreach (var b in buffs)
        {
            BuffManager.Instance.GetBuffEffect(b).TurnStart(this, b.value);
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void BuffTurnEnd()
    {
        foreach (var b in buffs)
        {
            BuffManager.Instance.GetBuffEffect(b).TurnEnd(this, b.value);
        }
        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            var b = buffs[i];
            if (!b.forever)
            {
                b.duration--;
            }
            if (b.duration <= 0)
            {
                var effect = BuffManager.Instance.GetBuffEffect(b);
                effect.Remove(this, b.value);

                buffs.RemoveAt(i);
            }
        }
    }
    #endregion

    #region 客户端获取数值
    [Client]
    public virtual int Attack()//攻击里获得接口
    {
        int i = 0;
        i = Mathf.RoundToInt((attack.Value + attackEX.Value) * (attackPercent.Value + 1f));//因为浮点数不精确的原因,所以需要四舍五入而不是取上
        return i;
    }

    [Client]
    public virtual int Speed()
    {
        int i = 0;
        i = Mathf.RoundToInt((speed.Value + speedEX.Value) * (speedPercent.Value + 1f));//因为浮点数不精确的原因,所以需要四舍五入而不是取上
        return i;
    }
    [Client]
    public virtual int BlockEX()
    {
        int i = 0;
        i = blockEX.Value;
        return i;
    }

    #endregion
    [ServerRpc(RequireOwnership = false)]
    public void ServerDTextRpc(string s, Transform t = null)
    {
        ClientDText(s, t);
    }
    [ObserversRpc]
    public void ClientDText(string s, Transform t)
    {
        if (this.gameObject.activeSelf)
        {
            if (t)
            {
                DTextManager.Instance.CreateText(t, s);
            }
            else
            {
                DTextManager.Instance.CreateText(transform, s);
            }
        }
    }

    #region 回调
    public virtual void HP_OnChange(int prev, int next, bool asServer)
    {
        _HP = next;
        if (asServer)
        {
            if (this is Enemy)
            {
                DTextManager.Instance.CreateHurtText(transform, next - prev);
            }
            if (this is Player p)
            {
                if (p.player_B != null)
                {
                    DTextManager.Instance.CreateHurtText(p.player_B.transform, next - prev);
                }
            }
        }
        else
        {
            if (!IsServerStarted)
            {
                if (this is Enemy)
                {
                    DTextManager.Instance.CreateHurtText(transform, next - prev);
                }
                if (this is Player p)
                {
                    if (p.player_B != null)
                    {
                        DTextManager.Instance.CreateHurtText(p.player_B.transform, next - prev);
                    }
                }
            }
        }
    }
    public virtual void Block_OnChange(int prev, int next, bool asServer)
    {

    }
    public virtual void AttackPercent_OnChange(float prev, float next, bool asServer)
    {
    }

    public virtual void AttackEX_OnChange(int prev, int next, bool asServer)
    {
    }
    public virtual void Glory_OnChange(int prev, int next, bool asServer)
    {
    }
    #endregion
    [ServerRpc(RequireOwnership = false)]
    /// <summary>
    /// 重置状态
    /// </summary>
    public void InitState()
    {
        block.Value = 0;
        glory.Value = 0;
        dodge.Value = 0;
    }

    [ContextMenu("测试给BUff")]
    public void Test()
    {
        Buff buff = new Buff();
        buff.buffName = "力量Buff";
        buff.value = 5;
        buff.duration = 1;
        buff.forever = true;
        AddBuff(buff);
        Debug.Log(Attack());
    }
}
