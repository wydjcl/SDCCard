using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 管理房间生成
/// </summary>
public class MapManager : NetworkBehaviour
{
    public static MapManager instance;
    public MapDataSO mapData;
    public NetworkObject roomPrefab;
    public NetworkObject roomObjectPrefab;

    public GameObject mapGameObject;
    public GameObject viewport;
    public GameObject content;

    public GameObject exitButtom;
    public int seed;
    public System.Random rng;
    public Dictionary<Vector2Int, Room> rooms = new();

    public int width = 10;
    public int height = 10;
    public int mainPathLength = 8;
    public float branchChance = 0.4f;

    private Vector2Int[] dirs = new Vector2Int[]
  {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
  };
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        //  GameObject healthBarUI = GameObject.FindGameObjectWithTag("PlayerHPBarUI");
        // healthBarUI.gameObject.SetActive(true);
    }
    private void OnDisable()
    {
        //GameObject healthBarUI = GameObject.FindGameObjectWithTag("PlayerHPBarUI");
        //healthBarUI.gameObject.SetActive(false);
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (IsServerStarted)
        {
            seed = (int)(Random.value * int.MaxValue);
            Debug.Log("种子为:" + seed);
            rng = new System.Random(seed);
            Generate();
        }
    }

    [ContextMenu("生成房间1")]
    public void SpawnRooms()
    {
        var room = Instantiate(roomPrefab).GetComponent<Room>();
        room.gridPos.Value = new Vector2Int(3, 3);


        Spawn(room.gameObject, null, this.gameObject.scene);
    }

    [ContextMenu("生成地图")]
    public void Generate()
    {
        if (!IsServerStarted)
        {
            Debug.LogError("必须服务器生成！");
            return;
        }

        rooms.Clear();

        // ===== 1. 主路径 =====
        List<Vector2Int> mainPath = GenerateMainPath();

        //  ===== 2.分支 =====
        foreach (var pos in mainPath)
        {
            if (Random.value < branchChance)
                GenerateBranch(pos, Random.Range(2, 4));
        }


        // ===== 3. 创建房间 =====
        var keys = new List<Vector2Int>(rooms.Keys);
        foreach (var kv in keys)
        {
            CreateRoomObject(kv);
        }
        rooms[new Vector2Int(0, 0)].canExplore.Value = true;
        SetRandomExit();
        SetBoss();
        SetChests();
        Debug.Log($"生成房间数量: {rooms.Count}");
        ClientStart();
    }
    [ObserversRpc]
    public void ClientStart()
    {
        // rooms[new Vector2Int(0, 0)].ServerClick(GameManager.Instance.player);
    }
    [ServerRpc(RequireOwnership = false)]
    public void ClientMoveToRoomRpc(Player player, Vector2Int pos)
    {
        if (rooms.ContainsKey(pos))
        {
            rooms[pos].ServerClick(player);
        }
    }

    // =========================
    // 主路径（随机游走）
    // =========================
    private List<Vector2Int> GenerateMainPath()
    {
        List<Vector2Int> path = new();

        Vector2Int pos = Vector2Int.zero;
        path.Add(pos);
        rooms[pos] = null;

        for (int i = 0; i < mainPathLength; i++)
        {
            Vector2Int dir = dirs[rng.Next(0, dirs.Length)];
            Vector2Int next = pos + dir;

            if (rooms.ContainsKey(next))
                continue;

            pos = next;
            path.Add(pos);
            rooms[pos] = null;
        }

        return path;
    }

    // =========================
    // 分支生成
    // =========================
    private void GenerateBranch(Vector2Int start, int length)
    {
        Vector2Int pos = start;

        for (int i = 0; i < length; i++)
        {
            Vector2Int dir = dirs[Random.Range(0, dirs.Length)];
            Vector2Int next = pos + dir;

            if (rooms.ContainsKey(next))
                break;

            rooms[next] = null;
            pos = next;
        }
    }

    // =========================
    // 创建房间对象
    // =========================
    private void CreateRoomObject(Vector2Int pos)
    {
        bool up = rooms.ContainsKey(pos + Vector2Int.up);
        bool down = rooms.ContainsKey(pos + Vector2Int.down);
        bool left = rooms.ContainsKey(pos + Vector2Int.left);
        bool right = rooms.ContainsKey(pos + Vector2Int.right);

        RoomType type = GetRoomType(pos);

        Vector3 worldPos = new Vector3(pos.x * width, pos.y * height, 0);

        NetworkObject obj = Instantiate(roomPrefab, worldPos, Quaternion.identity);
        Room room = obj.GetComponent<Room>();
        room.Init(type, pos);

        Spawn(obj.gameObject, null, gameObject.scene);

        rooms[pos] = room;

        RoomObject roomObj = Instantiate(roomObjectPrefab).GetComponent<RoomObject>();

        room.roomObject.Value = roomObj;

        Spawn(roomObj.gameObject, null, gameObject.scene);

    }

    // =========================
    // 房间类型分配
    // =========================
    private RoomType GetRoomType(Vector2Int pos)
    {
        // 起点
        if (pos == Vector2Int.zero)
            return RoomType.Start;

        // 最远点当出口
        //if (pos.magnitude > 6)
        //    return RoomType.Exit;

        // 随机类型
        int r = rng.Next(0, 100);
        return RoomType.Chest;
        if (r < 30f) return RoomType.Normal;
        if (r < 60f) return RoomType.Chest;
        if (r < 65f) return RoomType.Task;
        if (r < 70f) return RoomType.Exit;//TODO至少弄几个离开通道
        return RoomType.Normal;
    }
    public void SetRandomExit()
    {
        List<Room> candidates = new List<Room>();

        foreach (var kv in rooms)
        {
            Room room = kv.Value;

            if (room == null) continue;

            // ❌ 排除 start 和 boss
            if (room.roomType.Value == RoomType.Start) continue;
            if (room.roomType.Value == RoomType.Boss) continue;

            candidates.Add(room);
        }

        if (candidates.Count == 0)
        {
            Debug.LogWarning("没有可用房间可设置 Exit");
            return;
        }
        int index = rng.Next(0, candidates.Count);
        Room selected = candidates[index];

        selected.roomType.Value = RoomType.Exit;
        Debug.Log("逃生房间为" + selected.gridPos.Value);
    }

    public void SetBoss()
    {
        List<Room> candidates = new List<Room>();

        foreach (var kv in rooms)
        {
            Room room = kv.Value;

            if (room == null) continue;

            // ❌ 排除 start 和 boss
            if (room.roomType.Value == RoomType.Start) continue;
            if (room.roomType.Value == RoomType.Exit) continue;

            candidates.Add(room);
        }

        if (candidates.Count == 0)
        {
            Debug.LogWarning("没有可用房间可设置Boss");
            return;
        }
        int index = rng.Next(0, candidates.Count);
        Room selected = candidates[index];

        selected.roomType.Value = RoomType.Boss;
        Debug.Log("Boss房间为" + selected.gridPos.Value);
    }

    public void SetChests()
    {
        foreach (var r in rooms)
        {
            if (r.Value.roomType.Value == RoomType.Chest)
            {
                // 1️⃣ 随机生成 1~4 个宝箱
                int chestCount = rng.Next(1, 5);

                for (int i = 0; i < chestCount; i++)
                {
                    float x = rng.Next(0, 13);
                    float y = rng.Next(-2, 6);
                    Vector2 spawnPos = new Vector2(x, y);

                    var c = Instantiate(Dic.Instance.chestPrefab, spawnPos, Quaternion.identity).GetComponent<Chest>();

                    c.room.Value = r.Value;

                    Spawn(c, null, this.gameObject.scene);

                    c.InitChest(mapData.GenerateDrops());
                }
            }
        }
    }
    /// <summary>
    /// 点击房间后开放周围房间
    /// </summary>
    /// <param name="pos"></param>
    [Server]
    public void OpenAroundRooms(Vector2Int pos)
    {
        Vector2Int[] dirs8 = new Vector2Int[]
    {
       // new Vector2Int(-1,  1), // 左上
        new Vector2Int( 0,  1), // 上
        //new Vector2Int( 1,  1), // 右上

        new Vector2Int(-1,  0), // 左
        new Vector2Int( 1,  0), // 右

       // new Vector2Int(-1, -1), // 左下
        new Vector2Int( 0, -1), // 下
        //new Vector2Int( 1, -1), // 右下
    };
        rooms[pos].explored.Value = true;
        foreach (var dir in dirs8)
        {
            Vector2Int target = pos + dir;

            if (rooms.ContainsKey(target))
            {
                rooms[target].canExplore.Value = true;
            }
        }
    }


    [ContextMenu("测试创造敌人")]
    public void Test()
    {

    }

}
