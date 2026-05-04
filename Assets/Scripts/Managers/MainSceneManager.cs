using FishNet;
using FishNet.Managing.Scened;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainSceneManager : NetworkBehaviour
{
    // Start is called before the first frame update
    public GameObject battleRPCImage;
    public TextMeshProUGUI battleRPCText;
    public int voteCount;//InstanceFinder.ServerManager.Clients.Count;

    public GameObject preparationUI;
    public override void OnStartClient()
    {
        base.OnStartClient();
        GameManager.Instance.player.ChangeCharacterID(0);
    }
    #region 出发
    public void ClickBattle()
    {
        if (InstanceFinder.ServerManager.Clients.Count == 1)
        {
            StartBattleMap();
            return;
        }


        if (IsServerStarted)
        {
            voteCount = 0;
            BattleRPC(GameManager.Instance.player.playerName.Value);
        }
    }
    [ObserversRpc]
    private void BattleRPC(string playerName)
    {
        if (IsServerStarted)
        {
            return;
        }
        battleRPCImage.SetActive(true);
        battleRPCText.text = $"{playerName}邀请您前往地点A";
    }
    [Client]
    public void ClientRpc()
    {
        ClientVote();
        battleRPCImage.SetActive(false);
    }
    [Client]
    public void CloseRpc()
    {
        battleRPCImage.SetActive(false);
        ClientRefuseVote(GameManager.Instance.player.playerName.Value);
    }
    [ServerRpc(RequireOwnership = false)]
    public void ClientVote()
    {
        voteCount++;
        if (voteCount >= InstanceFinder.ServerManager.Clients.Count - 1)
        {
            StartBattleMap();
            voteCount = 0;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void ClientRefuseVote(string playerName)
    {
        voteCount = 0;
        Debug.Log($"玩家{playerName}拒绝了");
    }
    [Server]
    public void StartBattleMap()
    {
        SceneLoadData loadData = new SceneLoadData("BattleScene");
        InstanceFinder.SceneManager.LoadGlobalScenes(loadData);
        GameManager.Instance.player.ClientDisableMainUI();
    }
    #endregion
    #region 战备
    [Client]
    public void ClickPreparationButtom()
    {
        preparationUI.SetActive(true);
    }



    #endregion  

    public void ClickBagButton()
    {
        BagUI.Instance.OpenWithWareHouse();
    }
}
