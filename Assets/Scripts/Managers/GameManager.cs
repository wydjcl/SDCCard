using DG.Tweening;
using FishNet;
using FishNet.Object;
using FishNet.Transporting;
using LiteNetLib.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
/// <summary>
/// 一些需要调用动态数据或方法的单例
/// </summary>
public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public Player player;
    public List<Player> players;
    [Header("相机")]
    public Camera cam;
    public CameraMode camMode;
    public bool camCanDrag;
    public Vector3 orCamPos;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        DOTween.SetTweensCapacity(800, 300);
    }
    public void ChangeCameraMode(CameraMode newMode)
    {
        camMode = newMode;
        if (newMode == CameraMode.Main)
        {
            camCanDrag = false;
            orCamPos = cam.transform.position;
            cam.transform.position = new Vector3(0, 0, -10);
        }
        if (newMode == CameraMode.Map)
        {
            camCanDrag = true;
            cam.transform.position = orCamPos;
        }
    }
    [ContextMenu("正常相机")]
    public void Test1()
    {
        ChangeCameraMode(CameraMode.Main);
    }
    [ContextMenu("地图相机")]
    public void Test2()
    {
        ChangeCameraMode(CameraMode.Map);
    }
    public void DespawnAllObjectByScene(string sceneName)
    {
        var all = InstanceFinder.ServerManager.Objects.Spawned.Values;

        // ✔ 先拷贝一份（避免边遍历边修改）
        List<NetworkObject> list = new List<NetworkObject>(all);

        foreach (var nob in list)
        {
            if (!nob) continue;

            if (nob.gameObject.scene.name == sceneName)
            {
                InstanceFinder.ServerManager.Despawn(nob);
            }
        }
    }
    private void OnEnable()
    {
        //InstanceFinder.NetworkManager.ClientManager.OnClientConnectionState += OnClientState;
    }

    private void OnDisable()
    {
        //InstanceFinder.NetworkManager.ClientManager.OnClientConnectionState -= OnClientState;
    }
    private void OnClientState(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Stopped)
        {
            Debug.Log("客户端已断开（可能是主机退出）,尝试重加载");

            // 👉 这里写你的逻辑
            // 比如：
            // 返回主菜单
            // 弹窗提示
            UnityEngine.SceneManagement.SceneManager.LoadScene("InitialScene");
        }
    }
}
