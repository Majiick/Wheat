using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update() {
        int rotSpeed;
        if (transform.GetChild(0).transform.position.y > 0) {
            rotSpeed = 5;
        }
        else {
            rotSpeed = 180;
        }

        transform.Rotate(Vector3.right, rotSpeed * Time.deltaTime);
    }
}
