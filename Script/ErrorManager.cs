using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorManager : MonoBehaviour
{

    public GameObject errorPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [ContextMenu("Test")]
    public void RandomError()
    {
        GenerateError("Test", Color.blue);
    }
    public void GenerateError(string s, Color c)
    {
        string fullString = System.DateTime.Now.Hour + ":" +  System.DateTime.Now.Minute + ":" +System.DateTime.Now.Second + " | " + s;
        GameObject go = Instantiate(errorPrefab);
        go.transform.parent = transform;
        go.name = fullString;
        go.transform.SetSiblingIndex(0);
        go.transform.GetChild(0).GetComponent<Text>().text = fullString;
        go.transform.GetChild(0).GetComponent<Text>().color = c;
        if (transform.childCount > 5)
        {
            Destroy(transform.GetChild(transform.childCount-1).gameObject);
        }
    }
}
