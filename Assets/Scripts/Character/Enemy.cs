using DG.Tweening;
using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Enemy : Character
{
    // public BattleSceneManager battleSceneManager;
    [Header("需要导入")]
    public EnemyData enemyData;
    public GameObject Entry;
    public GameObject aniUI;

    public SpriteRenderer enemySprite;
    public SortingGroup sortingGroup;
    public TextMeshPro HPText;
    public TextMeshPro AttackText;


    //血条效果
    [HideInInspector]
    public float targetFill;
    [HideInInspector]
    public float currentFill;
    public override void OnStartServer()
    {
        base.OnStartServer();
        HP.Value = enemyData.maxHP;
        maxHP.Value = enemyData.maxHP;
        speed.Value = enemyData.speed;
        attack.Value = enemyData.attack;
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        // battleSceneManager = FindObjectOfType<BattleSceneManager>();
        transform.SetParent(currentRoom.Value.roomObject.Value.transform, false);
        Entry.gameObject.SetActive(true);
        gameObject.SetActive(true);
        // currentRoom.Value.ClientChangeEnemyPos();
    }
    protected virtual void Update()
    {
        currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * 11f);

        AttackText.text = Attack().ToString();
    }
    //[ServerRpc(RequireOwnership = false)]
    //public override void TurnStart()
    //{
    //    block.Value = 0;//回合开始格挡值清零
    //    isAction.Value = true;
    //    Act();

    //}
    public override void ServerTurnStart()
    {
        base.ServerTurnStart();
        //isAction.Value = true;
        Act();
    }
    //[ServerRpc(RequireOwnership = false)]
    //public override void TurnEnd()
    //{
    //    // Debug.Log("服务端玩家回合结束");
    //    if (currentRoom.Value.roomBattleManager)
    //    {
    //        //Debug.Log("房间坐标" + currentRoom.Value.gridPos.Value);
    //    }
    //    isAction.Value = false;
    //    currentRoom.Value.roomBattleManager.noBodyAction = true;
    //    //Debug.Log("房间Nobody" + currentRoom.Value.roomBattleManager.noBodyAction);
    //}
    public override void ServerTurnEnd()
    {
        base.ServerTurnEnd();
    }
    [Server]
    public virtual void Act()
    {
        Invoke("ServerMove", 0.4f);
        // ServerMove();
        // ServerMove();
        ClientAni();
    }
    [ObserversRpc]
    public virtual void ClientAni()
    {
        if (GameManager.Instance.player.currentRoom.Value == currentRoom.Value)
        {
            StartCoroutine(Ani());
        }
    }
    public virtual IEnumerator Ani()//默认攻击动画,变大变小
    {
        // Debug.Log("执行动画");
        yield return null;
        DG.Tweening.Sequence seq = DOTween.Sequence();


        //seq.SetLink(aniUI);
        //seq.Append(aniUI.transform.DOScale(1.2f, 0.3f));
        //seq.Append(aniUI.transform.DOScale(1f, 0.3f));

        seq.SetLink(aniUI.gameObject);

        seq.Append(aniUI.transform.DORotate(new Vector3(0, 0, 15f), 0.2f).SetEase(Ease.InOutSine));
        //seq.Append(aniUI.transform.DORotate(new Vector3(0, 0, -15f), 0.2f).SetEase(Ease.InOutSine));
        seq.Append(aniUI.transform.DORotate(Vector3.zero, 0.2f).SetEase(Ease.InOutSine));

        yield return seq.WaitForCompletion(); // ⭐关键

    }
    [Server]
    public virtual void ServerMove()//服务器处理敌人行动
    {

    }
    public override void ServerEnemyDead()
    {
        var c = Instantiate(Dic.Instance.chestPrefab, this.transform.position, Quaternion.identity).GetComponent<Chest>();
        c.room.Value = currentRoom.Value;
        Spawn(c, null, this.gameObject.scene);
        c.InitChest(enemyData.GetDropResult());
    }
    public override void ClientTakeDamageAni()
    {
        DG.Tweening.Sequence seq = DOTween.Sequence();


        seq.Append(enemySprite.DOFade(0.4f, 0.08f));
        seq.Append(enemySprite.DOFade(1f, 0.08f));
        seq.Append(enemySprite.DOFade(0.4f, 0.08f));
        seq.Append(enemySprite.DOFade(1f, 0.1f));
        seq.SetLink(enemySprite.gameObject);
    }


    public override void HP_OnChange(int prev, int next, bool asServer)
    {
        targetFill = (float)next / (float)maxHP.Value;
        HPText.text = next.ToString();
    }
    public override void AttackEX_OnChange(int prev, int next, bool asServer)
    {
        AttackText.text = Attack().ToString();
    }
    public override void AttackPercent_OnChange(float prev, float next, bool asServer)
    {
        AttackText.text = Attack().ToString();
    }
}
