using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "治疗", menuName = "SO/道具效果/治疗")]
public class P_Healing : PropEffect
{
    public int healAmount;
    public override void ApplyEffect(Player player)
    {
        player.Heal(healAmount);
    }
}
