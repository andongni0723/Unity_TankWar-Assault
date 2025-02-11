using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AreaUI : MonoBehaviour
{
    [Header("Component")]
    public Image blueTeamOccupiedImage;
    public Image redTeamOccupiedImage;
    public TMP_Text selfTeamOccupiedText;
    public TMP_Text textShadow;
    
    //[Header("Settings")]
    //[Header("Debug")]

    public void UpdateUI(AreaData area)
    {
        blueTeamOccupiedImage.fillAmount = area.blueTeamOccupiedPercentage.Value / 100;
        redTeamOccupiedImage.fillAmount = area.redTeamOccupiedPercentage.Value / 100;
        selfTeamOccupiedText.text = TeamManager.Instance.GetSelfTeamCharacterController().team.Value == Team.Blue ? 
                area.blueTeamOccupiedPercentage.Value + "%" : area.redTeamOccupiedPercentage.Value + "%";
        textShadow.text = selfTeamOccupiedText.text;
    }
}
