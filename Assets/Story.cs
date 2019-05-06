using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Story : MonoBehaviour {
    #region SINGLETON PATTERN
    public static Story _instance;
    public static Story Instance {
        get {
            if (_instance == null) {
                _instance = GameObject.FindObjectOfType<Story>();
            }

            return _instance;
        }
    }
    #endregion

    private bool firstSeedPlaced = false;
    private bool playReel = false;
    private int curReel = -1;
    private float timeCurReelStarted = 0;
    private bool spawnSeeds = false;
    private bool allGrainsSpawned = false;
    private Vector3 lastCameraPos = Vector3.zero;
    Vector3 wheatCameraPos = Vector3.zero;

    public struct Reel {
        public float timeToDisplay;
        public GameObject go;

        public Reel(GameObject go, float timeToDisplay) {
            this.go = go;
            this.timeToDisplay = timeToDisplay;
        }
    }

    public List<Reel> Reels = new List<Reel>();
    public GameObject PlantingInfo;
    public GameObject GerminationInfo;
    public GameObject AnnualPlantInfo;
    public GameObject GrainAmountInfo;
    public GameObject DispersalInfo;
    public GameObject ModernWheat;
    public GameObject Production;
    public GameObject LoadOfBread;
    public GameObject Instructions;
    public GameObject InfoPanel;

    // Start is called before the first frame update
    void Start()
    {
                Reels.Add(new Reel(PlantingInfo, 12f)); //10
                Reels.Add(new Reel(GerminationInfo, 12f));
                Reels.Add(new Reel(AnnualPlantInfo, 12f));
                Reels.Add(new Reel(GrainAmountInfo, 12f));
                Reels.Add(new Reel(DispersalInfo, 12f));
                Reels.Add(new Reel(ModernWheat, 12f));
                Reels.Add(new Reel(Production, 12f));
                Reels.Add(new Reel(LoadOfBread, 12f));
                Reels.Add(new Reel(Instructions, 6f));

//        Reels.Add(new Reel(PlantingInfo, 1f)); //10
//        Reels.Add(new Reel(GerminationInfo, 1f));
//        Reels.Add(new Reel(AnnualPlantInfo, 1f));
//        Reels.Add(new Reel(GrainAmountInfo, 1f));
//        Reels.Add(new Reel(DispersalInfo, 1f));
//        Reels.Add(new Reel(ModernWheat, 1f));
//        Reels.Add(new Reel(Production, 1f));
//        Reels.Add(new Reel(LoadOfBread, 1f));
//        Reels.Add(new Reel(Instructions, 1f));
    }

    // Update is called once per frame
    void Update()
    {
        if (firstSeedPlaced) {
            if (!playReel) {
                NextReel();
            }
            playReel = true;
            
            Vector3 posOffest = new Vector3(10, 10, 10);
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, Wheat.allWheats[0].transform.GetChild(1).position + posOffest, Time.deltaTime/4);
            Camera.main.transform.LookAt(Wheat.allWheats[0].transform.GetChild(1).position);
            Color c = GameObject.Find("Canvas").transform.Find("Fade").GetComponent<Image>().color;
            c.a -= Time.deltaTime/1.5f;
            GameObject.Find("Canvas").transform.Find("Fade").GetComponent<Image>().color = c;
            GameObject.Find("Canvas").transform.Find("FirstSeed").GetComponent<Text>().color = c;
        }

        if (playReel) {
            var reel = Reels[curReel];
            if (Time.time > timeCurReelStarted + reel.timeToDisplay) {
                NextReel();
            }

            if (timeCurReelStarted + reel.timeToDisplay/2 > Time.time) {
                Color c = reel.go.GetComponent<Text>().color;
                c.a += Time.deltaTime;
                reel.go.GetComponent<Text>().color = c;
            }

            if (Time.time > reel.timeToDisplay + timeCurReelStarted - 1) {
                Color c = reel.go.GetComponent<Text>().color;
                c.a -= Time.deltaTime;
                reel.go.GetComponent<Text>().color = c;
            }
        }

        if (spawnSeeds && !allGrainsSpawned) {
            Vector3 avgPos = Vector3.zero;
            foreach (var go in Grain.allGrains) {
                avgPos += go.transform.position;
            }

            avgPos = avgPos / (Grain.allGrains.Count == 0 ? 1 : Grain.allGrains.Count);
            if (Grain.allGrains.Count == 0) {
                avgPos = lastCameraPos;
                if (!allGrainsSpawned) {
                    AllGrainsSpawned();
                }
            }
            else {
                lastCameraPos = avgPos;
            }

            Camera.main.transform.position = avgPos + new Vector3(30, 30, 30);
            Camera.main.transform.LookAt(avgPos);
        }

        if (curReel >= 5 && allGrainsSpawned) {
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, wheatCameraPos + new Vector3(40, 40, 40), Time.deltaTime);
            Camera.main.transform.LookAt(wheatCameraPos);
        }
    }

    void NextReel() {
        if (curReel >= Reels.Count-1) {
            return;
        }

        if (curReel >= 3) {
            if (!spawnSeeds) {
                SpawnSeeds();
            }
        }

        if (curReel >= 4) {
            Vector3 avgPos = Vector3.zero;
            foreach (var wheat in Wheat.allWheats) {
                avgPos += wheat.transform.position;
            }
            avgPos /= Wheat.allWheats.Count;

            wheatCameraPos = avgPos;
        }

        if (curReel >= 7) {
            Camera.main.GetComponent<FlyCamera>().enabled = true;
            InfoPanel.SetActive(true);
        }
        

        timeCurReelStarted = Time.time;
        curReel++;

        if (curReel != 0) {
            Reels[curReel-1].go.SetActive(false);
        }

        Reels[curReel].go.SetActive(true);
        Color c = Reels[curReel].go.GetComponent<Text>().color;
        c.a = 0;
        Reels[curReel].go.GetComponent<Text>().color = c;
    }

    public void FirstSeedPlaced() {
        firstSeedPlaced = true;
    }

    public void AllGrainsSpawned() {
        allGrainsSpawned = true;
    }

    public void SpawnSeeds() {
        spawnSeeds = true;
        firstSeedPlaced = false;

        for (int i = 0; i < 50; i++) {
            Vector3 dest = new Vector3(Random.Range(1f, 60f), 0, Random.Range(1f, 60f));
            dest += (-Wind.constWindDir * Random.Range(45f, 120f)) + Wheat.allWheats[0].transform.GetChild(1).position;
            dest.y = 0;

            Grain.Spawn(Wheat.allWheats[0].transform.GetChild(1).position, dest);
        }
    }

}
