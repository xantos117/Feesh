using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feesh : Mover
{
    // leader values
    public float keyRadius = 5.0f;
    public float angleRads = 0.0f;
    public Feesh leader;
    public bool amLeader = false;

    // internal values regarding neighbors, updated by the spawner
    private Vector3 neighborCenter;
    private Vector3 neighborDistance;
    private Vector3 neighborDirection;
    private Vector3 predCenter;
    private Vector3 predDirection;
    private bool fleeing = false;
    private float panicking = 0.0f;

    private void Update() {
        cachedXform.position += velocity * Time.deltaTime;

        // feesh that leave the boundary get teleported to the inside of the opposite side
        if(cachedXform.position.sqrMagnitude > fs.wallBoundary * fs.wallBoundary) {
            float mag = cachedXform.position.magnitude;
            cachedXform.position = cachedXform.position.normalized * 0.99f * mag * -1;
            //cachedXform.position *= -1;
        }

        applyForce(randWalkForce() * fs.wanderStrength);

        // move differently dependent on state
        if (amLeader) {
            // calculate keyed circular movement
            Vector3 acceleration = Vector3.zero;
            acceleration = acceleration - transform.position;
            acceleration.Normalize();
            acceleration *= fs.angVel;
            velocity += acceleration * Time.deltaTime;
            angleRads += fs.angVel * Time.deltaTime;
            if(angleRads > 2 * Mathf.PI) {
                angleRads -= 2 * Mathf.PI;
            }

            cachedXform.position = new Vector3(keyRadius * Mathf.Sin(angleRads), 0, keyRadius * Mathf.Cos(angleRads));
            cachedXform.forward = Vector3.Cross(Vector3.up, cachedXform.position).normalized;
            transform.position = cachedXform.position;
            transform.forward = cachedXform.forward;
        } else if (fleeing || panicking < fs.panicTime) {
            panicking += Time.deltaTime;
            cachedXform.forward = velocity.normalized;
            transform.position = cachedXform.position;
            transform.forward = cachedXform.forward;
            // a panicking feesh only cares about predator data, as well as the best cluster of neighbors to hide in
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
            // normal flocking behavior
            applyForce(seekForce(leaderPos) * fs.followTheLeader);
            applyForce(seekForce(neighborCenter) * fs.attraction);
            applyForce(seekForce(neighborDistance) * -1 * fs.avoidance);
            applyForce(alignForce(neighborDirection) * fs.alignment);

            velocity += forceTotal * Time.deltaTime;
            velocity = velocity.normalized * fs.maxSpeed;
        }
    }
    // enable some visualization in the editor
    private void OnDrawGizmos() {
        if (showViewCircle) {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(this.transform.position, this.fs.viewDistance);
            Gizmos.DrawRay(transform.position, transform.forward);
        }
    }

    // iterate over neighbors provided by the spawner, calculate important values
    public void checkNeighbors(List<Feesh> fList,List<Predator> pList) {
        // some initialization
        Vector3 avgNeighborPosition = new Vector3();
        Vector3 separationForce = new Vector3();
        Vector3 avgFwd = velocity;
        int nNeighbors = 0;
        // iterate over fellow Feesh
        foreach(Feesh fsh in fList) {
            if (this.GetInstanceID() != fsh.GetInstanceID()) {
                Vector3 distanceToNeighbor = fsh.transform.position - transform.position;
                float dotToNeighbor = Vector3.Dot(distanceToNeighbor.normalized, transform.forward.normalized);
                float angleToNeighbor = Mathf.Abs(Mathf.Acos(dotToNeighbor) * 180 / Mathf.PI);
                // only gather information about neighbors within the view bubble
                if (distanceToNeighbor.sqrMagnitude < fs.viewDistance * fs.viewDistance && fs.viewAngle > angleToNeighbor) {
                    nNeighbors++;
                    avgNeighborPosition += fsh.transform.position;
                    avgFwd += (fsh.velocity);
                    if (feeshOfInterest) {
                        Debug.DrawLine(transform.position, fsh.transform.position, Color.green, 0.0f, false);
                    }
                }
                // separation force is the distance to the neighbor over it's squared magnitude, so long as that distance is within the avoidance radius and the neighbor is visible
                if (distanceToNeighbor.sqrMagnitude < fs.avoidanceRadius * fs.avoidanceRadius && fs.viewAngle > angleToNeighbor) {
                    separationForce += ( distanceToNeighbor / (distanceToNeighbor.sqrMagnitude));
                }
            }

        }
        // reset some fleeing values
        fleeing = false;
        int nPredators = 0;
        Vector3 predDir = Vector3.zero;
        Vector3 avgPredPos = Vector3.zero;
        // iterate over predators
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
