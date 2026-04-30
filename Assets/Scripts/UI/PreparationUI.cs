using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparationUI : UITemplate<PreparationUI>
{
    public bool localhost;
    public PlayerData data;
    public GameObject bagUI;
    public GameObject characterUI;

    public GameObject bag;
    public List<PropBox> bagBoxes = new List<PropBox>();
    public List<PropBox> wareHouseBoxes = new List<PropBox>();
    public int wareHousePage;
    protected override void Awake()
    {
        base.Awake();
        data = SaveData.Instance.data;
    }
    private void Enable()
    {
        InitBag();
        InitWareHouse();
    }
    private void OnEnable()
    {
        SaveData.Instance.SortAll();
        InitBag();
        InitWareHouse();
    }

    public void Init()
    {
        InitBag();
        InitWareHouse();
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
            propUI.isBag = true;
        }
    }
    public void InitWareHouse()
    {
        foreach (var item in wareHouseBoxes)
        {
            Destroy(item.propUI.gameObject);
        }
        for (int i = 0; i < data.warehouse.Count; i++)
        {
            //Debug.Log("生成ui");
            var propUI = Instantiate(Dic.Instance.propPrefab, wareHouseBoxes[i].transform).GetComponent<PropUI>();
            propUI.box = wareHouseBoxes[i];
            wareHouseBoxes[i].propUI = propUI.gameObject;

            propUI.transform.position = propUI.box.transform.position;
            propUI.propImage.sprite = Dic.Instance.GetPropData(data.warehouse[i].propName).propSprite;
            propUI.amountText.text = data.warehouse[i].amount.ToString();

            Props p = new Props();
            p.propName = data.warehouse[i].propName;
            p.amount = data.warehouse[i].amount;
            propUI.props = p;

            //propUI.propName = data.warehouse[i].propName;
            //propUI.stackAmount = data.warehouse[i].amount;
            propUI.isWarehouse = true;
        }
    }


    public void ClickBagUI()
    {
        bagUI.SetActive(true);
        characterUI.SetActive(false);
        InitBag();
        InitWareHouse();
    }
    public void ClickCharacterButtom()
    {
        bagUI.SetActive(false);
        characterUI.SetActive(true);
    }
    public void ChangeCharacterID0()
    {
        GameManager.Instance.player.ChangeCharacterID(0);
    }
    public void ChangeCharacterID1()
    {
        GameManager.Instance.player.ChangeCharacterID(1);
    }

}
