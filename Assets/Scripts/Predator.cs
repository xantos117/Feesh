using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Predator : MonoBehaviour
{
    Material mat;
    Transform target;
    Transform cachedXform;
    public Vector3 velocity;
    public Vector3 forceTotal = Vector3.zero;
    public bool showViewCircle = false;
    public bool feeshOfInterest = false;
    private bool hunting = false;
    private Vector3 preyPos;

    public FeeshSettings fs;
    private void Awake() {
        mat = transform.GetComponentInChildren<MeshRenderer>().material;
        cachedXform = transform;
    }

    private void Update() {
        forceTotal = Vector3.zero;
        cachedXform.position += velocity * Time.deltaTime;

        if (cachedXform.position.sqrMagnitude > fs.wallBoundary * fs.wallBoundary) {
            float mag = cachedXform.position.magnitude;
            cachedXform.position = cachedXform.position.normalized * 0.99f * mag * -1;
            //cachedXform.position *= -1;
        }


        cachedXform.forward = velocity.normalized;
        transform.position = cachedXform.position;
        transform.forward = cachedXform.forward;
        if (hunting) {
            Vector3 targetDir = preyPos - transform.position;
            applyForce(seekForce(targetDir));

        }
        applyForce(randWalkForce() * fs.wanderStrength);

        velocity += forceTotal * Time.deltaTime;
        velocity = velocity.normalized * fs.maxSpeed;
    }

    void applyForce(Vector3 inForce) {
        forceTotal += inForce;
    }

    private Vector3 seekForce(Vector3 targetPos) {
        Vector3 resForce = new Vector3();
        Vector3 curPos = transform.position;
        Vector3 desiredVel = targetPos - curPos;
        desiredVel = desiredVel.normalized * fs.maxSpeed;

        resForce = desiredVel - velocity;

        return resForce;
    }

    private Vector3 randWalkForce() {
        //Vector3 resDir = velocity;
        /*float randVal = Random.value;
        if (randVal < 0.1) {
            resDir = Quaternion.AngleAxis(fs.randWalkAngle, transform.right) * resDir;
        } else if (randVal >= 0.1 && randVal < 0.5) {
            resDir = Quaternion.AngleAxis(fs.randWalkAngle, transform.up) * resDir;
        } else if (randVal >= 0.5 && randVal < 0.9) {
            resDir = Quaternion.AngleAxis(fs.randWalkAngle, -1 * transform.up) * resDir;
        } else {
            resDir = Quaternion.AngleAxis(fs.randWalkAngle, -1 * transform.right) * resDir;
        }*/
        //resDir += Random.insideUnitSphere;


        return Random.insideUnitSphere;
    }

    public void choosePrey(List<Feesh> fList) {
        preyPos = Vector3.zero;
        float minDist = float.MaxValue;
        hunting = false;
        for (int i = 0; i < fList.Count; i++) { 
            Vector3 distanceToTarget = fList[i].transform.position - transform.position;
            float dotToNeighbor = Vector3.Dot(distanceToTarget.normalized, transform.forward.normalized);
            float angleToNeighbor = Mathf.Abs(Mathf.Acos(dotToNeighbor) * 180 / Mathf.PI);
            if (distanceToTarget.sqrMagnitude < fs.predViewDistance * fs.predViewDistance && fs.predViewAngle > angleToNeighbor) {
                hunting = true;
                if(distanceToTarget.magnitude < minDist) {
                    preyPos = fList[i].transform.position;
                    minDist = distanceToTarget.magnitude;
                }
                if (feeshOfInterest) {
                    Debug.DrawLine(transform.position, fList[i].transform.position, Color.green, 0.0f, false);
                }
            }
        }
    }
}
