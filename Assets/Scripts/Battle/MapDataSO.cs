using FishNet.Object;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "地图配置表", menuName = "SO/地图/地图配置表")]
public class MapDataSO : ScriptableObject
{
    [Header("普通敌人队列")]
    public List<SpawnEnemyList> normalList = new List<SpawnEnemyList>();
    [Header("精英怪队列")]
    public List<SpawnEnemyList> jingyingList = new List<SpawnEnemyList>();
    [Header("Boss队列")]
    public List<SpawnEnemyList> BossList = new List<SpawnEnemyList>();
    public List<MapPropsList> mapPropsLists = new List<MapPropsList>();

    public List<Props> GenerateDrops()
    {
        List<MapProps> mapPropsList = GetRandomMapPropsList();
        List<Props> result = new List<Props>();

        if (mapPropsList == null || mapPropsList.Count == 0)
            return result;

        foreach (var mp in mapPropsList)
        {
            // 1️⃣ 判空保护
            if (mp == null || mp.propData == null)
                continue;

            // 2️⃣ 掉落概率判断
            if (UnityEngine.Random.value > mp.dropRate)
                continue;

            // 3️⃣ 生成掉落数量（包含最大值）
            int count = UnityEngine.Random.Range(mp.minDropNum, mp.maxDropNum + 1);

            if (count <= 0)
                continue;

            // 4️⃣ 加入结果
            Props props = new Props();
            props.propName = mp.propData.propName;
            props.amount = count;
            result.Add(props);
        }

        return result;
    }
    public List<MapProps> GetRandomMapPropsList()
    {
        var list = mapPropsLists;
        if (list == null || list.Count == 0)
            return null;

        // 1️⃣ 计算总权重
        int totalWeight = 0;
        foreach (var item in list)
        {
            if (item == null || item.weight <= 0)
                continue;

            totalWeight += item.weight;
        }

        if (totalWeight == 0)
            return null;

        // 2️⃣ 随机一个值
        int rand = UnityEngine.Random.Range(0, totalWeight);

        // 3️⃣ 命中区间
        int current = 0;
        foreach (var item in list)
        {
            if (item == null || item.weight <= 0)
                continue;

            current += item.weight;

            if (rand < current)
            {
                return item.mapPropsList;
            }
        }

        // 理论不会走到这里
        return null;
    }
}
[Serializable]
public class SpawnEnemyList
{
    public List<NetworkObject> enemies;
}
[Serializable]
public class MapProps
{
    public PropData propData;
    public int minDropNum;
    public int maxDropNum;
    public float dropRate;
}
[Serializable]
public class MapPropsList
{
    public List<MapProps> mapPropsList;
    public int weight;
}