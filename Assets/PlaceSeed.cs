using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceSeed : MonoBehaviour {
    private bool firstSeed = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((firstSeed && Input.GetMouseButtonDown(0)) || Input.GetMouseButtonDown(1)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit)) {
                if (hit.transform.name == "Wheat" || hit.transform.name == "Wheat(Clone)") {
                    return;
                }
                Wheat.Spawn(hit.point);
                Terrain.Instance.MakeRipple(hit.point, firstSeed ? 258 : 60);


                if (firstSeed) {
                    Story.Instance.FirstSeedPlaced();
                    Wind.constWindDir = -(new Vector3(128, 0, 128) - hit.point).normalized;
                }

                firstSeed = false;
//                var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                g.transform.position = hit.point;
            }
        }
    }
}
