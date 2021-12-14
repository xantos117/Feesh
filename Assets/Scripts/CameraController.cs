using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// taken from https://dev.to/matthewodle/simple-3d-camera-movement-1lcj
// credit to user Christopher Toman in the comments
public class CameraController : MonoBehaviour
{
    private float moveSpeed = 5f;
    private float scrollSpeed = 100f;

    void Update() {
        if(Input.GetMouseButtonDown(0)) {
            Resolution res = Screen.currentResolution;
            int centerx = res.width / 2;
            int centery = res.height / 2;
            Vector3 dist_from_center = Input.mousePosition;
            
        }
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
            transform.position += moveSpeed * new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) * Time.deltaTime;
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0) {
            transform.position += scrollSpeed * new Vector3(0, -Input.GetAxis("Mouse ScrollWheel"), 0) * Time.deltaTime;
        }
    }
}
