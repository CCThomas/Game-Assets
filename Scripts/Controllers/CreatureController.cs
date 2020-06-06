using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using TMPro;

public class CreatureController : MonoBehaviour {
    //Player Character
    public Creature creature;
    public CreatureType type;

    // Camera
    public LayerMask firstPersonLayers = ~0;
    public LayerMask thirdPersonLayers = ~0;
    public CreatureCameraTarget cameraTarget = CreatureCameraTarget.Primary;

    // Controls
    public float mouseSensitivity = 10f;
    BirdTransitionSkill action1;

    // Managers
    CameraManager cameraManager;
    CreatureManager creatureManager;

    // Move
    PlayerInputActions controlls;

    private void Awake() {
        controlls = new PlayerInputActions();
        controlls.PlayerControls.SwitchCamera.performed += _ => SwitchCamera();
        controlls.PlayerControls.Move.performed += context => SetMove(context.ReadValue<Vector2>());
        controlls.PlayerControls.Look.performed += context => SetLook(context.ReadValue<Vector2>());
    }

    // Use this for initialization
    void Start() {
        Transform colliderGameObject = transform.GetChild((int) PlayerChild.Collider);
        Collider[] colliders = colliderGameObject.GetComponents<Collider>();
        creatureManager = new CreatureManager(colliders, creature, transform);

        cameraManager = new CameraManager(transform, firstPersonLayers, thirdPersonLayers);
        UpdateCameraTarget();
    }

    // Update is called once per frame
    void FixedUpdate() {
        float vertical = type == CreatureType.Bird ? 1 : forward;
        float horizontal = type == CreatureType.Bird ? 0 : right;
        CreatureCameraTarget currentCameraTarget = (CreatureCameraTarget) creatureManager.Update(vertical, horizontal, cameraManager.LookingDirection()); ;
        if (currentCameraTarget != cameraTarget) {
            cameraTarget = currentCameraTarget;
            //UpdateCameraTarget();
        }
    }

    void LateUpdate() {
        cameraManager.LateUpdate();
    }

    float forward, right;
    void SetMove(Vector2 direction) {
        forward = direction.y;
        right = direction.x;
    }

    void SetLook(Vector2 lookingDirection) {
        float yaw = lookingDirection.x;
        float pitch = lookingDirection.y;
        cameraManager.Rotate(yaw, pitch);
    }

    void SwitchCamera() {
        cameraManager.SwitchCamera();
    }

    void ToggleAction1() {
        action1.Action(creature);
    }

    void UpdateCameraTarget() {
        Transform cameraFocus = transform.GetChild((int) PlayerChild.Focus);
        cameraManager.SetTarget(cameraFocus.GetChild((int) cameraTarget));
    }

    private void OnEnable() {
        controlls.Enable();
    }

    private void OnDisable() {
        controlls.Disable();
    }
    private void OnTriggerEnter(Collider other) {
        Debug.Log("Warning");
    }

    enum PlayerChild {
        Collider = 1, Graphics = 0, Focus = 2
    }

    public enum CreatureCameraTarget {
        Primary
    }

    public enum CreatureType {
        Bird, Human
    }
}