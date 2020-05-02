using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public CameraMode currentCameaMode;
    public bool inverted = false;
    public float mouseSensitivity;
    public Vector2 pitchMinMax = new Vector2(-40, 85);
    float yaw, pitch;

    // First Person

    // Third Person
    public Transform target;
    public float distanceFromTarget = 2f;
    public float rotationSmoothTime = .12f;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    // Start is called before the first frame update
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        //cameraTransform = Camera.main.transform;
        MouseSensitivityUpdate();
    }


    // Update is called once per frame
    void Update() {
        if (currentCameaMode == CameraMode.FirstPerson) {
        } else if (currentCameaMode == CameraMode.ThirdPerson) {
        }
    }

    // Update is called once per frame
    void LateUpdate() {
        if (currentCameaMode == CameraMode.FirstPerson) {
        } else if (currentCameaMode == CameraMode.ThirdPerson) {
            yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
            pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * (inverted ? -1 : 1);
            pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

            currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
            transform.eulerAngles = currentRotation;

            transform.position = target.position - transform.forward * distanceFromTarget;
        }
    }

    void MouseSensitivityUpdate() {
        if (currentCameaMode == CameraMode.FirstPerson) {
            mouseSensitivity = 100f;
        } else {
            mouseSensitivity = 10f;
        }
    }
}


public enum CameraMode {
    FirstPerson, ThirdPerson
}
