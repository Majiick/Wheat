using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grain : MonoBehaviour {
    private Vector3 target = Vector3.zero;
    public static List<GameObject> allGrains = new List<GameObject>();

    // Start is called before the first frame update
    void Start() {
        Seek seek = gameObject.AddComponent<Seek>();
        seek.target = target;

        GetComponent<Boid>().behaviours.Add(seek);
    }

    // Update is called once per frame
    void Update() {
        var wind = GetComponent<Wind>();
//        Debug.Log(Vector3.Distance(GetComponent<Seek>().target, transform.position));
        if (transform.position.y < 10 && Vector3.Distance(GetComponent<Seek>().target, transform.position) > 20) {
            var pos = transform.position;
            pos.y += Random.Range(0.1f, 1f) * Time.deltaTime * 5;
            transform.position = pos;
        }

        if (Vector3.Distance(GetComponent<Seek>().target, transform.position) < 20) {
            GetComponent<Boid>().maxForce = 1000;
            GetComponent<Boid>().maxSpeed = 50;
        }
        else {
            transform.position += wind.windDir * wind.windMultiplier * 0.1f;
        }
    }

    public static void Spawn(Vector3 spawnLocation, Vector3 dest) {
        GameObject prefab = Resources.Load<GameObject>("Grain");
        GameObject go = Instantiate(prefab);
        go.transform.position = spawnLocation;
        go.GetComponent<Grain>().target = dest;
        allGrains.Add(go);
    }

    void OnCollisionEnter(Collision collision) {
        ContactPoint contact = collision.contacts[0];
        Vector3 pos = contact.point;
        allGrains.Remove(gameObject);
        Terrain.Instance.MakeRipple(pos, 10);
        Destroy(gameObject);

        Wheat.Spawn(pos);
    }
}
