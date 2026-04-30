using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITemplate<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this as T;
        //DontDestroyOnLoad(gameObject);
    }
    public virtual void CloseUI()
    {
        gameObject.SetActive(false);
        PropDigManager.Instance.DestroyDig();
    }
}
