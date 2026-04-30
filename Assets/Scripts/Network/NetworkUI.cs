using FishNet;
using FishNet.Managing;
using FishNet.Managing.Scened;
using FishNet.Object;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkUI : MonoBehaviour
{
    public NetworkManager networkManager;
    private string ipAddress = "localhost";
    private void Awake()
    {
        if (networkManager == null)
        {
            networkManager = FindAnyObjectByType<NetworkManager>();
        }
    }
    private void OnGUI()
    {
        if (networkManager == null)
            return;

        GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        buttonStyle.fontSize = 24;
        buttonStyle.fixedHeight = 60;
        buttonStyle.fixedWidth = 200;

        GUIStyle textFieldStyle = new GUIStyle(GUI.skin.textField);
        textFieldStyle.fontSize = 20;
        textFieldStyle.fixedHeight = 40;

        GUILayout.BeginArea(new Rect(100, 10, 250, 300));

        GUILayout.FlexibleSpace();

        if (!networkManager.ClientManager.Started && !networkManager.ServerManager.Started)
        {



            // ✅ 主机
            if (GUILayout.Button("主机端", buttonStyle))
            {
                networkManager.ServerManager.StartConnection();
                networkManager.ClientManager.StartConnection();
            }

            // ✅ 客户端（使用输入的IP）
            if (GUILayout.Button("客户端", buttonStyle))
            {
                var transport = networkManager.TransportManager.Transport;

                transport.SetClientAddress(ipAddress); // 👈 关键！

                networkManager.ClientManager.StartConnection();
            }
            // ✅ IP输入框
            GUILayout.Label("客户端输入IP地址:");
            ipAddress = GUILayout.TextField(ipAddress, textFieldStyle);
        }
        else
        {
            if (GUILayout.Button("断开连接", buttonStyle))
            {
                networkManager.ServerManager.StopConnection(true);
                networkManager.ClientManager.StopConnection();
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
            }
        }

        GUILayout.FlexibleSpace();

        GUILayout.EndArea();
    }


}
