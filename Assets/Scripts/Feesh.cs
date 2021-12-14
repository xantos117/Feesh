using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feesh : MonoBehaviour
{
    Material mat;
    Transform target;
    Transform cachedXform;

    public FeeshSettings fs;
    public float keyRadius = 5.0f;
    public float angleRads = 0.0f;

    public Vector3 velocity;
    public Vector3 forceTotal = Vector3.zero;
    public bool showViewCircle = false;
    public bool feeshOfInterest = false;
    private Vector3 neighborCenter;
    private Vector3 neighborDistance;
    private Vector3 neighborDirection;
    private Vector3 predCenter;
    private Vector3 predDirection;
    public Feesh leader;
    private bool fleeing = false;
    private float panicking = 0.0f;

    public bool amLeader = false;

    private void Awake() {
        mat = transform.GetComponentInChildren<MeshRenderer>().material;
        cachedXform = transform;
    }

    private void Update() {
        Vector3 acceleration = Vector3.zero;
        acceleration = acceleration - transform.position;
        acceleration.Normalize();
        acceleration *= fs.angVel;
        cachedXform.position += velocity * Time.deltaTime;

        if(cachedXform.position.sqrMagnitude > fs.wallBoundary * fs.wallBoundary) {
            float mag = cachedXform.position.magnitude;
            cachedXform.position = cachedXform.position.normalized * 0.99f * mag * -1;
            //cachedXform.position *= -1;
        }

        Vector3 curPos = transform.position;
        angleRads += fs.angVel * Time.deltaTime;
        if(angleRads > 2 * Mathf.PI) {
            angleRads -= 2 * Mathf.PI;
        }

        if (amLeader) {
            velocity += acceleration * Time.deltaTime;

            cachedXform.position = new Vector3(keyRadius * Mathf.Sin(angleRads), 0, keyRadius * Mathf.Cos(angleRads));
            cachedXform.forward = Vector3.Cross(Vector3.up, cachedXform.position).normalized;
            transform.position = cachedXform.position;
            transform.forward = cachedXform.forward;
        } else if (fleeing || panicking < fs.panicTime) {
            panicking += Time.deltaTime;
            cachedXform.forward = velocity.normalized;
            transform.position = cachedXform.position;
            transform.forward = cachedXform.forward;

            applyForce(seekForce(predCenter) * -1 * fs.fleeWeight);
            applyForce(alignForce(predDirection) * -1 * fs.reverseWeight);
            applyForce(seekForce(neighborCenter) * fs.centerWeight);
            applyForce(tanForce(predDirection) * fs.perpWeight);

            velocity += forceTotal * Time.deltaTime;
            velocity = velocity.normalized * fs.maxSpeed;
        } else {
            cachedXform.forward = velocity.normalized;
            transform.position = cachedXform.position;
            transform.forward = cachedXform.forward;
            Vector3 leaderPos = leader.transform.position;
            applyForce(seekForce(leaderPos) * fs.followTheLeader);

            applyForce(seekForce(neighborCenter) * fs.attraction);

            applyForce(seekForce(neighborDistance) * -1 * fs.avoidance);

            applyForce(alignForce(neighborDirection) * fs.alignment);

            velocity += forceTotal * Time.deltaTime;
            velocity = velocity.normalized * fs.maxSpeed;
        }
    }

    void applyForce(Vector3 inForce) {
        forceTotal += inForce;
    }

    private Vector3 tanForce(Vector3 predDir) {
        // get the direction directly away from the predator's velocity
        Vector3 orth = Vector3.Cross(velocity,predDir);
        Vector3 desVel = Vector3.Cross(predDir,orth);
        desVel.Normalize();
        return desVel * fs.maxSpeed;
    }

    private Vector3 seekForce(Vector3 targetPos) {
        Vector3 resForce = new Vector3();
        Vector3 curPos = transform.position;
        Vector3 desiredVel = targetPos - curPos;
        desiredVel = desiredVel.normalized * fs.maxSpeed;

        resForce = desiredVel - velocity;

        return resForce;
    }

    private Vector3 alignForce(Vector3 targetDir) {
        Vector3 resForce = targetDir.normalized - velocity.normalized;

        return resForce;
    }

    private void OnDrawGizmos() {
        if (showViewCircle) {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(this.transform.position, this.fs.viewDistance);
            Gizmos.DrawRay(transform.position, transform.forward);
        }
    }

    public void checkNeighbors(List<Feesh> fList,List<Predator> pList) {
        Vector3 avgNeighborPosition = new Vector3();
        Vector3 separationForce = new Vector3();
        Vector3 avgFwd = velocity;
        int nNeighbors = 0;
        for (int i = 0; i < fList.Count; i++) {
            if (this.GetInstanceID() != fList[i].GetInstanceID()) {
                Vector3 distanceToNeighbor = fList[i].transform.position - transform.position;
                float dotToNeighbor = Vector3.Dot(distanceToNeighbor.normalized, transform.forward.normalized);
                float angleToNeighbor = Mathf.Abs(Mathf.Acos(dotToNeighbor) * 180 / Mathf.PI);
                if (distanceToNeighbor.sqrMagnitude < fs.viewDistance * fs.viewDistance && fs.viewAngle > angleToNeighbor) {
                    nNeighbors++;
                    avgNeighborPosition += fList[i].transform.position;
                    avgFwd += (fList[i].velocity);
                    if (feeshOfInterest) {
                        Debug.DrawLine(transform.position, fList[i].transform.position, Color.green, 0.0f, false);
                    }
                }
                if (distanceToNeighbor.sqrMagnitude < fs.avoidanceRadius * fs.avoidanceRadius && fs.viewAngle > angleToNeighbor) {
                    separationForce += ( distanceToNeighbor / (distanceToNeighbor.sqrMagnitude));
                }
            }

        }
        fleeing = false;
        int nPredators = 0;
        Vector3 predDir = Vector3.zero;
        Vector3 avgPredPos = Vector3.zero;
        foreach(Predator p in pList) {
            Vector3 threatDist = p.transform.position - transform.position;
            float dotToThreat = Vector3.Dot(threatDist.normalized, transform.forward.normalized);
            float angleToThreat = Mathf.Abs(Mathf.Acos(dotToThreat) * 180 / Mathf.PI);
            if(threatDist.sqrMagnitude < fs.viewDistance * fs.viewDistance && fs.viewAngle > angleToThreat) {
                Debug.Log("Panicking %s", this);
                fleeing = true;
                panicking = 0.0f;
                nPredators++;
                predDir += p.velocity;
                avgPredPos += p.transform.position;
            }
        }
        if (nNeighbors > 0) {
            avgNeighborPosition /= nNeighbors;
            avgFwd /= nNeighbors;
            neighborDistance = separationForce;
            neighborCenter = (avgNeighborPosition - transform.position);
            neighborDirection = avgFwd;
        }
        if(nPredators > 0) {
            avgPredPos /= nPredators;
            predDir /= nPredators;
            predDirection = predDir;
            predCenter = avgPredPos;
        }

        if (feeshOfInterest) {
            Debug.DrawLine(transform.position, transform.position + neighborCenter, Color.red, 0.0f, false);
        }

    }
}
