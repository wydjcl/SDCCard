using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObject : NetworkBehaviour
{
    public BattleSceneManager battleSceneManager;
    public GameObject Entry;
    public SpriteRenderer back;
    public override void OnStartClient()
    {
        base.OnStartClient();
        battleSceneManager = FindObjectOfType<BattleSceneManager>();
        transform.SetParent(battleSceneManager.roomRoot.transform);
        battleSceneManager.roomObjects.Add(this);
        //gameObject.SetActive(false);

    }
}
