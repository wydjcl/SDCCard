using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BagUI : UITemplate<BagUI>
{
    public GameObject root;
    public GameObject wareHouseUI;
    public GameObject chestUI;
    private PlayerData data;
    public List<PropBox> bagBoxes = new List<PropBox>();
    public List<PropBox> wareHouseBoxes = new List<PropBox>();
    public List<PropBox> chestBoxes = new List<PropBox>();

    public Chest chest;

    public bool isWareHouse;
    public bool isChest;
    public bool isBattle;
    public bool isShop;

    //public TextMeshProUGUI text;//TODO切换界面时显示不同的文本
    private void Start()
    {
        data = SaveData.Instance.data;
        root.SetActive(false);
        wareHouseUI.gameObject.SetActive(false);
        chestUI.gameObject.SetActive(false);
    }
    public void Open()//战斗时候打开背包
    {
        isBattle = true;
        root.SetActive(true);
        InitBag();
    }
    public void OpenWithWareHouse()//仓库界面打开背包
    {
        isWareHouse = true;
        root.SetActive(true);
        wareHouseUI.gameObject.SetActive(true);
        InitBag();
        InitWareHouse();
    }
    public void OpenWithChest(Chest c)//战斗时宝箱界面打开背包
    {
        isBattle = true;
        isChest = true;
        root.SetActive(true);
        chestUI.gameObject.SetActive(true);
        chest = c;
        InitBag();
        InitChest(c);
    }
    public void Close()
    {
        isWareHouse = false;
        isChest = false;
        isBattle = false;
        isShop = false;
        root.SetActive(false);
        wareHouseUI.gameObject.SetActive(false);
        chestUI.gameObject.SetActive(false);
        if (chest != null)
        {
            chest.ClientCloseRpc(GameManager.Instance.player);
            chest = null;
        }
    }

    public void InitBag()
    {
        SaveData.Instance.SortAll();
        foreach (var item in bagBoxes)
        {
            if (item.propUI != null)
            {
                Destroy(item.propUI.gameObject);
            }
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
            if (item.propUI != null)
            {
                Destroy(item.propUI.gameObject);
            }
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
    public void InitChest(Chest chest)
    {
        foreach (var item in chestBoxes)
        {
            if (item.propUI != null)
            {
                Destroy(item.propUI.gameObject);
            }
        }
        for (int i = 0; i < chest.propsList.Count; i++)
        {

            var propUI = Instantiate(Dic.Instance.propPrefab, chestBoxes[i].transform).GetComponent<PropUI>();
            propUI.box = chestBoxes[i];
            chestBoxes[i].propUI = propUI.gameObject;

            propUI.transform.position = propUI.box.transform.position;
            propUI.propImage.sprite = Dic.Instance.GetPropData(chest.propsList[i].propName).propSprite;
            propUI.amountText.text = chest.propsList[i].amount.ToString();
            Props p = new Props();
            p.propName = chest.propsList[i].propName;
            p.amount = chest.propsList[i].amount;
            propUI.props = p;

            propUI.isChest = true;
        }
    }

}
