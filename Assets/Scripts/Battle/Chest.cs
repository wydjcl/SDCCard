using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventTrigger;

public class Chest : NetworkBehaviour, IPointerClickHandler
{
    public readonly SyncVar<Room> room = new SyncVar<Room>();
    public List<Props> propsList = new List<Props>();
    public readonly SyncVar<bool> isLocked = new SyncVar<bool>();
    public GameObject Entry;

    public override void OnStartServer()
    {
        base.OnStartServer();

    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        transform.SetParent(room.Value.roomObject.Value.transform);
        Entry.gameObject.SetActive(true);
        if (transform.position.y > 10)
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);//如果生成在空中则放置在地面上
        }
        return;
    }
    [ObserversRpc]
    public void InitChest(List<Props> ps)
    {

        if (IsServerStarted)
        {
            if (ps.Count == 0)
            {
                Debug.Log("空宝藏");
                Despawn();
            }
        }
        //Debug.Log("放入宝藏");
        if (ps.Count > 0)
        {
            // Debug.Log(ps[0].propName);
        }
        Entry.gameObject.SetActive(true);
        //if (IsServerStarted)
        //{
        //    //Debug.Log("服务端不需要再加载");
        //    return;
        //}
        //Debug.Log("客户端加载宝箱");
        propsList.Clear();
        propsList.AddRange(ps);
    }
    [ServerRpc(RequireOwnership = false)]
    public void ClientSyncChestListRpc(Player player, List<Props> ps)
    {

        ServerSyncChestList(player, ps);

    }
    [ObserversRpc]
    public void ServerSyncChestList(Player player, List<Props> ps)
    {

        if (ps.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }
        bool haveProp = false;
        foreach (Props p in ps)
        {
            if (string.IsNullOrEmpty(p.propName))
            {

            }
            else
            {
                haveProp = true;
            }
        }
        if (!haveProp)
        {
            gameObject.SetActive(false);
            return;
        }
        if (player == GameManager.Instance.player)
        {
            //Debug.Log("本地不需要在同步");
        }
        else
        {
            // Debug.Log("其他客户端同步");
            propsList.Clear();
            propsList.AddRange(ps);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isLocked.Value)
        {
            //Debug.Log("宝箱已锁定");
            return;
        }
        if (GameManager.Instance.player.isAction.Value)
        {
            Debug.Log("正在回合中,无法搜刮");
            return;
        }
        ClientOpenRpc(GameManager.Instance.player);

        // BattleSceneManager.Instance.ChangePlayerUIState(this);
        BagUI.Instance.OpenWithChest(this);
        //BagUI.Instance.chest = this;
    }

    private void Update()
    {
        //var roomObj = room.Value?.roomObject.Value;

        //if (roomObj == null) return;

        //// 可选：确保 NetworkObject 已经初始化
        //if (!roomObj.IsSpawned) return;

        //if (!transform.parent)
        //{
        //    if (room.Value?.roomObject.Value?.transform != null)
        //    {
        //        transform.SetParent(room.Value.roomObject.Value.transform);
        //    }
        //    else
        //    {
        //        Debug.Log("无room或者room无Object");
        //    }

        //}
        //else
        //{
        //    Debug.Log("有父节点");
        //}
    }
    [ServerRpc(RequireOwnership = false)]
    public void ClientOpenRpc(Player player)
    {
        isLocked.Value = true;
        player.isSkip.Value = true;
        transform.SetParent(room.Value.roomObject.Value.transform);
    }
    [ServerRpc(RequireOwnership = false)]
    public void ClientCloseRpc(Player player)
    {
        isLocked.Value = false;
        player.isSkip.Value = false;
    }
    [ServerRpc(RequireOwnership = false)]
    public void TakeProp(Player player, string propName, int amount)
    {
        //if (lastBagAmount == 0)
        //{
        //    return;
        //}
        //if (lastBagAmount < amount)
        //{
        //    amount = lastBagAmount;
        //}
        for (int i = 0; i < propsList.Count; i++)
        {
            if (propsList[i].propName == propName)
            {
                propsList[i].amount -= amount;
                if (propsList[i].amount <= 0)
                {
                    propsList.RemoveAt(i);
                }
                break;
            }
        }
        ClientTakePropRpc(player.Owner, propName, amount);
        ClientSyncChestListRpc(player, propsList);
    }

    [TargetRpc]
    public void ClientTakePropRpc(NetworkConnection conn, string propName, int amount)
    {
        //TODO显示获得物品
        //SaveData.Instance.data.bag.Add(new Props { propName = propName, amount = amount });
        //SaveData.Instance.Sort(SaveData.Instance.data.bag);
        //BattleScenePlayerUI.Instance.Init();
    }

    [ContextMenu("输出房间")]
    public void Test()
    {
        if (room.Value)
        {
            Debug.Log("有房间");
        }
        if (room.Value?.roomObject.Value?.transform == null)
        {
            Debug.Log("无Object");
        }
    }
}
