using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform mainCameraTransform;
    void Start()
    {
        mainCameraTransform = Camera.main.transform;
    }

    // lateUpdate is called after the camera is rotated and the new rotation is to be setup
    private void LateUpdate()
    {
        transform.LookAt(
            transform.position + mainCameraTransform.rotation * Vector3.forward,
            mainCameraTransform.rotation * Vector3.up);
    } 
}
