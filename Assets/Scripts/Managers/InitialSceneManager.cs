using FishNet;
using FishNet.Connection;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using FishNet.Transporting;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 初始场景管理器
/// </summary>
public class InitialSceneManager : MonoBehaviour
{
    public NetworkManager networkManager;
    public InitialSceneUI ui;
    //private string ipAddress = "localhost";
    #region 生命周期和网络回调
    private void Awake()
    {
        if (networkManager == null)
        {
            networkManager = FindAnyObjectByType<NetworkManager>();
        }
    }
    public void Start()
    {
        InstanceFinder.ServerManager.OnServerConnectionState += OnServerConnectionState;
        InstanceFinder.ServerManager.OnRemoteConnectionState += OnRemoteConnectionState;
        InstanceFinder.ClientManager.OnClientConnectionState += OnClientConnectionState;
        SaveManager.LoadOrCreate(0);
    }

    public void OnDisable()
    {
        //InstanceFinder.ServerManager.OnServerConnectionState -= OnServerConnectionState;
        //InstanceFinder.ServerManager.OnRemoteConnectionState -= OnRemoteConnectionState;
        //InstanceFinder.ClientManager.OnClientConnectionState -= OnClientConnectionState;
    }
    private void OnServerConnectionState(ServerConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            OnServerStarted();
        }
    }
    /// <summary>
    /// 服务端初始化逻辑
    /// </summary>
    void OnServerStarted()
    {
        Debug.Log("服务端初始化逻辑！");
    }

    /// <summary>
    /// 客户端执行,连接成功逻辑
    /// </summary>
    /// <param name="args"></param>
    private void OnClientConnectionState(ClientConnectionStateArgs args)
    {
        if (args.ConnectionState == LocalConnectionState.Started)
        {
            //Debug.Log("客户端连接成功！");
            MyClientConnected();
        }
        else if (args.ConnectionState == LocalConnectionState.Stopped)
        {
            //Debug.Log("客户端断开连接！");
            MyClientDisconnected();
        }
    }
    /// <summary>
    /// 客户端连接成功本地逻辑
    /// </summary>
    void MyClientConnected()
    {
        // 客户端连接成功后的逻辑
        //Debug.Log("客户端连接成功后的逻辑！");
        ui.hostButttom.SetActive(false);
        ui.clientButtom.SetActive(false);
        ui.inputText.SetActive(false);
        ui.disconnectButtom.SetActive(true);
        //ui.chooseBox.SetActive(true);
        if (InstanceFinder.IsServerStarted)
        {
            ui.startGameButtom.SetActive(true);
        }
    }
    /// <summary>
    /// 客户端断开连接 本地逻辑
    /// </summary>
    void MyClientDisconnected()
    {
        // 客户端连接成功后的逻辑
        Debug.Log("客户端断开成功后的逻辑！");
        ui.hostButttom.SetActive(true);
        ui.clientButtom.SetActive(true);
        ui.inputText.SetActive(true);
        ui.disconnectButtom.SetActive(false);
        ui.startGameButtom.SetActive(false);

        UnloadAllExcept("InitialScene");
        //ui.chooseBox.SetActive(false);
    }
    /// <summary>
    /// 服务端监测客户端连接
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="args"></param>
    private void OnRemoteConnectionState(NetworkConnection conn, RemoteConnectionStateArgs args)
    {
        if (args.ConnectionState == RemoteConnectionState.Started)
        {
            Debug.Log($"客户端连入服务器：{conn.ClientId}");
            //OnClientConnected(conn);
        }
        else if (args.ConnectionState == RemoteConnectionState.Stopped)
        {
            Debug.Log($"客户端断开连接：{conn.ClientId}");
            //OnClientDisconnected(conn);
        }
    }
    #endregion

    #region UI的方法
    public void StartLobby()
    {
        Debug.Log("开始联机大厅");
    }
    public void QuitGame()
    {
        Debug.Log("退出游戏");
        Application.Quit();
    }
    public void StartHost()
    {
        networkManager.ServerManager.StartConnection();
        networkManager.ClientManager.StartConnection();
    }
    public void StartClinet(string ipAddress)
    {
        var transport = networkManager.TransportManager.Transport;

        transport.SetClientAddress(ipAddress); // 👈 关键！

        networkManager.ClientManager.StartConnection();
    }
    public void StartDisconnect()
    {
        networkManager.ServerManager.StopConnection(true);
        networkManager.ClientManager.StopConnection();
    }

    /// <summary>
    /// 服务端调用,开始游戏
    /// </summary>
    public void StartGame()
    {
        // 配置加载数据
        SceneLoadData loadData = new SceneLoadData("MainScene");
        //SceneLoadData loadData = new SceneLoadData("BattleScene");

        // 可选：叠加场景（非单一替换）
        // loadData.Options.Merge = true;

        // 服务器全局加载 → 所有客户端自动同步
        InstanceFinder.SceneManager.LoadGlobalScenes(loadData);
        GameManager.Instance.player.DisableInitUI();

    }
    public void UnloadAllExcept(string keepScene)
    {
        ui.gameObject.SetActive(true);//特殊化处理,开启初始场景的时候把UI打开
        int count = UnityEngine.SceneManagement.SceneManager.sceneCount;

        for (int i = 0; i < count; i++)
        {
            Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

            if (scene.name != keepScene)
            {
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
            }
        }
    }
    #endregion

    [ContextMenu("测试加载场景")]
    public void Test1()
    {
        // 配置加载数据
        SceneLoadData loadData = new SceneLoadData("TestScene");
        //SceneLoadData loadData = new SceneLoadData("BattleScene");

        // 可选：叠加场景（非单一替换）
        // loadData.Options.Merge = true;

        // 服务器全局加载 → 所有客户端自动同步
        InstanceFinder.SceneManager.LoadGlobalScenes(loadData);

    }
    [ContextMenu("测试卸载场景")]
    public void Test2()
    {
        GameManager.Instance.DespawnAllObjectByScene("TestScene");
        SceneUnloadData unloadData = new SceneUnloadData("TestScene");

        InstanceFinder.SceneManager.UnloadGlobalScenes(unloadData);//TODO回调

    }
}
