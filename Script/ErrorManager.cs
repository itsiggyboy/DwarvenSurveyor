using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ErrorManager : MonoBehaviour
{
    int i;
    public GameObject errorPrefab;
    public GameObject clearButton;
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
        clearButton.SetActive(true);
        i++;
        if (transform.childCount > 5)
        {
            for (int i = transform.childCount - 1; i > 0; i--)
            {
                if(i > 5)
                {
                    Destroy(transform.GetChild(i).gameObject);
                }

            }
            Destroy(transform.GetChild(transform.childCount-1).gameObject);
        }
        string fullString = i + "| "  + System.DateTime.Now.Hour + ":" +  System.DateTime.Now.Minute + ":" +System.DateTime.Now.Second + " | " + s;
        GameObject go = Instantiate(errorPrefab);
        go.transform.parent = transform;
        go.name = fullString;
        go.transform.SetSiblingIndex(0);
        go.transform.GetChild(0).GetComponent<Text>().text = fullString;
        go.transform.GetChild(0).GetComponent<Text>().color = c;

    }

    public void Clear()
    {
        if (transform.childCount > 0)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        clearButton.SetActive(false);
    }
}
