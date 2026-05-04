using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PropUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("需导入UI")]
    public Image propImage;
    public Image backImage;
    public TextMeshProUGUI amountText;
    public GameObject dig;

    [Header("拖动数据")]
    public PropBox box;
    private Vector2 dragOffset;
    public PropBox target;
    [Header("道具数据")]
    public Props props;
    //public string propName;
    //public int stackAmount;
    public bool isWarehouse;//在战备的仓库
    public bool isBag;//在战备的背包
    public bool isChest;//在战斗的宝箱
    public bool isShop;//在商店
    public bool isVault;//在保险箱
    //public bool isBattleBag;

    public bool isEquip;//是装备
    public bool isUseful;//是消耗品

    public bool isEnter;
    private void Start()
    {
        PropRate r = Dic.Instance.GetPropData(props.propName).propRate;
        switch (r)
        {
            case PropRate.white:
                backImage.color = Color.white;
                break;
            case PropRate.green:
                backImage.color = Color.green;
                break;
            case PropRate.blue:
                backImage.color = new Color(0f, 1f, 1f);
                break;
            case PropRate.purple:
                backImage.color = new Color(0.5f, 0, 0.5f);
                break;
            case PropRate.gold:
                backImage.color = new Color(1, 0.84f, 0);
                break;
            case PropRate.red:
                backImage.color = new Color(0.70f, 0.05f, 0.05f);
                break;
        }

    }
    public void GetAll()//左键
    {
        if (isWarehouse)
        {
            if (BagUI.Instance.isBattle)
            {
                return;
            }
            SaveData.Instance.TakeFromList(SaveData.Instance.data.bag, SaveData.Instance.data.warehouse, props.propName, props.amount);
            //PreparationUI.Instance.Init();
            BagUI.Instance.InitBag();
            this.gameObject.SetActive(false);
            Destroy(this.gameObject);
            box.propUI = null;
        }
        if (isBag)
        {
            if (BagUI.Instance.isBattle)
            {
                return;
            }
            //TODO商店里按左键卖出
            SaveData.Instance.StoreToWarehouse(SaveData.Instance.data.bag, SaveData.Instance.data.warehouse, props.propName, props.amount);
            //PreparationUI.Instance.Init();
            BagUI.Instance.InitWareHouse();
            this.gameObject.SetActive(false);
            Destroy(this.gameObject);
            box.propUI = null;
        }
        if (isChest)
        {
            SaveData.Instance.TakeFromList(SaveData.Instance.data.bag, BagUI.Instance.chest.propsList, props.propName, props.amount);
            // BattleScenePlayerUI.Instance.chest.ClientSyncChestListRpc(GameManager.Instance.player, BattleScenePlayerUI.Instance.chest.propsList);
            // BattleScenePlayerUI.Instance.Init();

            BagUI.Instance.chest.ClientSyncChestListRpc(GameManager.Instance.player, BagUI.Instance.chest.propsList);
            BagUI.Instance.InitBag();
            BagUI.Instance.InitChest(BagUI.Instance.chest);
            //Destroy(this.gameObject);
            //box.propUI = null;
        }
    }
    public void GetOne()//右键
    {
        if (isWarehouse)
        {
            if (BagUI.Instance.isBattle)
            {
                return;
            }
            SaveData.Instance.TakeFromList(SaveData.Instance.data.bag, SaveData.Instance.data.warehouse, props.propName, 1);
            BagUI.Instance.InitWareHouse();
            BagUI.Instance.InitBag();
        }
        if (isBag)
        {
            if (BagUI.Instance.isBattle)
            {
                return;
            }
            SaveData.Instance.StoreToWarehouse(SaveData.Instance.data.bag, SaveData.Instance.data.warehouse, props.propName, 1);
            BagUI.Instance.InitWareHouse();
            BagUI.Instance.InitBag();
        }
        if (isChest)
        {
            SaveData.Instance.TakeFromList(SaveData.Instance.data.bag, BagUI.Instance.chest.propsList, props.propName, 1);
            //BattleScenePlayerUI.Instance.chest.ClientSyncChestListRpc(GameManager.Instance.player, BattleScenePlayerUI.Instance.chest.propsList);
            //BattleScenePlayerUI.Instance.Init();

            BagUI.Instance.chest.ClientSyncChestListRpc(GameManager.Instance.player, BagUI.Instance.chest.propsList);
            BagUI.Instance.InitBag();
            BagUI.Instance.InitChest(BagUI.Instance.chest);
            //Destroy(this.gameObject);
            //box.propUI = null;
        }
    }

    public void E()//消耗品或装备E键盘使用
    {
        if (GameManager.Instance.player.currentRoom.Value.isBattle.Value)
        {
            Debug.Log("战斗中无法使用");
            return;
        }
        if (BagUI.Instance.isBattle)
        {
            if (Dic.Instance.GetPropType(props.propName) == PropType.Consumable)
            {
                Debug.Log("使用消耗品");
                foreach (var e in Dic.Instance.GetPropData(props.propName).effects)
                {
                    e.ApplyEffect(GameManager.Instance.player);
                }
                SaveData.Instance.UseProp(props.propName);
                BagUI.Instance.InitBag();
            }
            if (Dic.Instance.GetPropType(props.propName) == PropType.Equipment)
            {
                Debug.Log("装备");
            }
        }
    }
    #region 物理
    public void OnBeginDrag(PointerEventData eventData)
    {
        //// 开始拖拽时，计算鼠标和图片的偏移
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //    transform.parent as RectTransform,
        //    Input.mousePosition,
        //    eventData.pressEventCamera,
        //    out Vector2 localPoint);

        //dragOffset = (Vector2)transform.localPosition - localPoint;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //// 核心：让UI跟随鼠标移动
        //RectTransform rect = GetComponent<RectTransform>();

        //RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //    rect.parent as RectTransform,
        //    Input.mousePosition,
        //    eventData.pressEventCamera,
        //    out Vector2 localPoint);

        //// 应用偏移，让拖拽更自然
        //rect.localPosition = localPoint + dragOffset;



        //target = null;

        //// 1. 创建一个射线事件（从鼠标位置发射，专门找UI）
        //PointerEventData clickData = new PointerEventData(EventSystem.current);
        //clickData.position = Input.mousePosition;

        //// 2. 存储射线碰到的所有UI
        //var results = new System.Collections.Generic.List<RaycastResult>();
        //EventSystem.current.RaycastAll(clickData, results);

        //// 3. 遍历找到第一个标签为 PropBox 的格子
        //foreach (var result in results)
        //{
        //    if (result.gameObject.CompareTag("PropBox"))
        //    {
        //        target = result.gameObject.GetComponent<PropBox>();
        //        break; // 找到就停
        //    }
        //}

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //if (target)
        //{
        //    box = target;
        //}
        //transform.position = box.transform.position;
        ////transform.SetParent(box.transform, false);
        //target = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }
    #endregion
}
