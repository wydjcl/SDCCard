using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BagUI : UITemplate<BagUI>
{
    public GameObject root;
    public GameObject UIContainer;//生成的UI的容器

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
        LoadData();
        for (int i = 0; i < bagBoxes.Count; i++)
        {
            bagBoxes[i].index = i;
            bagBoxes[i].props.index = i;
            bagBoxes[i].propBoxType = PropBoxType.bag;
            bagBoxes[i].currentPropsList = SaveData.Instance.data.bag;
            bagBoxes[i].isBag = true;
            bagBoxes[i].bagUI = this;
        }
        for (int i = 0; i < wareHouseBoxes.Count; i++)
        {
            wareHouseBoxes[i].index = i;
            wareHouseBoxes[i].props.index = i;
            wareHouseBoxes[i].propBoxType = PropBoxType.warehouse;
            wareHouseBoxes[i].currentPropsList = SaveData.Instance.data.warehouse;
            wareHouseBoxes[i].isWarehouse = true;
            wareHouseBoxes[i].bagUI = this;
        }
        for (int i = 0; i < chestBoxes.Count; i++)
        {
            chestBoxes[i].index = i;
            chestBoxes[i].props.index = i;
            chestBoxes[i].propBoxType = PropBoxType.chest;
            chestBoxes[i].isChest = true;
            chestBoxes[i].bagUI = this;
        }
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
        DiscardContainer();
        foreach (var b in bagBoxes)
        {
            b.RefreshUI();
        }
    }
    public void InitWareHouse()
    {
        foreach (var b in wareHouseBoxes)
        {
            b.RefreshUI();
        }
    }
    public void InitChest(Chest chest)
    {
        for (int i = 0; i < chest.propsList.Count; i++)
        {
            chestBoxes[i].props.propName = chest.propsList[i].propName;
            chestBoxes[i].props.amount = chest.propsList[i].amount;
            if (chest.propsList[i].amount == 0)
            {
                chestBoxes[i].props.propName = "";
            }
        }
        foreach (var b in chestBoxes)
        {
            b.RefreshUI();
        }
    }

    public PropBox FindPropBoxByIndex(List<PropBox> boxes, int index)
    {
        foreach (PropBox box in boxes)
        {
            if (box.index == index)
                return box;
        }
        Debug.LogWarning($"没找到对应下标的盒子!!!Index:{index}");
        return null;
    }

    public void LoadData()
    {
        foreach (var d in SaveData.Instance.data.bag)
        {
            var i = d.index;
            bagBoxes[i].props.propName = d.propName;
            bagBoxes[i].props.amount = d.amount;
        }
        foreach (var d in SaveData.Instance.data.warehouse)
        {
            var i = d.index;
            wareHouseBoxes[i].props.propName = d.propName;
            wareHouseBoxes[i].props.amount = d.amount;
        }
    }

    public int PutInBox(List<PropBox> boxes, PropUI propUI)
    {
        string propName = propUI.props.propName;
        int amount = propUI.props.amount;
        foreach (var box in boxes)
        {
            if (string.IsNullOrEmpty(box.props.propName))
            {
                box.props.propName = propName;
                box.props.amount = amount;
                amount = 0;
            }
            else if (box.props.propName == propName)
            {
                int empty = Dic.Instance.GetPropData(propName).maxStack - box.props.amount;
                if (empty >= amount)
                {
                    box.props.amount += amount;
                    amount = 0;
                }
                else
                {
                    amount -= empty;
                    box.props.amount = Dic.Instance.GetPropData(propName).maxStack;
                }

            }
            box.RefreshUI();
            if (amount == 0)
            {
                return amount;
            }
        }

        return amount;
    }
    public int DisassembleProp(List<PropBox> boxes, PropUI propUI, int i)//拆解
    {
        string propName = propUI.props.propName;
        int amount = i;
        if (i == 0)
        {
            return amount;
        }
        foreach (var box in boxes)
        {
            if (string.IsNullOrEmpty(box.props.propName))
            {
                box.props.propName = propName;
                box.props.amount = amount;
                amount = 0;
                box.RefreshUI();
                break;
            }

        }

        return amount;
    }
    public int PutOneInBox(List<PropBox> boxes, PropUI propUI)
    {
        string propName = propUI.props.propName;
        int amount = 1;
        foreach (var box in boxes)
        {
            if (string.IsNullOrEmpty(box.props.propName))
            {
                box.props.propName = propName;
                box.props.amount = amount;
                amount = 0;
            }
            else if (box.props.propName == propName)
            {
                int empty = Dic.Instance.GetPropData(propName).maxStack - box.props.amount;
                if (empty >= amount)
                {
                    box.props.amount += amount;
                    amount = 0;
                }
                else
                {
                    amount -= empty;
                    box.props.amount = Dic.Instance.GetPropData(propName).maxStack;
                }

            }
            box.RefreshUI();
            if (amount == 0)
            {
                return 1 - amount;
            }
        }

        return 1 - amount;
    }

    public void DiscardContainer()
    {

        for (int i = UIContainer.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(UIContainer.transform.GetChild(i).gameObject);
        }
    }
}
