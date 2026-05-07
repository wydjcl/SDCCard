using DG.Tweening;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Room : NetworkBehaviour, IPointerClickHandler
{
    public MapManager mapManager;
    public RoomBattleManager roomBattleManager;

    public TextMeshProUGUI typeText;
    public Image roomImage;

    public readonly SyncVar<Vector2Int> gridPos = new SyncVar<Vector2Int>();//房间坐标
    public readonly SyncVar<bool> explored = new SyncVar<bool>();
    public readonly SyncVar<bool> canExplore = new SyncVar<bool>();
    public readonly SyncVar<RoomType> roomType = new SyncVar<RoomType>();

    public readonly SyncVar<RoomObject> roomObject = new SyncVar<RoomObject>();//房间实体
    [Header("战斗相关")]
    public readonly SyncVar<bool> isBattle = new SyncVar<bool>();
    public readonly SyncList<Character> characters = new SyncList<Character>();//房间内的角色对象列表,包括敌人和玩家

    public readonly SyncVar<bool> isBattled = new SyncVar<bool>();//生成过敌人了,避免重复生成    
    public readonly SyncVar<bool> isChested = new SyncVar<bool>();//生成过宝箱了,避免重复生成

    public Vector2 _gridPos;
    public bool _explored;
    public bool _canExplore;
    public bool _isBattle;
    public RoomType _roomType;
    public float timer;

    public override void OnStartClient()
    {
        base.OnStartClient();
        gridPos.OnChange += GridPos_OnChange;
        canExplore.OnChange += CanExplore_OnChange;
        explored.OnChange += Explored_OnChange;
        characters.OnChange += Characters_OnChange;
        mapManager = FindObjectOfType<MapManager>();
        transform.SetParent(mapManager.content.transform, false);
        if (roomType.Value == RoomType.Start)
        {
            roomImage.color = Color.white;
            //ServerClick(GameManager.Instance.player);

            if (IsServerStarted)
            {
                GameManager.Instance.player.currentRoom.Value = this;
                ServerClick(GameManager.Instance.player);
            }
            else
            {
                GameManager.Instance.player.EnterRoom(this);
            }
        }
    }



    public override void OnStopClient()
    {
        base.OnStopClient();
        gridPos.OnChange -= GridPos_OnChange;
        canExplore.OnChange -= CanExplore_OnChange;
        explored.OnChange -= Explored_OnChange;
        characters.OnChange -= Characters_OnChange;
    }

    private void Update()
    {
        _explored = explored.Value;
        _roomType = roomType.Value;
        _canExplore = canExplore.Value;
        _isBattle = isBattle.Value;
        typeText.text = _roomType.ToString() + "\n" + gridPos.Value;

        if (GameManager.Instance.player.currentRoom.Value != this)
        {
            return;
        }
        return;
    }

    private void GridPos_OnChange(Vector2Int prev, Vector2Int next, bool asServer)
    {
        _gridPos = gridPos.Value;
    }
    private void Explored_OnChange(bool prev, bool next, bool asServer)
    {
        if (explored.Value == true)
        {
            roomImage.color = Color.white;
        }
    }

    private void CanExplore_OnChange(bool prev, bool next, bool asServer)
    {
        if (canExplore.Value == true)
        {
            roomImage.color = Color.gray;
        }
    }
    private void Characters_OnChange(SyncListOperation op, int index, Character oldItem, Character newItem, bool asServer)
    {
        // ❗ 只在客户端处理
        //if (asServer) return;
        if (asServer)
        {
            // Debug.Log("服务器修改了角色列表,不排序");
        }
        // Debug.Log("客户端排序");

        // 👉 只关心“新增”
        if (op == SyncListOperation.Add && newItem is Enemy)
        {
            Debug.Log("有敌人加入，重新排序");
            ClientChangeEnemyPos();
        }
        if (GameManager.Instance.player.currentRoom.Value == this)
        {
            if (op == SyncListOperation.Add && newItem is Player playerAdd)
            {
                Debug.Log("玩家加入，显示血条");
                playerAdd.healthBar.gameObject.SetActive(true);
                playerAdd.healthBar.ui.UpdateLayoutHealthBar();

                playerAdd.player_B.gameObject.SetActive(true);//TODO动画结束后再显示
            }

            else if ((op == SyncListOperation.RemoveAt) && oldItem is Player playerRemove)
            {
                Debug.Log("玩家移除，隐藏血条");
                playerRemove.healthBar.gameObject.SetActive(false);
                playerRemove.healthBar.ui.UpdateLayoutHealthBar();

                playerRemove.player_B.gameObject.SetActive(false);
            }
        }


    }
    public void DebugLogs()
    {
        Debug.Log(gridPos.Value);
    }

    // ================================
    // 初始化（服务器调用）
    // ================================
    public void Init(RoomType type, Vector2Int pos
)
    {
        roomType.Value = type;
        gridPos.Value = pos;

        explored.Value = false;
    }



    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("点击房间");
        if (GameManager.Instance.player.isExit.Value)
        {
            return;
        }
        ServerClick(GameManager.Instance.player);
    }
    /// <summary>
    /// 服务端处理点击
    /// </summary>
    [ServerRpc(RequireOwnership = false)]
    public void ServerClick(Player player)
    {
        if (player.currentRoom.Value.isBattle.Value)
        {
            Debug.Log("玩家正身处战斗,无法移动");
            return;
        }
        bool nearby = false;
        Vector2Int[] dirs =
   {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
        Vector2Int.zero,
    };

        foreach (var dir in dirs)
        {
            if (player.playerPos.Value + dir == gridPos.Value)
                nearby = true;
        }

        if (canExplore.Value && nearby)//房间可以探索且就在旁边
        {
            player.EnterRoom(this);
            //TODO如果不遇敌,则周围房间开放探索
            mapManager.OpenAroundRooms(gridPos.Value);
        }
        else if (!nearby)
        {
            Debug.Log("不在范围,玩家坐标为" + player.playerPos.Value + "房间坐标为" + gridPos.Value);
        }
        else if (!canExplore.Value)
        {
            Debug.Log("未解锁");
        }
    }
    [ServerRpc(RequireOwnership = false)]
    /// <summary>
    /// 当玩家进入该房间,服务端执行
    /// </summary>
    public void ServerPlayerIn(Player player, NetworkConnection conn)
    {
        //激活房间
        if (roomType.Value == RoomType.Chest && !isChested.Value)
        {
            //CreateChestInRoom(this);
        }

        if (HaveAliveEnemy())
        {
            isBattle.Value = true;
        }
        else
        {
            if (roomType.Value != RoomType.Boss && roomType.Value != RoomType.Start && roomType.Value != RoomType.Exit)
            {
                int i = mapManager.rng.Next(0, 100);
                if (i < 20)//遇敌几率
                {
                    int r = Random.Range(1, 6);
                    for (int h = 0; h < r; h++)
                    {
                        int value = Random.Range(0, 2);
                        var e = Instantiate(Dic.Instance.enemies[value]);
                        e.transform.position = new Vector3(0, 30, 0);//在屏幕外生成,等战斗开始再移到房间里
                        var ee = e.GetComponent<Enemy>();
                        ee.currentRoom.Value = this;
                        ee.Entry.gameObject.SetActive(false);
                        characters.Add(ee);
                        Spawn(e, null, gameObject.scene);
                    }


                    //ClientPlayerInBattle(conn);
                    isBattle.Value = true;
                    isBattled.Value = true;
                    roomBattleManager.StartBattle();
                }
            }

        }

        ClientPlayerIn(conn);
    }
    /// <summary>
    /// 玩家进入房间后,客户端打开那个房间
    /// </summary>
    /// <param name="conn"></param>
    [TargetRpc]
    public void ClientPlayerIn(NetworkConnection conn)
    {


        foreach (var r in BattleSceneManager.Instance.roomObjects)
        {
            r.gameObject.SetActive(false);
            // r.transform.position = new Vector3(0, 30, 0);
        }
        roomObject.Value.gameObject.SetActive(true);
        roomObject.Value.back.sprite = Dic.Instance.forestSprites[Random.Range(0, Dic.Instance.forestSprites.Count)];
        //roomObject.Value.transform.position = new Vector3(0, 0, 0);
        if (roomType.Value == RoomType.Exit)
        {
            mapManager.exitButtom.SetActive(true);
        }
        else
        {
            mapManager.exitButtom.SetActive(false);
        }
        //进房间后显示房间的玩家对象,TODO添加动画
        foreach (var c in GameManager.Instance.players)
        {
            c.player_B.gameObject.SetActive(false);
        }

        foreach (var c in characters)
        {
            if (c is Player p)
            {
                p.player_B.gameObject.SetActive(true);
            }
        }

        foreach (var r in GameManager.Instance.player.healthBar.ui.bars)
        {
            if (r.gameObject != GameManager.Instance.player.healthBar.gameObject)
            {
                if (r.gameObject.GetComponent<PlayerHealthBar>().player.currentRoom.Value != GameManager.Instance.player.currentRoom.Value)
                {
                    r.gameObject.SetActive(false);
                }
                else
                {
                    r.gameObject.SetActive(true);
                }
            }
            else
            {
                r.gameObject.SetActive(true);
            }
        }
        GameManager.Instance.player.healthBar.ui.UpdateLayoutHealthBar();
    }
    public void ClientChangeEnemyPos()
    {
        List<Enemy> aliveEnemies = new List<Enemy>();
        foreach (var c in characters)
        {
            if (c is Enemy enemy)
            {
                if (!enemy.isDead.Value)
                {
                    aliveEnemies.Add(enemy);
                }
            }
        }
        var dir = new Vector2[]
        {
            new Vector2(1, -1),
            new Vector2(1, 3.5f),
            new Vector2(4.5f, -1),
            new Vector2(4.5f, 3.5f),
             new Vector2(8f, -1),
            new Vector2(8f, 3.5f),
             new Vector2(11.5f, -1),
            new Vector2(11.5f, 3.5f),

        };

        //if (aliveEnemies.Count > 0)
        //{
        //    aliveEnemies[0].transform.position = new Vector3(0, 3, 0);//TODO根据存活敌人改变位置
        //    aliveEnemies[1].transform.position = new Vector3(-5, 3, 0);
        //    aliveEnemies[2].transform.position = new Vector3(-5, 3, 0);
        //}

        for (int i = 0; i < aliveEnemies.Count; i++)
        {
            aliveEnemies[i].transform.position = dir[i];
        }
        //float baseSpacing = 7;
        //float compress = Mathf.Clamp(1f - aliveEnemies.Count * 0.05f, 0.3f, 1f);//压缩系数,0-0.5,最后乘上基础距离

        //for (int i = 0; i < aliveEnemies.Count; i++)
        //{
        //    int order = (i + 1) / 2;

        //    float offset = order * baseSpacing * compress;

        //    float x = 0;
        //    float y = order * 0.75f;

        //    if (i == 0)
        //        x = 0;
        //    else if (i % 2 == 1)
        //        x = offset;
        //    else
        //        x = -offset;

        //    // ⭐ 关键：Z 层级
        //    float z = i * 0.1f;

        //    aliveEnemies[i].transform.position = new Vector3(x, y, z);
        //    aliveEnemies[i].sortingGroup.sortingOrder = -i; // 确保后生成的敌人覆盖前一个
        //}
    }
    /// <summary>
    /// 开始战斗后,创建卡片,暂无用
    /// </summary>
    /// <param name="conn"></param>
    [TargetRpc]
    public void ClientPlayerInBattle(NetworkConnection conn)
    {
        Debug.Log("开始战斗初始化!!!");
        //BattleSceneManager.Instance.turnButtom.SetActive(true);
        //TODO出现回合结束按钮
    }
    /// <summary>
    /// 检查房间是否有存活敌人
    /// </summary>
    /// <returns></returns>
    public bool HaveAliveEnemy()
    {
        foreach (var c in characters)
        {
            if (c is Enemy enemy)
            {
                if (!enemy.isDead.Value)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void CreateChestInRoom(Room room)//为房间生成宝物
    {
        // Debug.Log("生成宝箱");
        room.isChested.Value = true;
        var r = mapManager.rng.Next(1, 4);
        for (int i = 0; i < r; i++)
        {
            var c = Instantiate(Dic.Instance.chestPrefab).GetComponent<Chest>();
            c.room.Value = room;
            if (i == 1)
            {
                c.transform.position = new Vector3(-5, 0, 0);
            }
            if (i == 0)
            {
                c.transform.position = new Vector3(0, 0, 0);
            }
            if (i == 2)
            {
                c.transform.position = new Vector3(5, 0, 0);
            }
            Spawn(c, null, this.gameObject.scene);
        }


    }
    [ContextMenu("测试有几个角色")]
    public void Test1()
    {
        foreach (var c in characters)
        {
            Debug.Log(c.name);
        }
        Debug.Log("总共有" + characters.Count + "个角色对象在该房间");
    }

}
