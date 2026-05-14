using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRewardUI : UITemplate<CardRewardUI>
{
    private List<CardDataSO> datas = new List<CardDataSO>();
    public GameObject root;
    public RewardCard rewardCard0;
    public RewardCard rewardCard1;
    public RewardCard rewardCard2;
    public void Open()
    {
        datas = Dic.Instance.TakeCharacterCardList();
        root.gameObject.SetActive(true);
        InitCards();
    }
    public void InitCards()
    {
        for (int i = 0; i < 3; i++)
        {
            int index = Random.Range(0, datas.Count);
            CardDataSO item = datas[index];
            if (i == 0)
            {
                rewardCard0.Init(item);
            }
            if (i == 1)
            {
                rewardCard1.Init(item);
            }
            if (i == 2)
            {
                rewardCard2.Init(item);
            }
            datas.RemoveAt(index);
        }

    }
}
