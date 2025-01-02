using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : Singleton<PanelManager>
{
    //[Header("Component")]
    //[Header("Settings")]
    //[Header("Debug")]
    private const float OutScreenPositionX = 2500;
    
    
    public void ChangePanel(UIAnimation from, UIAnimation to)
    {
        StartCoroutine(ChangePanelCoroutine(from, to));
    }
    
    IEnumerator ChangePanelCoroutine(UIAnimation from, UIAnimation to)
    {
        from.PanelRightOutAnimation();
        yield return new WaitForSeconds(0.5f);
        from.gameObject.SetActive(false);
        to.gameObject.SetActive(true);
        to.PanelLeftInAnimation(to.startPos.x);
    }
}
