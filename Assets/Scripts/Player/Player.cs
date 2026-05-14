using DG.Tweening;
using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class Player : Character
{
    private InitialSceneUI initialSceneUI;
    public CardLayout cardLayout;

    public readonly SyncVar<string> playerName = new SyncVar<string>();
    public readonly SyncVar<int> characterID = new SyncVar<int>();
    public readonly SyncVar<bool> isGo = new SyncVar<bool>();//是否开始游戏了,开始游戏后初始场景UI就会关闭,或者开始游戏后可以使用道具


    public readonly SyncVar<Vector2Int> playerPos = new SyncVar<Vector2Int>();
    public readonly SyncVar<bool> isExit = new SyncVar<bool>();

    //战斗数据
    public readonly SyncVar<int> cost = new SyncVar<int>();


    public readonly SyncList<string> cardNameList = new SyncList<string>();//初始化的卡组
    public List<Card> handDeck = new List<Card>();
    public List<Card> drawDeck = new List<Card>();
    public List<Card> discardDeck = new List<Card>();
    public List<Card> removeDeck = new List<Card>();

    public GameObject healthBarPrefab;
    public TextMeshProUGUI mapPosText;

    public PlayerHealthBar healthBar;//血条
    public Player_B player_B;

    private GameObject mainUI;

    #region 生命周期
    private void Awake()
    {
        if (initialSceneUI == null)
        {
            initialSceneUI = FindAnyObjectByType<InitialSceneUI>();
        }
    }
    public override void OnStartServer()
    {
        base.OnStartServer();

        characterID.Value = -1;
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsOwner)
        {
            name = "本人玩家";
            GameManager.Instance.player = this;
            ChangeName(SaveData.Instance.data.playerName);
        }
        else
        {
            name = "非本人玩家";
        }

        healthBar = Instantiate(healthBarPrefab).GetComponent<PlayerHealthBar>();
        healthBar.player = this;

        //ChangeCharacterID(0);
        //bar.playerNameText.text = playerName.Value;
        GameManager.Instance.players.Add(this);



        playerName.OnChange += PlayerName_OnChange;
        characterID.OnChange += CharacterID_OnChange;
        playerPos.OnChange += PlayerPos_OnChange;


        InitDeck();//暂用,给卡组名
        cardLayout = FindAnyObjectByType<CardLayout>();
    }



    public override void OnStopClient()
    {
        base.OnStopClient();
        playerName.OnChange -= PlayerName_OnChange;
        characterID.OnChange -= CharacterID_OnChange;
        playerPos.OnChange -= PlayerPos_OnChange;
        if (healthBar != null)
        {
            Destroy(healthBar.gameObject);
        }
    }


    #endregion

    #region 网络方法
    [ServerRpc(RequireOwnership = false)]
    public void ChangeName(string n)
    {
        playerName.Value = n;
    }
    [ServerRpc(RequireOwnership = false)]
    public void ChangeCharacterID(int i)
    {
        characterID.Value = i;
        InitDeck();
        InitData(i);
    }
    [ServerRpc(RequireOwnership = false)]
    public void InitData(int i)
    {
        isExit.Value = false;
        isDead.Value = false;
        isAction.Value = false;
        isGo.Value = false;

        var data = Dic.Instance.characterDatas[i];
        maxHP.Value = data.maxHp;
        HP.Value = data.maxHp;
        attack.Value = data.attack;
        speed.Value = data.speed;
        cost.Value = data.cost;

    }
    [ServerRpc(RequireOwnership = false)]
    public void InitDeck()
    {
        if (characterID.Value == 0)
        {
            cardNameList.Clear();
            cardNameList.Add("给你一拳");
            //cardNameList.Add("给你一拳");
            //cardNameList.Add("给你一拳");
            //cardNameList.Add("给你一拳");
            //cardNameList.Add("给你一拳");
            cardNameList.Add("基础魔法屏障");
            // cardNameList.Add("基础魔法屏障");
            //cardNameList.Add("基础魔法屏障");
            //cardNameList.Add("基础魔法屏障");


            cardNameList.Add("发现宝箱");
            cardNameList.Add("发现宝箱");
            cardNameList.Add("发现宝箱");
            cardNameList.Add("发现宝箱");

            cardNameList.Add("战争怒吼");
            cardNameList.Add("旋风斩");
            cardNameList.Add("我身为盾");
            cardNameList.Add("临阵磨剑");
            cardNameList.Add("二连斩");
            cardNameList.Add("护盾猛击");
            cardNameList.Add("荣耀重击");
            cardNameList.Add("誓约胜利之剑!");
            cardNameList.Add("骑士斩");

        }
        if (characterID.Value == 1)
        {
            cardNameList.Clear();
            cardNameList.Add("发现宝箱");
            cardNameList.Add("发现宝箱");
            cardNameList.Add("发现宝箱");
            cardNameList.Add("发现宝箱");
            cardNameList.Add("发现宝箱");
            cardNameList.Add("发现宝箱");
            cardNameList.Add("发现宝箱");
            cardNameList.Add("发现宝箱");
            cardNameList.Add("发现宝箱");
            cardNameList.Add("发现宝箱");
            cardNameList.Add("发现宝箱");
            cardNameList.Add("发现宝箱");
        }
    }

    /// <summary>
    /// 关闭初始场景UI
    /// </summary>
    [ObserversRpc]
    public void DisableInitUI()
    {
        initialSceneUI.gameObject.SetActive(false);
    }
    [ObserversRpc]
    public void ClientOpenMainUI()
    {
        // mainUI = GameObject.FindGameObjectWithTag("MainUI");
        mainUI.gameObject.SetActive(true);
        foreach (var b in healthBar.ui.bars)
        {
            b.gameObject.SetActive(true);
        }
    }
    [ObserversRpc]
    public void ClientDisableMainUI()
    {
        mainUI = GameObject.FindGameObjectWithTag("MainUI");
        mainUI.gameObject.SetActive(false);
        SaveManager.Instance.SaveTest();
    }
    /// <summary>
    /// 进入某房间,传房间坐标
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void EnterRoom(Room room)
    {
        playerPos.Value = room.gridPos.Value;
        if (currentRoom.Value)
        {
            currentRoom.Value.characters.Remove(this);

        }
        currentRoom.Value = room;
        room.characters.Add(this);

        room.ServerPlayerIn(this, Owner);


        Debug.Log($"玩家*{playerName.Value}*进入了{room.gridPos.Value}坐标的房间");
    }
    [ServerRpc(RequireOwnership = false)]
    public void Exit()
    {
        isExit.Value = true;
        CheckExit();
    }
    [Server]
    public void CheckExit()
    {
        int i = 0;
        foreach (var p in GameManager.Instance.players)
        {
            if (p.isExit.Value || p.isDead.Value)
            {
                i++;
            }
        }
        if (i == InstanceFinder.ServerManager.Clients.Count)
        {
            Debug.Log("全部玩家逃离或者死亡");
            UnloadBattleScene();
        }
    }
    [Server]
    public void UnloadBattleScene()
    {
        GameManager.Instance.DespawnAllObjectByScene("BattleScene");
        SceneUnloadData unloadData = new SceneUnloadData("BattleScene");

        InstanceFinder.SceneManager.UnloadGlobalScenes(unloadData);//TODO回调

        //foreach (var p in GameManager.Instance.players)
        //{
        //    p.InitData(p.characterID.Value);
        //}
        ClientOpenMainUI();
        ClientUnloadBattleScene();
    }
    [ObserversRpc]
    public void ClientUnloadBattleScene()//客户端结束战斗场景
    {
        Debug.Log("客户端结束战斗场景");
        if (isDead.Value)
        {
            Debug.Log("死亡,清空背包");
            SaveData.Instance.data.bag.Clear();
        }
        InitData(characterID.Value);
        InitDeck();
        SaveManager.Instance.SaveTest();
    }

    #endregion




    #region 卡牌
    [ServerRpc(RequireOwnership = false)]
    public void AddCard(string cardName)
    {
        cardNameList.Add(cardName);
        Debug.Log($"{cardName}已经添加到玩家{this.playerName.Value}");
    }
    public void CreateCard()
    {
        var cardZone = GameObject.FindGameObjectWithTag("CardZone");

        for (int i = cardZone.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(cardZone.transform.GetChild(i).gameObject);
        }
        handDeck.Clear();
        discardDeck.Clear();
        drawDeck.Clear();
        removeDeck.Clear();


        foreach (var cardName in cardNameList)
        {
            foreach (var so in Dic.Instance.cards0)
            {
                if (cardName == so.cardName)
                {
                    var card = Instantiate(Dic.Instance.cardPrefab).GetComponent<Card>();
                    card.InitCard(so);
                    card.gameObject.SetActive(false);
                    drawDeck.Add(card);
                }
            }
        }

        ShuffleDrawDeck();
    }

    /// <summary>
    /// 为该连接玩家增加单张卡片,i为0的时候弃牌堆,i为1的时候抽牌堆,i为2的时候除外堆
    /// </summary>
    /// <param name="conn"></param>
    [TargetRpc]
    public void CreateOneCard(NetworkConnection conn, string cardName, int i)
    {
        //Debug.Log("只有这个玩家执行了方法！");
        //// 这里写客户端逻辑，比如播放动画、显示 UI
        //if (i == 0)
        //{
        //    Debug.Log("在弃牌堆插入");
        //    var cardP = Instantiate(cardPrefab, cardLayout.transform);
        //    cardP.SetActive(false);
        //    Card card = cardP.GetComponent<Card>();
        //    var so = Dic.Instance.FindCard(cardName);
        //    card.InitCard(so);
        //    discardDeck.Add(card);
        //}
        //if (i == 1)
        //{
        //    Debug.Log("在抽牌堆插入");
        //}
        //if (i == 2)
        //{
        //    Debug.Log("在除外堆插入");
        //}
    }


    public void DestroyCard()
    {
        var cardZone = GameObject.FindGameObjectWithTag("CardZone");

        for (int i = cardZone.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(cardZone.transform.GetChild(i).gameObject);
        }
        //TODO删除实体
        handDeck.Clear();
        discardDeck.Clear();
        drawDeck.Clear();
        removeDeck.Clear();
    }
    [ContextMenu("抽5牌")]
    public void DrawFive()
    {
        DrawCard(5);
    }
    public void DrawCard(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (drawDeck.Count == 0)
            {
                // 弃牌堆也空 → 没牌可抽，直接停止
                if (discardDeck.Count == 0)
                {
                    //Debug.Log("抽牌堆和弃牌堆都空了，无法继续抽牌");
                    break;
                }
                //Debug.Log("抽牌堆为空,洗牌");
                // 洗回抽牌堆
                ShuffleDiscardIntoDraw();
            }
            // 洗牌后仍然空 → 安全退出
            if (drawDeck.Count == 0)
            {
                //Debug.Log("洗牌后抽牌堆仍为空");
                break;
            }
            // 现在抽牌堆一定有牌，抽一张
            Card card = drawDeck[0];
            drawDeck.RemoveAt(0);
            handDeck.Add(card);
            card.gameObject.SetActive(true);
            card.transform.position = new Vector3(0, 0, 0);
            card.isAni = true;
            //var delay = i * 0.1f;
        }
        SetCardLayout(0);
    }
    private void SetCardLayout(float delay)
    {
        for (int i = 0; i < handDeck.Count; i++)
        {
            var currentCard = handDeck[i];
            currentCard.transform.DOKill();
        }//删去所有卡牌的动画
        for (int i = 0; i < handDeck.Count; i++)
        {
            var currentCard = handDeck[i];

            CardTransForm cardTransForm = cardLayout.GetCardTransForm(i, handDeck.Count);
            currentCard.transform.DOScale(Vector3.one, 0.05f).SetDelay(delay).onComplete = () =>
            {
                currentCard.transform.DOMove(cardTransForm.pos, 0.1f).onComplete = () =>
                {
                    currentCard.isAni = false;
                };
            };
            currentCard.GetComponent<SortingGroup>().sortingOrder = i;
            currentCard.orSortingOrder = i;
            currentCard.UpdatePosRot(cardTransForm.pos, cardTransForm.rotation);
        }
    }
    public void ShuffleDiscardIntoDraw()
    {
        drawDeck.AddRange(discardDeck);
        discardDeck.Clear();
        ShuffleDrawDeck();
    }
    /// <summary>
    /// 打乱抽牌堆的顺序（Fisher–Yates 洗牌）
    /// </summary>
    public void ShuffleDrawDeck()
    {
        if (drawDeck.Count <= 1)
            return;
        for (int i = 0; i < drawDeck.Count; i++)
        {
            int rand = Random.Range(i, drawDeck.Count);
            (drawDeck[i], drawDeck[rand]) = (drawDeck[rand], drawDeck[i]);
        }
    }
    public void DiscardCard(Card card)
    {
        discardDeck.Add(card);
        handDeck.Remove(card);
        card.gameObject.SetActive(false);
        SetCardLayout(0f);
    }

    public void RemoveCard(Card card)
    {
        removeDeck.Add(card);
        handDeck.Remove(card);
        card.gameObject.SetActive(false);
        SetCardLayout(0f);
    }

    public void DiscardAllCards()
    {
        // 倒序遍历手牌堆
        for (int i = handDeck.Count - 1; i >= 0; i--)
        {
            Card card = handDeck[i];

            // 移动到弃牌堆
            discardDeck.Add(card);

            // 从手牌堆移除
            handDeck.RemoveAt(i);

            // 隐藏卡牌
            card.gameObject.SetActive(false);
        }

        // 更新手牌布局
        SetCardLayout(0f);
    }
    #endregion

    #region 战斗

    public override void ServerTurnStart()
    {
        base.ServerTurnStart();
        ClientTurnStart(Owner);

    }
    [TargetRpc]
    public void ClientTurnStart(NetworkConnection conn)
    {
        BattleSceneManager.Instance.turnButtom.SetActive(true);
        DrawCard(10);
    }

    public override void ServerTurnEnd()
    {
        base.ServerTurnEnd();
        ClientTurnEnd(Owner);
    }
    [TargetRpc]
    public void ClientTurnEnd(NetworkConnection conn)
    {
        BattleSceneManager.Instance.turnButtom.SetActive(false);
        DiscardAllCards();
    }


    [ServerRpc(RequireOwnership = false)]
    public void ChangeCost(int i)
    {
        cost.Value += i;
    }


    #endregion

    #region 动画效果
    [ServerRpc(RequireOwnership = false)]
    public void ServerUseCardAniRpc(string cardName)
    {
        ClientUseCardAni(cardName);
    }
    [ObserversRpc]
    public void ClientUseCardAni(string cardName)
    {
        if (this.player_B.gameObject.activeSelf)
        {
            DTextManager.Instance.CreateText(player_B.AniUI.transform, cardName);

            DG.Tweening.Sequence seq = DOTween.Sequence();
            seq.SetLink(player_B.AniUI.gameObject);

            seq.Append(player_B.AniUI.transform.DORotate(new Vector3(0, 0, -15f), 0.2f).SetEase(Ease.InOutSine));
            seq.Append(player_B.AniUI.transform.DORotate(new Vector3(0, 0, 15f), 0.2f).SetEase(Ease.InOutSine));
            seq.Append(player_B.AniUI.transform.DORotate(Vector3.zero, 0.2f).SetEase(Ease.InOutSine));

        }
    }

    public override void ClientTakeDamageAni()
    {
        DG.Tweening.Sequence seq = DOTween.Sequence();
        seq.Append(player_B.playerSprite.DOFade(0.4f, 0.08f));
        seq.Append(player_B.playerSprite.DOFade(1f, 0.08f));
        seq.Append(player_B.playerSprite.DOFade(0.4f, 0.08f));
        seq.Append(player_B.playerSprite.DOFade(1f, 0.1f));
        seq.SetLink(player_B.playerSprite.gameObject);
    }



    #endregion





    #region 同步字段回调
    private void PlayerName_OnChange(string prev, string next, bool asServer)
    {
        //initialSceneUI.UpdateLayoutHealthBar();
        healthBar.playerNameText.text = playerName.Value;
    }
    private void CharacterID_OnChange(int prev, int next, bool asServer)
    {
        healthBar.avatar.sprite = Resources.Load<Sprite>($"PC_{characterID.Value}");
        if (!IsOwner)
        {
            return;
        }
        Image characterImage = GameObject.FindGameObjectWithTag("CharacterImage")?.GetComponent<Image>();
        if (characterImage)
        {
            characterImage.sprite = Resources.Load<Sprite>($"P_{characterID.Value}");
        }
        //initialSceneUI.characterImage.sprite = Resources.Load<Sprite>($"P_{characterID.Value}");
    }
    private void PlayerPos_OnChange(Vector2Int prev, Vector2Int next, bool asServer)
    {
        if (IsOwner)
        {
            if (mapPosText)
            {
                mapPosText.text = $"当前坐标{next.ToString()}";
            }
        }
    }

    public override void HP_OnChange(int prev, int next, bool asServer)
    {
        base.HP_OnChange(prev, next, asServer);

        if (healthBar)
        {
            if (block.Value <= 0)
            {
                healthBar.hpText.text = $"{HP.Value}/{maxHP.Value}";
            }
            else
            {
                healthBar.hpText.text = $"{HP.Value}/{maxHP.Value}({block.Value})";
            }
            healthBar.healthImage.fillAmount = (float)HP.Value / (float)maxHP.Value;
        }
        if (player_B)
        {
            player_B.hpText.text = HP.Value.ToString();
        }
    }

    public override void Block_OnChange(int prev, int next, bool asServer)
    {
        base.Block_OnChange(prev, next, asServer);
        if (healthBar)
        {
            if (block.Value <= 0)
            {
                healthBar.hpText.text = $"{HP.Value}/{maxHP.Value}";
            }
            else
            {
                healthBar.hpText.text = $"{HP.Value}/{maxHP.Value}({block.Value})";
            }
            healthBar.blockImage.fillAmount = (float)block.Value / (float)maxHP.Value;
            if (healthBar.blockImage.fillAmount > healthBar.healthImage.fillAmount)
            {
                healthBar.blockImage.fillAmount = healthBar.healthImage.fillAmount;
            }
        }
    }

    public override void AttackEX_OnChange(int prev, int next, bool asServer)
    {
        if (player_B)
        {
            player_B.attackText.text = Attack().ToString();
        }
    }
    public override void AttackPercent_OnChange(float prev, float next, bool asServer)
    {
        if (player_B)
        {
            player_B.attackText.text = Attack().ToString();
        }
    }
    public override void Glory_OnChange(int prev, int next, bool asServer)
    {
        if (asServer)
        {
            attackPercent.Value += (next - prev) * 0.01f;
            gloryBlockEX.Value = next / 5;
        }
    }
    #endregion
    [ContextMenu("测试输出isbattle")]
    public void DebugLogsb()
    {
    }
    [ContextMenu("测试增加荣耀值")]
    public void DebugLogs()
    {
        glory.Value += 50;
    }
    [ContextMenu("测试清除荣耀值")]
    public void DebugLogs2()
    {
        glory.Value = 0;
    }
}
