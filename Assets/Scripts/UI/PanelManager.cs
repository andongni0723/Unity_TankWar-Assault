using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : Singleton<PanelManager>
{
    //[Header("Component")]
    [Header("Settings")]
    public GameObject StartPanel;
    public List<GameObject> AllPanels;
    //[Header("Debug")]
    private const float OutScreenPositionX = 2500;

    public override void Awake()
    {
        base.Awake();
        InitialPanel();
    }

    private void InitialPanel()
    {
        foreach (var panel in AllPanels)
            panel.SetActive(false);
        StartPanel.SetActive(true);
    }

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
