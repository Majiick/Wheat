using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class LSystem3D : MonoBehaviour {
    private List<Vector3> savedPositions;
    private Vector3[] savedPositionsArr;
    private int segmentIterator = 0;

    struct PosAndRot {
        public Quaternion rot;
        public Vector3 pos;

        public PosAndRot(Vector3 pos, Quaternion rot) {
            this.pos = pos;
            this.rot = rot;
        }
    }

    public LineRenderer lr;
    // Use this for initialization
    void Start() {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        transform.Rotate(new Vector3(-90, 0, 0));
        float rotation = 22.5f;

        string lSystem = "A";
        Dictionary<string, string> rules = new Dictionary<string, string>() {
                {"A", "[&FL!A]/////’[&FL!A]///////’[&FL!A]"},
                {"F",  "S/////F"},
                {"S", "FL"},
                {"L", "[’’’∧∧{-f+f+f-|-f+f+f}]"}
            };
        foreach (var rule in rules) {
            Debug.Log(rule.Key + ": " + rule.Value);
        }
        Debug.Log("\n");

        for (int i = 0; i < 8; i++) {
            StringBuilder next = new StringBuilder();
            foreach (char c in lSystem) {
                if (rules.ContainsKey(c.ToString())) {
                    next.Append(rules[c.ToString()]);
                } else {
                    next.Append(c);
                }
            }

            lSystem = next.ToString();
        }

        List<Vector3> positions = new List<Vector3>() { transform.position };
        Stack<PosAndRot> stack = new Stack<PosAndRot>();
        for (int i = 0; i < lSystem.Length; i++) {
            switch (lSystem[i]) {
                case 'F':
                    transform.Translate(Vector3.forward, Space.Self);
                    break;
                case 'f':
                    transform.Translate(Vector3.forward, Space.Self);
                    break;
                case '-':
                    transform.Rotate(Vector3.up, -rotation);
                    break;
                case '+':
                    transform.Rotate(Vector3.up, +rotation);
                    break;
                case '[':
                    stack.Push(new PosAndRot(transform.position, transform.rotation));
                    break;
                case ']':
                    var posAndRot = stack.Pop();
                    transform.position = posAndRot.pos;
                    transform.rotation = posAndRot.rot;
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
            }

            positions.Add(transform.position);
        }

        savedPositions = positions;
        savedPositionsArr = savedPositions.ToArray();
        transform.position = new Vector3(128, 0, 128);
        transform.rotation = Quaternion.identity;
//        lr.SetPositions(savedPositionsArr);
//        lr.positionCount = savedPositionsArr.Length;
    }

    private bool setLr = false;
    // Update is called once per frame
    void Update() {
        if (Input.anyKeyDown) {
            
//            segmentIterator += 200;
        }

        if (!setLr) {
            setLr = true;
            lr.positionCount = savedPositionsArr.Length;
            lr.SetPositions(savedPositionsArr);
            
        }
        
    }
}
