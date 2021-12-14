using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctNode<T>
{
    public List<OctNode<T>> children;
    public Vector3 minCorner;
    public Vector3 maxCorner;
    public Vector3 center;
    public bool isLeaf;
    public List<T> data;

    public OctNode(Vector3 minC, Vector3 maxC) {
        minCorner = minC;
        maxCorner = maxC;
        Vector3 crosser = (maxCorner - minCorner);
        center = minCorner + (crosser.normalized * (crosser.magnitude / 2));
        isLeaf = false;
        data = new List<T>();
        if (crosser.magnitude < 1) {
            isLeaf = true;
        } else {
            children = new List<OctNode<T>>();
            children.Add(new OctNode<T>(minCorner, center));
            children.Add(new OctNode<T>(new Vector3(minCorner.x, minCorner.y, center.z), new Vector3(center.x, center.y, maxCorner.z)));
            children.Add(new OctNode<T>(new Vector3(minCorner.x, center.y, minCorner.z), new Vector3(center.x, maxCorner.y, center.z)));
            children.Add(new OctNode<T>(new Vector3(minCorner.x, center.y, center.z), new Vector3(center.x, maxCorner.y, maxCorner.z)));
            children.Add(new OctNode<T>(new Vector3(center.x, minCorner.y, minCorner.z), new Vector3(maxCorner.x, center.y, center.z)));
            children.Add(new OctNode<T>(new Vector3(center.x, minCorner.y, center.z), new Vector3(maxCorner.x, center.y, maxCorner.z)));
            children.Add(new OctNode<T>(new Vector3(center.x, center.y, minCorner.z), new Vector3(maxCorner.x, maxCorner.y, center.z)));
            children.Add(new OctNode<T>(center, maxCorner));
        }
    }

    public OctNode<T> navigate(Vector3 v) {
        if(v.x < center.x) {
            if(v.y < center.y) {
                if(v.z < center.z) {
                    return children[0];
                } else {
                    return children[1];
                }
            } else {
                if(v.z < center.z) {
                    return children[2];
                } else {
                    return children[3];
                }
            }
        } else {
            if (v.y < center.y) {
                if (v.z < center.z) {
                    return children[4];
                } else {
                    return children[5];
                }
            } else {
                if (v.z < center.z) {
                    return children[6];
                } else {
                    return children[7];
                }
            }
        }
    }
}
