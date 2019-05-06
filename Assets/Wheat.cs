using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheat : MonoBehaviour
{
    /*
    a is an Apex. I is the Internode. A is the flowering apex.
    S is a grain. C is a level of grains. E is the end.
    R sets the rotation. D sets the forward distance.
     */
    public static List<GameObject> allWheats = new List<GameObject>();

    private List<Vector3> savedPositions;
    struct PosAndRotAndRotValueAndForwardDistance {
        public Quaternion rot;
        public Vector3 pos;
        public float rotValue;
        public float forwardDistance;

        public PosAndRotAndRotValueAndForwardDistance(Vector3 pos, Quaternion rot, float rotValue, float forwardDistance) {
            this.pos = pos;
            this.rot = rot;
            this.rotValue = rotValue;
            this.forwardDistance = forwardDistance;
        }
    }

    private StochasticLSystem lSystem = new StochasticLSystem("a");
    public LineRenderer lr;
    private float segmentIterator = 0;

    void Start() {
        lSystem.InsertRule("a", "D1.0Fa", 0.9);
        lSystem.InsertRule("a", "A", 0.1);
        lSystem.InsertRule("A", "MD0.3C", 1.0);
        lSystem.InsertRule("C", "SF[R90&[R45^S][R45)R90-S][R45&R90--S][R45/R90---S]]C", 0.98);
        lSystem.InsertRule("C", "SE", 0.02);
        // For a line through the middle of the spikelet: [R30&F^^F][R90/R30&F^^F][R90//R30&F^^F][R90///R30&F^^F]
        lSystem.InsertRule("S", "[R30&F^[^F]][R90/R30&F^[^F]][R90//R30&F^[^F]][R90///R30&F^[^F]]", 1.0f);

        string lSystemResult = lSystem.GetSystem(40);
        lSystemResult = "R90^" + lSystemResult;  // To turn it upright
        Debug.Log(lSystemResult);

        float rotation = 0;
        float forwardDistance = 0;
        List<Vector3> positions = new List<Vector3>() { transform.position };
        Stack<PosAndRotAndRotValueAndForwardDistance> stack = new Stack<PosAndRotAndRotValueAndForwardDistance>();
        for (int i = 0; i < lSystemResult.Length; i++) {
            switch (lSystemResult[i]) {
                case 'F':
                    transform.Translate(Vector3.forward * forwardDistance, Space.Self);
                    break;
                case 'f':
                    transform.Translate(Vector3.forward * forwardDistance, Space.Self);
                    break;
                case '-':
                    transform.Rotate(Vector3.up, -rotation);
                    break;
                case '+':
                    transform.Rotate(Vector3.up, +rotation);
                    break;
                case '[':
                    stack.Push(new PosAndRotAndRotValueAndForwardDistance(transform.position, transform.rotation, rotation, forwardDistance));
                    break;
                case ']':
                    var posAndRot = stack.Pop();
                    transform.position = posAndRot.pos;
                    transform.rotation = posAndRot.rot;
                    rotation = posAndRot.rotValue;
                    forwardDistance = posAndRot.forwardDistance;
                    break;
                case '/':
                    transform.Rotate(Vector3.forward, rotation);
                    break;
                case ')':
                    transform.Rotate(Vector3.forward, -rotation);
                    break;
                case '&':
                    transform.Rotate(Vector3.right, rotation);
                    break;
                case '^':
                    transform.Rotate(Vector3.right, -rotation);
                    break;
                case '|':
                    transform.Rotate(Vector3.up, 180f);
                    break;
                case 'R':
                    string newRotStr = "";
                    for (int j = 0; j < 3; j++) {
                        if (char.IsDigit(lSystemResult[i + 1 + j])) {
                            newRotStr += lSystemResult[i + 1 + j];
                        }
                    }

                    rotation = float.Parse(newRotStr);
                    break;
                case 'D':
                    string newDistanceString = "";
                    for (int j = 0; j < 3; j++) {
                        if (char.IsDigit(lSystemResult[i + 1 + j]) || lSystemResult[i + 1 + j] == '.') {
                            newDistanceString += lSystemResult[i + 1 + j];
                        }
                    }

                    forwardDistance = float.Parse(newDistanceString);
                    break;
            }

            positions.Add(transform.position);
        }

        savedPositions = positions;
    }


    void Update() {
        Vector3 pos = transform.parent.position;
        Wind wind = GetComponent<Wind>();

        float highestPoint = float.MinValue;
        float curHighestPoint = savedPositions[0].y;
        int c = 0;
        foreach (var savedPosition in savedPositions) {
            c++;
            if (savedPosition.y > highestPoint) highestPoint = savedPosition.y;

            if (segmentIterator > c) {
                if (savedPosition.y > curHighestPoint) curHighestPoint = savedPosition.y;
            }
        }
        Debug.Assert(highestPoint > 0); // Won't work otherwise
        transform.parent.GetChild(1).transform.localPosition = new Vector3(0, curHighestPoint, 0);

        // Using SIMD here will greatly speed this up.
        Vector3[] newPositions = new Vector3[savedPositions.Count];
        int i;
        for (i = 0; i < savedPositions.Count; i++) {
            float windEffect = Wind.WindFunction(savedPositions[i].y / highestPoint, highestPoint) * wind.windMultiplier;
//            Debug.Log(windEffect);
            newPositions[i] = pos + savedPositions[i] + (wind.windDir * windEffect);
        }

        if (segmentIterator >= savedPositions.Count) {
            segmentIterator = savedPositions.Count;
        }

        lr.positionCount = (int)segmentIterator;
        lr.SetPositions(newPositions);
        segmentIterator += Time.deltaTime*360;
//        Debug.Break();
    }

    public static void Spawn(Vector3 pos) {
        GameObject wheatPrefab = Resources.Load<GameObject>("Wheat");
        GameObject newWheat = Instantiate(wheatPrefab);
        newWheat.transform.position = pos;
        newWheat.transform.GetChild(0).position = Vector3.zero;
        allWheats.Add(newWheat);
        GameObject.Find("Sound").GetComponent<AudioSource>().Play();
    }
}
