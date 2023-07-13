using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDelete : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject tMesh;

    public int width;
    public int height;
    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Instantiate(tilePrefab, new Vector3(x - 300, y, 0), Quaternion.identity);
                TextMesh tMeshy = Instantiate(tMesh, new Vector3(x - 300, y, 0), Quaternion.identity).GetComponent<TextMesh>();
                tMeshy.text = x + "," + y;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
