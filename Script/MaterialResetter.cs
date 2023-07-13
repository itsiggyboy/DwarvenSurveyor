using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialResetter : MonoBehaviour
{
    public Color initialColor;
    public Material material;

    //public bool Counting = true;
    //float counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(material.color != initialColor)
        {
            //material.color = initialColor;// Color.Lerp(material.color, initialColor, Time.time);
        }
    }
}
