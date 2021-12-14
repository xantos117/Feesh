using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    public FeeshSettings fs;
    protected Material mat;
    protected Transform target;
    protected Transform cachedXform;
    public Vector3 velocity;
    public Vector3 forceTotal = Vector3.zero;
    public bool showViewCircle = false;
    public bool feeshOfInterest = false;
    private void Awake() {
        mat = transform.GetComponentInChildren<MeshRenderer>().material;
        cachedXform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // function for cleanly adding to the total force for a frame
    protected void applyForce(Vector3 inForce) {
        forceTotal += inForce;
    }
    // calculate the tangent direction from the predator velocity in the plane shared by the two velocities
    protected Vector3 tanForce(Vector3 predDir) {
        // get the direction directly away from the predator's velocity
        Vector3 orth = Vector3.Cross(velocity, predDir);
        Vector3 desVel = Vector3.Cross(predDir, orth);
        desVel.Normalize();
        return desVel * fs.maxSpeed;
    }
    // align with average velocity 
    protected Vector3 alignForce(Vector3 targetDir) {
        Vector3 resForce = targetDir.normalized - velocity.normalized;

        return resForce;
    }
    // go towards target point
    protected Vector3 seekForce(Vector3 targetPos) {
        Vector3 resForce = new Vector3();
        Vector3 curPos = transform.position;
        Vector3 desiredVel = targetPos - curPos;
        desiredVel = desiredVel.normalized * fs.maxSpeed;

        resForce = desiredVel - velocity;

        return resForce;
    }
    // have some random change in movement, best modulated with a low effect weight
    protected Vector3 randWalkForce() {
        return Random.insideUnitSphere * fs.maxSpeed;
    }
}
