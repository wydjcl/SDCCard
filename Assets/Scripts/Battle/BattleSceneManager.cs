using FishNet;
using FishNet.Managing.Scened;
using FishNet.Object;
using GameKit.Dependencies.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 管理战斗场景UI
/// </summary>
public class BattleSceneManager : MonoBehaviour
{
    public static BattleSceneManager Instance;

    public MapManager mapManager;

    public RectTransform mapRT;
    public GameObject turnButtom;
    [Header("需导入UI")]
    public GameObject map;
    public BattleScenePlayerUI playerUI;

    public bool isBattle;
    public RoomObjectRoot roomRoot;
    public GameObject enemyRoot;
    public List<RoomObject> roomObjects;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        GameManager.Instance.player.CreateCard();
    }
    void Update()
    {
        if (GameManager.Instance.player.isExit.Value)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            ChangeMapState();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            ChangePlayerUIState();
        }

        //WSAD移动
        if (Vector2.Distance(mapRT.anchoredPosition, Vector2.zero) < 0.1f)
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                mapManager.ClientMoveToRoomRpc(GameManager.Instance.player, GameManager.Instance.player.currentRoom.Value.gridPos.Value + Vector2Int.up);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                mapManager.ClientMoveToRoomRpc(GameManager.Instance.player, GameManager.Instance.player.currentRoom.Value.gridPos.Value + Vector2Int.left);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                mapManager.ClientMoveToRoomRpc(GameManager.Instance.player, GameManager.Instance.player.currentRoom.Value.gridPos.Value + Vector2Int.down);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                mapManager.ClientMoveToRoomRpc(GameManager.Instance.player, GameManager.Instance.player.currentRoom.Value.gridPos.Value + Vector2Int.right);
            }
        }
    }

    public void OpenMap()
    {
        //map.gameObject.SetActive(true);
        var rt = map.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, 0);
    }

    public void CloseMap()
    {
        var rt = map.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, 1000);
    }

    public void ChangeMapState()
    {
        // map.gameObject.SetActive(!map.activeSelf);
        var rt = map.GetComponent<RectTransform>();

        if (Vector2.Distance(rt.anchoredPosition, Vector2.zero) < 0.1f)
        {
            rt.anchoredPosition = new Vector2(0, 1000);
        }
        else
        {
            rt.anchoredPosition = Vector2.zero;
        }
    }
    public void ChangePlayerUIState()
    {
        playerUI.gameObject.SetActive(!playerUI.gameObject.activeSelf);
        playerUI.chestRoot.SetActive(false);
    }
    public void ChangePlayerUIState(Chest chest)
    {
        playerUI.chest = chest;
        playerUI.gameObject.SetActive(true);
    }



    public void PlayerEndTurn()
    {
        GameManager.Instance.player.TurnEnd();
    }
    public void ExitBattle()
    {
        GameManager.Instance.player.Exit();
    }

}
