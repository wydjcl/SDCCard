using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropDigManager : SingletonMono<PropDigManager>
{
    public PropUI currentPropUI;
    public GameObject digRoot;
    public GameObject currentPropDig;

    [Header("dig预制体")]
    public GameObject PropTakeDig;
    public void DestroyDig()
    {
        if (currentPropDig)
        {
            Destroy(currentPropDig);
            currentPropDig = null;
        }
    }
    public void CreatePropTakeDig(PropUI caster, RectTransform rect)//TODO把生成框集成一下
    {
        if (caster != currentPropUI)
        {
            Debug.Log("跟上次发起者不一样");
            DestroyDig();
            currentPropDig = Instantiate(PropTakeDig, digRoot.transform);
            RectTransform r = currentPropDig.GetComponent<RectTransform>();

            // 👉 获取目标UI的世界坐标
            Vector3 worldPos = rect.position;

            // 👉 转成当前Canvas坐标
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, worldPos);
            RectTransform parentRect = digRoot.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPos, null, out Vector2 localPos);

            // 👉 偏移
            r.anchoredPosition = localPos + new Vector2(100, 100);

        }
        else
        {
            if (currentPropDig)
            {
                DestroyDig();
                Debug.Log("跟上次发起者一样,但已经有框");
            }
            else
            {
                Debug.Log("跟上次发起者一样,但无框");
                currentPropDig = Instantiate(PropTakeDig, digRoot.transform);
                RectTransform r = currentPropDig.GetComponent<RectTransform>();

                // 👉 获取目标UI的世界坐标
                Vector3 worldPos = rect.position;

                // 👉 转成当前Canvas坐标
                Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, worldPos);
                RectTransform parentRect = digRoot.GetComponent<RectTransform>();
                RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, screenPos, null, out Vector2 localPos);

                // 👉 偏移
                r.anchoredPosition = localPos + new Vector2(100, 100);
            }
        }
        currentPropUI = caster;


    }

}
