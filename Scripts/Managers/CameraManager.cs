using UnityEngine;
using UnityEditor;
using System;

public class CameraManager {

    // Needed From Parent
    Camera myCamera;
    Transform transform;
    Transform target;
    public LayerMask firstPersonLayers;
    public LayerMask thirdPersonLayers;

    // Properties
    public float distanceFromTarget = 2f;
    public float rotationSmoothTime = .12f;
    public Vector2 pitchMinMax = new Vector2(-40, 85);

    // Trackers
    bool inverted = false;
    float yaw, pitch;
    float mouseSensitivity = 10f;
    CameraMode currentCameaMode;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    public CameraManager(Transform parent, LayerMask firstPersonLayers, LayerMask thirdPersonLayers) {
        GameObject cameraGameObject = new GameObject("Camera for " + parent.name);
        myCamera = cameraGameObject.AddComponent<Camera>();
        cameraGameObject.transform.parent = parent;
        transform = cameraGameObject.transform;
        this.firstPersonLayers = firstPersonLayers;
        this.thirdPersonLayers = thirdPersonLayers;

        // Lock Cursor to Game
        Cursor.lockState = CursorLockMode.Locked;

        UpdateCamera();
    }

    public void Rotate(float yawModifier, float pitchModifier) {
        yaw += yawModifier * mouseSensitivity * Time.deltaTime;
        pitch -= pitchModifier * mouseSensitivity * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);
    }

    public void LateUpdate() {
        // Rotate Camera
        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
        transform.eulerAngles = currentRotation;

        // Move Camera to target location
        if (currentCameaMode == CameraMode.FirstPerson) {
            transform.position = target.position;
        } else if (currentCameaMode == CameraMode.ThirdPerson) {
            transform.position = target.position - transform.forward * distanceFromTarget;
        }
    }

    public void SwitchCamera() {
        if (currentCameaMode == CameraMode.ThirdPerson) {
            currentCameaMode = CameraMode.FirstPerson;
        } else {
            currentCameaMode = CameraMode.ThirdPerson;
        }
        UpdateCamera();
    }

    internal Vector3 LookingDirection() {
        return transform.forward;
    }

    void UpdateCamera() {
        if (currentCameaMode == CameraMode.FirstPerson) {
            myCamera.cullingMask = firstPersonLayers;

        } else {
            myCamera.cullingMask = thirdPersonLayers;
        }
    }

    public void SetTarget(Transform target) {
        this.target = target;
    }

    public enum CameraMode {
        FirstPerson, ThirdPerson
    }
}