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
    Transform thirdPersonTarget;
    public float distanceFromTarget = 2f;
    public float rotationSmoothTime = .12f;

    // Thid person maybe first
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    // Start is called before the first frame update
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        MouseSensitivityUpdate();
    }


    // Update is called once per frame
    void Update() {
         if (Input.GetKeyDown(KeyCode.F)) {
            if (currentCameaMode == CameraMode.ThirdPerson) {
                currentCameaMode = CameraMode.FirstPerson;
                transform.position = thirdPersonTarget.position;
            } else {
                currentCameaMode = CameraMode.ThirdPerson;
            }
            MouseSensitivityUpdate();
        }
    }

    // Update is called once per frame
    void LateUpdate() {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity * (inverted ? -1 : 1);
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);

        transform.eulerAngles = currentRotation;

        if (currentCameaMode == CameraMode.FirstPerson) {
            transform.position = thirdPersonTarget.position;
        } else if (currentCameaMode == CameraMode.ThirdPerson) {
            transform.position = thirdPersonTarget.position - transform.forward * distanceFromTarget;
        }
    }

    void MouseSensitivityUpdate() {
        if (currentCameaMode == CameraMode.FirstPerson) {
            mouseSensitivity = 10f;
        } else {
            mouseSensitivity = 10f;
        }
    }
    public void SetThirdPersonTarget(Transform thirdPersonTarget) {
        this.thirdPersonTarget = thirdPersonTarget;
    }
}


public enum CameraMode {
    FirstPerson, ThirdPerson
}
