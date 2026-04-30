using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PropBox : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameObject propUI;
    public bool isEnter;
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
        if (isEnter)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                propUI.GetComponent<PropUI>().E();
            }
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
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("左键点击");
            propUI.GetComponent<PropUI>().GetAll();
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("右键点击");
            propUI.GetComponent<PropUI>().GetOne();
        }
    }
}

