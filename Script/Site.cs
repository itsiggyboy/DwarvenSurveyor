using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Site : MonoBehaviour
{
    public string name;
    public string type;
    public int typeIndex;
    public Vector2Int coord;
    //public Rect rectangle;
    public SpriteRenderer spriteR;

    public SitePanel uiPanel;

    private void OnMouseEnter()
    {
        Debug.Log("Mouse over");
        uiPanel.Display(name, type, "(" + coord.x + "," + coord.y + ")");
    }

    private void OnMouseExit()
    {
        uiPanel.Clear();
        Debug.Log("Mouse left");    
    }
}
