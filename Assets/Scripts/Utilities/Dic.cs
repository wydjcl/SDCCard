using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
/// <summary>
/// 字典,存储一些全局的静态数据
/// </summary>
public class Dic : SingletonMono<Dic>
{
    [Header("卡牌预制体")]
    public GameObject cardPrefab;
    public GameObject dynamicTextPrefab;
    [Header("道具预制体")]
    public GameObject propPrefab;
    [Header("道具格子预制体")]
    public GameObject propBoxPrefab;
    [Header("宝藏点预制体")]
    public NetworkObject chestPrefab;
    [Header("战斗中玩家预制体")]
    public GameObject player_BPrefab;

    public List<CharacterData> characterDatas = new List<CharacterData>();
    /// <summary>
    /// 敌人
    /// </summary>
    public List<NetworkObject> enemies = new List<NetworkObject>();
    public List<CardDataSO> cards0 = new List<CardDataSO>();
    public List<PropData> propDatas = new List<PropData>();
    [Header("战斗场景背景图队列")]
    public List<Sprite> forestSprites = new List<Sprite>();
    /// <summary>
    /// Server端调用,根据id加载角色的数据
    /// </summary>
    public void LoadCharacterData()
    {

    }
    //public PropData GetPropData(Prop prop)
    //{
    //    foreach (var p in propDatas)
    //    {
    //        if (p.propName == prop.propName)
    //        {
    //            return p;
    //        }
    //    }
    //    Debug.LogWarning("没找到对应道具!!!");
    //    return null;
    //}
    public PropData GetPropData(string propName)
    {
        foreach (var p in propDatas)
        {
            if (p.propName == propName)
            {
                return p;
            }
        }
        Debug.LogWarning("没找到对应道具!!!");
        return null;
    }
    public PropType GetPropType(string propName)
    {
        foreach (var p in propDatas)
        {
            if (p.propName == propName)
            {
                return p.propType;
            }
        }
        Debug.LogWarning("没找到对应道具!!!");
        return PropType.Normal;
    }
}
