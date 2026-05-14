
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class PropUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("需导入UI")]
    public Image propImage;
    public Image backImage;
    public TextMeshProUGUI amountText;
    public GameObject Entry;
    public GameObject dig;

    [Header("拖动数据")]
    public PropBox box;
    private Vector2 dragOffset;
    public PropBox target;
    [Header("道具数据")]
    public Props props;
    //public string propName;
    //public int stackAmount;
    //public bool isWarehouse;//在战备的仓库
    //public bool isBag;//在战备的背包
    //public bool isChest;//在战斗的宝箱
    //public bool isShop;//在商店
    //public bool isVault;//在保险箱
    //public bool isBattleBag;

    public bool isEquip;//是装备
    public bool isUseful;//是消耗品

    public bool isEnter;
    public bool isDrag;
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
    private void Update()
    {
        if (isEnter)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                E();
            }
        }
    }
    public void ClickLeft()
    {
        if (box.bagUI.isWareHouse)
        {
            if (box.propBoxType == PropBoxType.bag)
            {
                if (BagUI.Instance.isBattle)
                {
                    return;
                }
                box.props.amount = box.bagUI.PutInBox(box.bagUI.wareHouseBoxes, this);
                if (box.props.amount == 0)//全被放入
                {
                    box.props.propName = "";
                }
            }
            if (box.propBoxType == PropBoxType.warehouse)
            {
                if (BagUI.Instance.isBattle)
                {
                    return;
                }
                box.props.amount = box.bagUI.PutInBox(box.bagUI.bagBoxes, this);
                if (box.props.amount == 0)//全被放入
                {
                    box.props.propName = "";
                }
            }
        }
        if (box.bagUI.isChest)
        {
            Debug.Log("现在在宝箱UI");
            if (box.propBoxType == PropBoxType.chest)
            {
                Debug.Log("点击宝箱UI");

                int oramount = box.props.amount;
                box.props.amount = box.bagUI.PutInBox(box.bagUI.bagBoxes, this);
                box.bagUI.chest.TakeProp(GameManager.Instance.player, box.props.propName, oramount - box.props.amount);
                if (box.props.amount == 0)//全被放入
                {
                    box.props.propName = "";
                }
            }
        }
        box.RefreshUI();
    }

    public void ClickRight()
    {
        if (box.bagUI.isWareHouse)
        {
            if (box.propBoxType == PropBoxType.bag)
            {
                if (BagUI.Instance.isBattle)
                {
                    return;
                }
                box.props.amount -= (1 - box.bagUI.PutOneInBox(box.bagUI.wareHouseBoxes, this));
                if (box.props.amount == 0)//全被放入
                {
                    box.props.propName = "";
                }
            }
            if (box.propBoxType == PropBoxType.warehouse)
            {
                if (BagUI.Instance.isBattle)
                {
                    return;
                }
                box.props.amount -= (1 - box.bagUI.PutOneInBox(box.bagUI.bagBoxes, this));
                if (box.props.amount == 0)//全被放入
                {
                    box.props.propName = "";
                }
            }
        }
        if (box.bagUI.isChest)
        {
            if (box.propBoxType == PropBoxType.chest)
            {
                int oramount = box.props.amount;
                box.props.amount -= (1 - box.bagUI.PutOneInBox(box.bagUI.bagBoxes, this));
                box.bagUI.chest.TakeProp(GameManager.Instance.player, box.props.propName, oramount - box.props.amount);
                if (box.props.amount == 0)//全被放入
                {
                    box.props.propName = "";
                }
            }
        }
        box.RefreshUI();
    }
    public void ClickMid()
    {
        //if (box.bagUI.isWareHouse)
        //{
        if (box.propBoxType == PropBoxType.bag)
        {
            if (GameManager.Instance.player.currentRoom.Value.isBattle.Value)
            {
                return;
            }
            int am = box.props.amount / 2;
            box.props.amount = box.props.amount - am + box.bagUI.DisassembleProp(box.bagUI.bagBoxes, this, am);
            if (box.props.amount == 0)//全被放入
            {
                box.props.propName = "";
            }
        }
        if (box.propBoxType == PropBoxType.warehouse)
        {
            if (BagUI.Instance.isBattle)
            {
                return;
            }
            int am = box.props.amount / 2;
            box.props.amount = box.props.amount - am + box.bagUI.DisassembleProp(box.bagUI.wareHouseBoxes, this, am);

            if (box.props.amount == 0)//全被放入
            {
                box.props.propName = "";
            }
        }
        // }

        box.RefreshUI();
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
                box.props.amount -= 1;
                if (box.props.amount == 0)
                {
                    box.props.propName = "";
                }
            }
            else if (Dic.Instance.GetPropType(props.propName) == PropType.Equipment)
            {
                Debug.Log("装备");
            }
        }
        box.RefreshUI();
    }
    #region 物理
    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;
        //// 开始拖拽时，计算鼠标和图片的偏移
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            transform.parent as RectTransform,
            Input.mousePosition,
            eventData.pressEventCamera,
            out Vector2 localPoint);

        dragOffset = (Vector2)transform.localPosition - localPoint;
        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("拖动中");
        //// 核心：让UI跟随鼠标移动
        RectTransform rect = Entry.GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect.parent as RectTransform,
            Input.mousePosition,
            eventData.pressEventCamera,
            out Vector2 localPoint);

        // 应用偏移，让拖拽更自然
        rect.localPosition = localPoint + dragOffset;



        target = null;

        // 1. 创建一个射线事件（从鼠标位置发射，专门找UI）
        PointerEventData clickData = new PointerEventData(EventSystem.current);
        clickData.position = Input.mousePosition;

        // 2. 存储射线碰到的所有UI
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(clickData, results);

        // 3. 遍历找到第一个标签为 PropBox 的格子
        foreach (var result in results)
        {
            if (result.gameObject.CompareTag("PropBox"))
            {
                target = result.gameObject.GetComponent<PropBox>();
                break; // 找到就停
            }
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        if (target)
        {
            // if(target.propBoxType!=PropBoxType)  //TODO做判断,装备栏什么的
            if (Dic.Instance.GetPropType(box.props.propName) != PropType.Equipment)
            {

            }
            if (target.propBoxType == PropBoxType.bag || target.propBoxType == PropBoxType.warehouse)
            {
                if (target.props.propName == box.props.propName)
                {
                    int targetCanGetNum = Dic.Instance.GetPropData(target.props.propName).maxStack - target.props.amount;
                    if (targetCanGetNum > 0)
                    {
                        if (box.props.amount > targetCanGetNum)
                        {
                            box.props.amount -= targetCanGetNum;
                            target.props.amount += targetCanGetNum;
                            if (box.propBoxType == PropBoxType.chest)
                            {
                                box.bagUI.chest.TakeProp(GameManager.Instance.player, box.props.propName, targetCanGetNum);
                            }
                        }
                        else
                        {
                            if (box.propBoxType == PropBoxType.chest)
                            {
                                box.bagUI.chest.TakeProp(GameManager.Instance.player, box.props.propName, box.props.amount);
                            }
                            target.props.amount += box.props.amount;
                            box.props.propName = "";
                            box.props.amount = 0;
                        }
                    }
                    else
                    {
                        transform.position = box.transform.position;
                        target = null;
                        box.RefreshUI();
                        return;
                        //return;
                    }
                }
                else
                {
                    if (box.propBoxType == PropBoxType.chest && !(string.IsNullOrEmpty(target.props.propName)))
                    {
                        //如果从宝箱移到背包且对应格子有东西,则什么都不做
                    }
                    else///移动到空格子
                    {
                        if (box.propBoxType == PropBoxType.chest)
                        {
                            box.bagUI.chest.TakeProp(GameManager.Instance.player, box.props.propName, box.props.amount);
                        }
                        Props tprops = new Props();
                        tprops.propName = box.props.propName;
                        tprops.amount = box.props.amount;
                        //tprops.index = box.props.index;
                        box.props.propName = target.props.propName;
                        box.props.amount = target.props.amount;
                        target.props.propName = tprops.propName;
                        target.props.amount = tprops.amount;
                    }
                }

                box.RefreshUI();
                target.RefreshUI();
            }


            //box = target;
        }
        else
        {
            transform.position = box.transform.position;
            target = null;
            box.RefreshUI();
        }
        box.RefreshUI();
        //transform.position = box.transform.position;
        //props.index = box.index;
        ////transform.SetParent(box.transform, false);
        //target = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isDrag)
        {
            box.RefreshUI();
            Debug.Log("拖动中不能点击");
            return;
        }
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("左键点击");
            // GetAll();
            ClickLeft();
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("右键点击");
            ClickRight();
        }
        if (eventData.button == PointerEventData.InputButton.Middle)
        {
            Debug.Log("中键点击！");
            ClickMid();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isEnter = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isEnter = false;
    }
    #endregion
}
