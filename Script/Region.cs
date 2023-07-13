using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region : MonoBehaviour
{
    public string name;
    public string type;

    public SitePanel uiPanel;

    private void OnMouseEnter()
    {
        Debug.Log("Mouse over");
        uiPanel.Display(name, type, "");
    }

    private void OnMouseExit()
    {
        uiPanel.Clear();
        Debug.Log("Mouse left");
    }
}
