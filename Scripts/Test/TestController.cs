using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour {

    // Camera
    public LayerMask firstPersonLayers = ~0;
    public LayerMask thirdPersonLayers = ~0;
    public float mouseSensitivity = 10f;

    // Bipedal Skeleton (Human, Elf, ect...)
    public GameObject bipedalGameObject;

    // Bird Skeleton
    public GameObject birdGameObject;

    // Managers
    public TestCreature creature;

    public TestCreature.CreatureType form;
    public TestMovement.MovementState state;

    // Managers
    TestManager manager;
    CameraManager cameraManager;

    // Player Controls
    public bool toggleCrouch, toggleRunning;
    PlayerInputActions controls;
    float horizontal, vertical;

    private void Awake() {
        controls = new PlayerInputActions();
        controls.PlayerControls.Crouch.performed += _ => SetCrouch(InputActionType.Performed);
        controls.PlayerControls.Crouch.canceled += _ => SetCrouch(InputActionType.Canceled);
        controls.PlayerControls.Jump.performed += _ => SetJump(InputActionType.Performed);
        controls.PlayerControls.Run.performed += _ => SetRun(InputActionType.Performed);
        controls.PlayerControls.Run.canceled += _ => SetRun(InputActionType.Canceled);
        controls.PlayerControls.SwitchCamera.performed += _ => SwitchCamera();
        controls.PlayerControls.Move.performed += context => SetMove(context.ReadValue<Vector2>());
        controls.PlayerControls.Look.performed += context => SetLook(context.ReadValue<Vector2>());
    }

    // Start is called before the first frame update
    void Start() {
        manager = new TestManager(creature, transform);
        manager.SetModel(bipedalGameObject, TestCreature.CreatureType.Human);
        manager.SetModel(birdGameObject, TestCreature.CreatureType.Bird);
        manager.Start(form, state);

        cameraManager = new CameraManager(transform, firstPersonLayers, thirdPersonLayers);
        Transform head = manager.GetHead();
        cameraManager.SetTarget(head);
    }

    // Update is called once per frame
    void FixedUpdate() {
        bool cameraTargetNeedsUpdate = manager.Update(horizontal, vertical, cameraManager.LookingDirection());
        if (cameraTargetNeedsUpdate) {
            cameraManager.SetTarget(manager.GetHead());
        }
    }

    void LateUpdate() {
        manager.UpdateAnimation();
        cameraManager.LateUpdate();
    }

    void SetCrouch(InputActionType type) {
        if (type == InputActionType.Performed && toggleCrouch) {
            manager.ToggleDown();
        } else if (type == InputActionType.Performed && !toggleCrouch) {
            manager.SetDown(true);
        } else if (type == InputActionType.Canceled && !toggleCrouch) {
            manager.SetDown(false);
        }
    }

    void SetJump(InputActionType type) {
        if (type == InputActionType.Performed) {
            manager.SetUp(true);
        }
    }
    void SetRun(InputActionType type) {
        if (type == InputActionType.Performed && toggleRunning) {
            manager.ToggleQuick();
        } else if (type == InputActionType.Performed && !toggleRunning) {
            manager.SetQuick(true);
        } else if (type == InputActionType.Canceled && !toggleRunning) {
            manager.SetQuick(false);
        }
    }

    void SetMove(Vector2 direction) {
        horizontal = direction.x;
        vertical = direction.y;
    }

    void SetLook(Vector2 lookingDirection) {
        float yaw = lookingDirection.x;
        float pitch = lookingDirection.y;
        cameraManager.Rotate(yaw, pitch);
    }

    void SwitchCamera() {
        cameraManager.SwitchCamera();
    }

    private void OnEnable() {
        controls.Enable();
    }

    private void OnDisable() {
        controls.Disable();
    }
    enum InputActionType {
        Canceled, Performed, Started
    }
}
