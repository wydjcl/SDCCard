using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleScenePlayerUI : UITemplate<BattleScenePlayerUI>
{
    public GameObject chestRoot;
    public Chest chest;
    public List<PropBox> bagBoxes = new List<PropBox>();
    public List<PropBox> chestBoxes = new List<PropBox>();

    public PlayerData data;
    protected override void Awake()
    {
        base.Awake();
        data = SaveData.Instance.data;
    }
    private void OnEnable()
    {
        if (chest)
        {
            Init();
        }
        else
        {
            InitBag();
        }
    }
    private void OnDisable()
    {
        if (chest)
        {
            chest.ClientCloseRpc(GameManager.Instance.player);
        }
        chest = null;
    }

    public void Init()
    {
        InitBag();
        InitChest(chest.propsList);
    }

    public void InitBag()
    {
        foreach (var item in bagBoxes)
        {
            Destroy(item.propUI.gameObject);
        }
        for (int i = 0; i < data.bag.Count; i++)
        {

            var propUI = Instantiate(Dic.Instance.propPrefab, bagBoxes[i].transform).GetComponent<PropUI>();
            propUI.box = bagBoxes[i];
            bagBoxes[i].propUI = propUI.gameObject;

            propUI.transform.position = propUI.box.transform.position;
            propUI.propImage.sprite = Dic.Instance.GetPropData(data.bag[i].propName).propSprite;
            propUI.amountText.text = data.bag[i].amount.ToString();
            Props p = new Props();
            p.propName = data.bag[i].propName;
            p.amount = data.bag[i].amount;
            propUI.props = p;
        }
    }
    public void InitChest(List<Props> props)
    {
        chestRoot.SetActive(true);
        foreach (var item in chestBoxes)
        {
            Destroy(item.propUI.gameObject);
        }
        for (int i = 0; i < props.Count; i++)
        {

            var propUI = Instantiate(Dic.Instance.propPrefab, chestBoxes[i].transform).GetComponent<PropUI>();
            propUI.box = chestBoxes[i];
            chestBoxes[i].propUI = propUI.gameObject;

            propUI.transform.position = propUI.box.transform.position;
            propUI.propImage.sprite = Dic.Instance.GetPropData(props[i].propName).propSprite;
            propUI.amountText.text = props[i].amount.ToString();
            Props p = new Props();
            p.propName = props[i].propName;
            p.amount = props[i].amount;
            propUI.props = p;

            propUI.isChest = true;
        }
    }

    public void TakeChest(Player player, string propName, int amount)
    {

    }
}
