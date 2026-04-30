using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CardData", menuName = "SO/卡牌Data")]
public class CardDataSO : ScriptableObject
{
    public string cardName;
    public int cardCost;
    public int cardCoin;
    public CardType cardType;
    public Sprite cardImage;
    [TextArea]
    public string cardDes;

    public List<CardEffectSO> effects;
}
