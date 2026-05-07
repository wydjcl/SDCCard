using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "玩家数据", menuName = "SO/玩家数据")]
public class PData : ScriptableObject
{
    public Sprite playerSprite;
    public int maxHp;
    public int attack;
    public int cost;
    public int speed;
    public List<CardDataSO> cards;


}
