using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FeeshSettings : ScriptableObject {

    public float maxSpeed = 2;
    public float viewDistance = 1;

    public float avoidance = 1;
    public float attraction = 1;
    public float alignment = 1;
    public float followTheLeader = 1;
    public float rotationSpeed = 1;
    [Range(0, 180)]
    public float viewAngle = 180;
    public float speedSpringConst = 5;
    public float wallSpringConst = -5;
    public float angVel = 0.98f;

    public float avoidanceRadius = 1;


    [Header("Panic")]
    public float fleeWeight = 1;
    public float reverseWeight = 1;
    public float centerWeight = 1;
    public float perpWeight = 1;
    public float panicTime = 3;

    [Header("Bounds")]
    public float wallBoundary = 15;

    [Header("Predator")]
    public float randWalkAngle = 30;
    public float wanderStrength = 0.1f;
    public float predViewDistance = 2;
    [Range(0, 180)]
    public float predViewAngle = 120;

    public void SetAlignment(float f) {
        alignment = f;
    }
    public void SetAvoidance(float f) {
        avoidance = f;
    }
    public void SetAttraction(float f) {
        attraction = f;
    }
    public void SetFollow(float f) {
        followTheLeader = f;
    }
    public void SetViewDist(float f) {
        viewDistance = f;
    }
    public void SetViewAngle(float f) {
        viewAngle = f;
    }
    public void SetPredViewDist(float f) {
        predViewDistance = f;
    }
    public void SetPredViewAngle(float f) {
        predViewAngle = f;
    }
    public void SetPanicFlee(float f) {
        fleeWeight = f;
    }
    public void SetPanicOppose(float f) {
        reverseWeight = f;
    }
    public void SetPanicCenter(float f) {
        centerWeight = f;
    }
    public void setPanicTangent(float f) {
        perpWeight = f;
    }
    public void setPanicTime(float f) {
        panicTime = f;
    }

}