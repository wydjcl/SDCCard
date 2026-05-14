using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PropBox : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // public GameObject propUI;
    [HideInInspector]
    public BagUI bagUI;
    public PropUI propUI;
    public bool isEnter;
    public int index;
    public PropBoxType propBoxType;
    public List<Props> currentPropsList;
    public Props props;//格子维护的props

    public bool isWarehouse;//在战备的仓库
    public bool isBag;//在战备的背包
    public bool isChest;//在战斗的宝箱
    public bool isShop;//在商店
    public bool isVault;//在保险箱
    private void Update()
    {
        //if (isEnter)
        //{

        //    if (Input.GetMouseButtonDown(0))
        //    {
        //       propUI.GetComponent<PropUI>().GetAll();
        //    }
        //    else if (Input.GetMouseButtonDown(1))
        //    {
        //        propUI.GetComponent<PropUI>().GetOne();
        //    }

        //}
        //if (isEnter)
        //{
        //    if (Input.GetKeyDown(KeyCode.E))
        //    {
        //        propUI.GetComponent<PropUI>().E();
        //    }
        //}
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isEnter = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isEnter = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isEnter)
        {
            return;
        }
        if (!propUI)
        {
            return;
        }
    }

    public void RefreshUI()
    {
        if (propUI != null)
        {
            Destroy(propUI.gameObject);
        }
        propUI = null;

        if (string.IsNullOrEmpty(props.propName))
        {
            return;
        }
        var pr = Instantiate(Dic.Instance.propPrefab, transform).GetComponent<PropUI>();
        propUI = pr;
        propUI.transform.SetParent(bagUI.UIContainer.transform);
        propUI.box = this;
        propUI.transform.position = propUI.box.transform.position;
        propUI.propImage.sprite = Dic.Instance.GetPropData(props.propName).propSprite;

        propUI.amountText.text = props.amount.ToString();
        propUI.props = props;

    }
}

