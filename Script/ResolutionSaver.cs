using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class ResolutionSaver : MonoBehaviour
{
    public Dropdown resDropDown;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("width") && PlayerPrefs.HasKey("height"))
        {
            int width = PlayerPrefs.GetInt("width");
            int height = PlayerPrefs.GetInt("height");
            Debug.Log("Playerprefs has width and height keys." + width + ", " + height);
            Screen.SetResolution(width, height, false);

            switch(width) {
                case 1280:
                    resDropDown.value = 0;
                    break;
                case 1920:
                    resDropDown.value = 1;
                    break;
                case 3840:
                    resDropDown.value = 2;
                    break;
                default:
                    resDropDown.value = 1;
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ForceFullHD()
    {
        int width = 1920;
        int height = 1080;
        Screen.SetResolution(width, height, false);
        Debug.Log("Changing resolution to " + width + "," + height);
        PlayerPrefs.SetInt("width", width);
        PlayerPrefs.SetInt("height", height);
        PlayerPrefs.Save();
    }

    public void SetResolution()
    {
        int dropDownValue = resDropDown.value;

        int width = 1920;
        int height = 1080;

        switch (dropDownValue)
        {
            case 0:
                width = 1280;
                height = 720;
                break;
            case 1:
                width = 1920;
                height = 1080;
                break;
            case 2:
                width = 3840;
                height = 2160;
                break;
            default:
                width = 1920;
                height = 1080;
                break;
        }
        Screen.SetResolution(width, height, false);
        Debug.Log("Changing resolution to " + width + "," + height);
        PlayerPrefs.SetInt("width", width);
        PlayerPrefs.SetInt("height", height);
        PlayerPrefs.Save();
    }


}
