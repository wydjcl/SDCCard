using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
/// <summary>
/// 战斗中的角色UI
/// </summary>
public class Player_B : MonoBehaviour
{
    public Player player;
    public GameObject Entry;
    public GameObject AniUI;
    public SpriteRenderer playerSprite;

    public TextMeshPro attackText;
    public TextMeshPro hpText;
    public TextMeshPro BlockText;
    public void Init(Player p)
    {
        player = p;
        p.player_B = this;
        transform.SetParent(GameObject.FindGameObjectWithTag("PlayerZone").transform);
        playerSprite.sprite = Dic.Instance.characterDatas[p.characterID.Value].playerSprite;

        attackText.text = p.Attack().ToString();
        hpText.text = p.HP.Value.ToString();
        //playerSprite.sprite =

    }

}
