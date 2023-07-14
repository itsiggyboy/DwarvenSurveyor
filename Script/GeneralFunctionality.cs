using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralFunctionality : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Maximize()
    {
        Screen.fullScreen = true;
    }
    public void Minimize()
    {
        Screen.fullScreen = false;
    }

    public void Close()
    {
        Application.Quit();
    }
}
