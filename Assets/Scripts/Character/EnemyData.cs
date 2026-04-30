using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "敌人数据", menuName = "SO/敌人数据")]
public class EnemyData : ScriptableObject
{
    public int maxHP;
    public int attack;
    public int denfense;
    public int speed;
    public List<EnemyProp> enemyProps = new List<EnemyProp>();
    public List<Props> GetDropResult()
    {
        List<Props> result = new List<Props>();

        foreach (var drop in enemyProps)
        {
            if (Random.value <= drop.dropRate)
            {
                int amount = Random.Range(drop.amountMin, drop.amountMax + 1);

                // 🔥 查找是否已经存在该物品
                Props exist = result.Find(x => x.propName == drop.propData.propName);

                if (exist != null)
                {
                    exist.amount += amount;
                }
                else
                {
                    result.Add(new Props
                    {
                        propName = drop.propData.propName,
                        amount = amount
                    });
                }
            }
        }

        return result;
    }
}
[System.Serializable]
public class EnemyProp//敌人掉落物
{
    public PropData propData;
    public int amountMin;
    public int amountMax;
    public float dropRate;//掉落概率
}
