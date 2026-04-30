using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MapPosText : MonoBehaviour
{
    public TextMeshProUGUI tText;
    public void Start()
    {
        GameManager.Instance.player.mapPosText = tText;
    }
}
