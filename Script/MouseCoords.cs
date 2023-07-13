using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCoords : MonoBehaviour
{
    public RegionPanel regionPanel;
    Vector3 point;
    int x = 0;
    int y = 0;
    public MapXMLParser xmlParser;

    string currentName;
    string currentType;
    string currentEvilness;

    Material lastMat;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
        x = Mathf.CeilToInt(Mathf.Clamp(point.x -1, 0, xmlParser.maxX));
        y = Mathf.CeilToInt(Mathf.Clamp(point.y, 0, xmlParser.maxY));
        if (xmlParser.regionDataMap != null)
        {
            //Debug.Log(x + "," + y + " | " + xmlParser.regionDataMap[x, y].name);
            currentName = xmlParser.regionDataMap[x, y].name;
            currentType = xmlParser.regionDataMap[x, y].type;
            currentEvilness = xmlParser.regionDataMap[x, y].evilness;
            regionPanel.Display(currentName, currentType, currentEvilness);

            if (lastMat != null)
            {
                lastMat.SetFloat("_whiteness", 0f);
            }
            lastMat = xmlParser.regionDataMap[x, y].material;
            xmlParser.regionDataMap[x, y].material.SetFloat("_whiteness", 0.6f);
        }
    }
}
