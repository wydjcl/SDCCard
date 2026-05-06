using FishNet.Managing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 初始场景UI,调用管理器
/// </summary>
public class InitialSceneUI : MonoBehaviour
{
    public NetworkManager networkManager;
    [SerializeField]
    private InitialSceneManager manager;

    public GameObject dataImage;
    public TMP_InputField dataNameInput;

    [Header("联机大厅UI")]
    public GameObject lobby;
    public TMP_InputField inputField;

    public GameObject hostButttom;
    public GameObject clientButtom;
    public GameObject inputText;
    public GameObject disconnectButtom;
    public GameObject startGameButtom;

    [Header("玩家血条UI管理")]
    public float spacing = 100f;
    public List<RectTransform> bars = new List<RectTransform>();
    private void Awake()
    {
        if (networkManager == null)
        {
            networkManager = FindAnyObjectByType<NetworkManager>();
        }
    }
    private void Start()
    {
        inputField.text = "localhost";
    }

    public void StartLobby()
    {
        //manager.StartLobby();
        lobby.SetActive(true);
    }
    public void QuitLobby()
    {
        lobby.SetActive(false);
        manager.StartDisconnect();
    }
    public void StartDataImage()
    {
        dataImage.SetActive(true);
        dataNameInput.text = SaveData.Instance.data.playerName;
    }
    public void QuitDataImage()
    {
        dataImage.SetActive(false);
    }
    public void ChangePlayerNameData()
    {
        SaveData.Instance.data.playerName = dataNameInput.text;
        SaveManager.Save(SaveData.Instance.data);
    }
    public void QuitGame()
    {
        manager.QuitGame();
    }
    public void StartHost()
    {
        manager.StartHost();
    }
    public void StartClinet()
    {
        if (inputField.text == "")
        {
            Debug.Log("IP默认修改为:localhost");
            manager.StartClinet("localhost");
            return;
        }
        manager.StartClinet(inputField.text);
    }
    public void StartDisconnect()
    {
        manager.StartDisconnect();
    }

    public void StartGame()
    {
        manager.StartGame();
    }

    public void RegisterHealthBar(RectTransform bar)
    {
        bars.Add(bar);
        UpdateLayoutHealthBar();
    }

    public void UnregisterHealthBar(RectTransform bar)
    {
        bars.Remove(bar);
        UpdateLayoutHealthBar();
    }

    public void UpdateLayoutHealthBar()
    {
        for (int i = 0; i < bars.Count; i++)
        {
            if (!bars[i].gameObject.activeSelf)
            {
                continue;
            }
            bars[i].anchoredPosition = new Vector2(240, i * spacing + 100);
        }
    }


}
