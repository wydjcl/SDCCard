using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "获得卡牌奖励", menuName = "SO/道具效果/获得卡牌奖励")]
public class P_RewardCards : PropEffect
{
    public override void ApplyEffect(Player player)
    {
        CardRewardUI.Instance.Open();
    }
}
