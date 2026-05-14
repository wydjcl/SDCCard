using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RewardCard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    public CardDataSO data;
    public Image cardImage;
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI cardDescription;
    public TextMeshProUGUI cardCost;

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.player.AddCard(data.cardName);
        GameManager.Instance.player.CreateCard();
        CardRewardUI.Instance.CloseUI();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

    }

    public void Init(CardDataSO cardData)
    {
        data = cardData;
        cardImage.sprite = data.cardImage;
        cardName.text = data.cardName;
        cardDescription.text = data.cardDes;
        cardCost.text = data.cardCost.ToString();
    }
}
