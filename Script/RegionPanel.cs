using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class RegionPanel : MonoBehaviour
{
    public Text regionType;
    public Text regionName;
    public Text regionEvilness;

    public void Display(string name, string type, string evilness)
    {
        regionName.text = name;
        regionType.text = type;
        regionEvilness.text = evilness;
    }

    public void Clear()
    {
        regionName.text = "";
        regionType.text = "";
        regionEvilness.text = "";
    }
}
