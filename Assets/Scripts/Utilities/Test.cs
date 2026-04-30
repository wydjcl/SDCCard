using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : NetworkBehaviour
{
    [ContextMenu("1")]
    public void TestWay()
    {
        Despawn();

    }
    [ContextMenu("输出场景")]
    public void TestWay2()
    {
        Debug.Log(gameObject.scene.name);
    }
    public void TestWay3()
    {
        if (IsSpawned)
        {

        }
    }
}
