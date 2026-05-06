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
    public GameObject cardPrefab;
    public GameObject dynamicTextPrefab;
    public GameObject propPrefab;
    public GameObject propBoxPrefab;
    public NetworkObject chestPrefab;
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
