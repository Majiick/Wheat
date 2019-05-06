using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetColor : MonoBehaviour {
    public float r, g, b;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Renderer>().material.color = new Color(r, g, b);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
