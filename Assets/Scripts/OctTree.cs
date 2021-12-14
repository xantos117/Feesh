using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctTree<T>
{
    public OctNode<T> root;
    public Vector3 minCorner;
    public Vector3 maxCorner;

    public OctTree(Vector3 minC, Vector3 maxC) {
        root = new OctNode<T>(minC, maxC);
        minCorner = minC;
        maxCorner = maxC;
    }
}
