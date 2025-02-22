using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AreaUI : MonoBehaviour
{
    [Header("Component")]
    public bool openImageFill;
    [ShowIf("openImageFill")] public Image blueTeamOccupiedImage;
    [ShowIf("openImageFill")] public Image redTeamOccupiedImage;
    
    [Space(15)]
    public bool openTextUpdate;
    [ShowIf("openTextUpdate")] public TMP_Text selfTeamOccupiedText;
    [ShowIf("openTextUpdate")] public TMP_Text textShadow;
    
    //[Header("Settings")]
    //[Header("Debug")]

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        // Image Fill
        if(!openImageFill) return;
        blueTeamOccupiedImage.fillAmount = 0;
        redTeamOccupiedImage.fillAmount = 0;

        // Text
        if(!openTextUpdate) return;
        selfTeamOccupiedText.text = "";
        textShadow.text = "";

    }

    public void UpdateUI(AreaData area)
    {
        // Image Fill
        if(!openImageFill) return;
        DOTween.To(
            () => blueTeamOccupiedImage.fillAmount,
            x => blueTeamOccupiedImage.fillAmount = x,
            area.blueTeamOccupiedPercentage.Value / 100,
            0.3f
        );
        DOTween.To(
            () => redTeamOccupiedImage.fillAmount,
            x => redTeamOccupiedImage.fillAmount = x,
            area.redTeamOccupiedPercentage.Value / 100,
            0.3f
        );
        
        // Text 
        if(!openTextUpdate) return;
        selfTeamOccupiedText.text = TeamManager.Instance.GetSelfTeamCharacterController().team.Value == Team.Blue ? 
                area.blueTeamOccupiedPercentage.Value + "%" : area.redTeamOccupiedPercentage.Value + "%";
        textShadow.text = selfTeamOccupiedText.text;
    }
}
