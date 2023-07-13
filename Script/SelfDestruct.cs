using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float timeToSelfDestruct;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timeToSelfDestruct);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
