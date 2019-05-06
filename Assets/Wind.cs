using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public static Vector3 constWindDir = Vector3.forward;
    public Vector3 windDir = Vector3.forward;
    public Vector3 windDirTarget = Vector3.forward;
    public float windMultiplier = 0f;
    private int multiplierDir = -1;
    private float windSpeed = 1.0f;
    private float windSpeedTarget = 1.0f;


    private float constMinWindMultiplier = -1.5f;
    private float constMaxWindMultiplier = -0.5f;
    private float minWindMultiplier = 0;
    private float maxWindMultiplier = 0;

    public static float WindFunction(float x, float maxHeight) {
        float heightMultiplier = 0;
        if (maxHeight > 10) {
            heightMultiplier = 1.0f;
        }
        else {
            heightMultiplier = maxHeight / 10f;
        }
        return Mathf.Pow(x, 2) * heightMultiplier;
    }

    // Start is called before the first frame update
    void Start() {
        minWindMultiplier = constMinWindMultiplier;
        maxWindMultiplier = constMaxWindMultiplier;
    }

    // Update is called once per frame
    void Update() {
        float quake = 1;
        if (Random.Range(0f, 11f) > 9.0f) {
            quake = Random.Range(3, 10);
        }

        if (Random.Range(0f, 11f) > 9.0f) {
            windSpeedTarget += Random.Range(0.1f, 1.0f);
        }

        if (Random.Range(0f, 11f) > 9.0f) {
            windDirTarget += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        }

        windDir = Vector3.Lerp(windDir, windDirTarget, Time.deltaTime * 2).normalized;
        windDirTarget = Vector3.Lerp(windDirTarget, constWindDir, Time.deltaTime * 3); // Always lerp back to constWindDir.

        windSpeed = Mathf.Lerp(windSpeed, windSpeedTarget, Time.deltaTime * 3);

        windMultiplier += Time.deltaTime * 0.5f * multiplierDir * quake * windSpeed;

        windSpeedTarget = Mathf.Lerp(windSpeedTarget, 1.0f, Time.deltaTime * 10); // Always lerp back to 1.


        if (windMultiplier < minWindMultiplier) {
            multiplierDir = 1;
        }

        if (windMultiplier > maxWindMultiplier) {
            multiplierDir = -1;
        }
    }
}
