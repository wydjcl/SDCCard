using FishNet.Managing;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Player player;
    private InitialSceneUI ui;

    public Image avatar;
    public Image healthImage;
    public Image healthBarImage;
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI hpText;
    // Start is called before the first frame update
    private void Awake()
    {
        if (ui == null)
        {
            ui = FindAnyObjectByType<InitialSceneUI>();
        }
    }
    void Start()
    {
        GameObject parentObj = GameObject.Find("PlayerHPBarUI");

        if (parentObj == null)
        {
            Debug.LogError("场景中没有找到 PlayerHPBarUI");
            return;
        }

        Transform parent = parentObj.transform;

        // 设置父子关系
        this.transform.SetParent(parent, false);
        if (ui != null)
        {
            ui.RegisterHealthBar(this.GetComponent<RectTransform>());
        }
        playerNameText.text = player.playerName.Value;
    }
    private void OnDestroy()
    {
        ui.UnregisterHealthBar(this.GetComponent<RectTransform>());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hpText.text = $"{player.HP.Value}/{player.maxHP.Value}";
        hpText.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hpText.gameObject.SetActive(false);
    }
}
