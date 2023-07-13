using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SitePanel : MonoBehaviour
{
    public Canvas canvas;
    public RectTransform rectT;
    public RectTransform canvasRectT;

    public Text nameT;
    public Text typeT;
    public Text coordsT;

    public GameObject panel;

    public void Display(string name, string type, string coords)
    {
        nameT.text = name;
        typeT.text = type;
        coordsT.text = coords;
        panel.SetActive(true);
    }

    private void Update()
    {
        // Get the screen position of the mouse
        Vector2 mousePosition = Input.mousePosition;

        // Set the UI element's position to the mouse position
        rectT.position = mousePosition;
    }

    public void Clear()
    {
        panel.SetActive(false);
        nameT.text = "";
        typeT.text = "";
        coordsT.text = "";
    }
}
