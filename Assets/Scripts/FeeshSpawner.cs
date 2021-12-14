using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeeshSpawner : MonoBehaviour
{
    public Feesh feeshFab;
    public Predator predFab;
    public List<Feesh> feeshList;
    public List<Predator> predList;
    public FeeshSettings fSettings;
    public float spawnRadius = 10;
    public int spawnCount = 10;
    public int predSpawnCount = 1;
    public float initSpeed = 2;
    public float initViewDistance = 1;
    public Feesh feeshLeader;

    public float avoidance = 1;
    public float attraction = 1;
    public float alignment = 1;
    public float followTheLeader = 1;
    public float rotationSpeed = 1;
    [Range(0, 180)]
    public float viewAngle = 180;
    public float speedSpringConst = 5;
    public float wallSpringConst = -5;
    private Slider sepSlider;
    private Slider cohSlider;
    private Slider alignSlider;
    private Slider folSlider;
    private Slider vdSlider;
    private Slider vaSlider;
    private Slider pvdSlider;
    private Slider pvaSlider;
    private Slider fleeSlider;
    private Slider revSlider;
    private Slider clusSlider;
    private Slider tanSlider;

    // Start is called before the first frame update
    private void Start() {
        sepSlider = GameObject.Find("SeparationSlider").GetComponent<Slider>();
        cohSlider = GameObject.Find("CoherenceSlider").GetComponent<Slider>();
        alignSlider = GameObject.Find("AlignmentSlider").GetComponent<Slider>();
        folSlider = GameObject.Find("FollowSlider").GetComponent<Slider>();
        vdSlider = GameObject.Find("ViewDistSlider").GetComponent<Slider>();
        vaSlider = GameObject.Find("ViewAngleSlider").GetComponent<Slider>();
        pvdSlider = GameObject.Find("PredViewDistSlider").GetComponent<Slider>();
        pvaSlider = GameObject.Find("PredViewAngleSlider").GetComponent<Slider>();
        fleeSlider = GameObject.Find("FleeSlider").GetComponent<Slider>();
        revSlider = GameObject.Find("ReverseSlider").GetComponent<Slider>();
        clusSlider = GameObject.Find("ClusterSlider").GetComponent<Slider>();
        tanSlider = GameObject.Find("TangentSlider").GetComponent<Slider>();
    }

    void Awake()
    {
        feeshList = new List<Feesh>();
        predList = new List<Predator>();
        for (int i = 0; i < spawnCount; i++) {
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            Feesh fish = Instantiate(feeshFab);
            feeshList.Add(fish);
            fish.transform.position = pos;
            fish.transform.forward = Random.insideUnitSphere;
            fish.leader = feeshLeader;
            fish.fs = fSettings;
        } 
        for(int i = 0;i<predSpawnCount;i++) {
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            Predator pred = Instantiate(predFab);
            predList.Add(pred);
            pred.transform.position = pos;
            pred.transform.forward = Random.insideUnitSphere;
            pred.fs = fSettings;
        }
    }

    public void doubleAttract() {
        fSettings.attraction *= 2;
        fSettings.avoidance /= 2;
    }
    public void doubleAvoid() {
        fSettings.attraction /= 2;
        fSettings.avoidance *= 2;
    }

    public void doubleFollowLeader() {
        fSettings.followTheLeader *= 2;
    }

    public void doubleAlign() {
        fSettings.alignment *= 2;
    }

    // Update is called once per frame
    void Update()
    {
        normalizeWeights();
        for (int i = 0; i < spawnCount; i++) {
            feeshList[i].checkNeighbors(feeshList,predList);
        }
        for(int i = 0;i < predSpawnCount; i++) {
            predList[i].choosePrey(feeshList);
        }
        /*vaSlider.value = fSettings.viewAngle;
        vdSlider.value = fSettings.viewDistance;
        pvaSlider.value = fSettings.predViewAngle;
        pvdSlider.value = fSettings.predViewDistance;*/
    }

    void normalizeWeights() {
        float sumWeights = fSettings.avoidance + fSettings.attraction + fSettings.alignment + fSettings.followTheLeader;
        float sumPredWeights = fSettings.reverseWeight + fSettings.fleeWeight + fSettings.centerWeight + fSettings.perpWeight;
        fSettings.avoidance /= sumWeights;
        fSettings.attraction /= sumWeights;
        fSettings.alignment /= sumWeights;
        fSettings.followTheLeader /= sumWeights;
        fSettings.reverseWeight /= sumPredWeights;
        fSettings.fleeWeight /= sumPredWeights;
        fSettings.centerWeight /= sumPredWeights;
        fSettings.perpWeight /= sumPredWeights;

        sepSlider.value = fSettings.avoidance;
        cohSlider.value = fSettings.attraction;
        alignSlider.value = fSettings.alignment;
        folSlider.value = fSettings.followTheLeader;
        fleeSlider.value = fSettings.fleeWeight;
        revSlider.value = fSettings.reverseWeight;
        clusSlider.value = fSettings.centerWeight;
        tanSlider.value = fSettings.perpWeight;
    }
}
