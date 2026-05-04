using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreparationUI : UITemplate<PreparationUI>
{
    public bool localhost;

    public GameObject characterUI;
    protected override void Awake()
    {
        base.Awake();
    }

    public void ClickBagUI()
    {
        characterUI.SetActive(false);
    }
    public void ClickCharacterButtom()
    {
        characterUI.SetActive(true);
    }
    public void ChangeCharacterID0()
    {
        GameManager.Instance.player.ChangeCharacterID(0);
    }
    public void ChangeCharacterID1()
    {
        GameManager.Instance.player.ChangeCharacterID(1);
    }

}
