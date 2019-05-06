using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour {
    public Text grainsText;
    public Text grainWeight;
    public Text flourWeight;
    public Text flourSale;
    public Text loaf;

    private int grains;
    private double grainmg;
    private double flourg;
    private double saleeuro;
    private double bread;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit)) {
                if (hit.transform.name == "Wheat" || hit.transform.name == "Wheat(Clone)") {
                    Destroy(hit.transform.gameObject);

                    grains += Random.Range(40, 80);
                    grainmg = grains * 50;
                    flourg = grainmg * 0.96 / 1000;
                    saleeuro = 0.00000175 * flourg;
                    bread = flourg / 512 * 100;

                    GameObject.Find("Cut").GetComponent<AudioSource>().Play();
                }
            }
        }

        Debug.Log(saleeuro);
        grainsText.text = "Grains: " + grains;
        grainWeight.text = "Grain weight: " + grainmg + "mg";
        flourWeight.text = "Flour weight: " + flourg + "g";
        flourSale.text = "Flour euro value: " + saleeuro.ToString("E5") + "€";
        loaf.text = "Loaf completion: " + bread + "%";
    }
}
