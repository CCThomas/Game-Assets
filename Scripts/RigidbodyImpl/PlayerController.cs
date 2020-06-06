using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    // Object
    public string startGraphicKey;
    public List<Graphic> graphics;
    public MyCreature creature;
    MyCreatureController controller;
    Dictionary<string, Transform> graphicsDict;

    // Camera
    PlayerCamera playerCamera;
    public LayerMask firstPersonLayers;
    public LayerMask thirdPersonLayers;
    public float mouseSensitivity = 10f;

    // Controls
    PlayerInputActions controls;
    new Rigidbody rigidbody;
    float horizontal, vertical;
    public bool toggleCrouching, toggleRunning;

    private void Awake() {
        controls = new PlayerInputActions();
        controls.PlayerControls.Crouch.performed += _ => SetCrouch(InputActionType.Performed);
        controls.PlayerControls.Crouch.canceled += _ => SetCrouch(InputActionType.Canceled);
        controls.PlayerControls.Jump.performed += _ => SetJump(InputActionType.Performed);
        controls.PlayerControls.Run.performed += _ => SetRun(InputActionType.Performed);
        controls.PlayerControls.Run.canceled += _ => SetRun(InputActionType.Canceled);
        controls.PlayerControls.SwitchCamera.performed += _ => SwitchCamera();
        controls.PlayerControls.Move.performed += context => SetMove(context.ReadValue<Vector2>());
        controls.PlayerControls.Move.canceled += context => SetMove(new Vector2(0, 0));
        controls.PlayerControls.Look.performed += context => SetLook(context.ReadValue<Vector2>());
        controls.PlayerControls.Look.canceled += context => SetLook(new Vector2(0, 0));
        controls.PlayerControls.CameraDistance.performed += context => SetScroll(context.ReadValue<Vector2>());
        controls.PlayerControls.SwitchModel.performed += _ => SwitchModel();
    }

    // Use this for initialization
    void Start() {
        creature.Initialize();
        graphicsDict = Graphic.ToDictionary(graphics);

        controller = new MyCreatureController(transform, creature);
        controller.UpdateGraphics(MyCreatureController.ModelType.Humanoid, graphicsDict[startGraphicKey]);

        playerCamera = new PlayerCamera(transform, firstPersonLayers, thirdPersonLayers);
        playerCamera.SetTarget(controller.GetHead());
    }

    // Update is called once per frame
    void LateUpdate() {
        playerCamera.LateUpdate();
        controller.UpdateAnimation();
    }

    void FixedUpdate() {
        controller.UpdateMovement(horizontal, vertical, playerCamera.LookingDirection());
    }

    Transform GetGraphic(string key) {
        return graphicsDict[key];
    }

    void SetCrouch(InputActionType type) {
        bool desired = false;
        if (type == InputActionType.Performed && toggleCrouching) {
            desired = !controller.crouch;
        } else if (type == InputActionType.Performed && !toggleCrouching) {
            desired = true;
        } else if (type == InputActionType.Canceled && !toggleCrouching) {
            desired = false;
        } else {
            return;
        }
        controller.AttemptSetCrouch(desired);
    }

    void SetJump(InputActionType type) {
        controller.AttemptSetJump(true);
    }

    void SetLook(Vector2 direction) {
        float yaw = direction.x * mouseSensitivity * Time.deltaTime;
        float pitch = direction.y * mouseSensitivity * Time.deltaTime;
        playerCamera.Rotate(yaw, pitch);
    }

    void SetMove(Vector2 direction) {
        horizontal = direction.x;
        vertical = direction.y;
    }

    void SetRun(InputActionType type) {
        bool desired = false;
        if (type == InputActionType.Performed && toggleRunning) {
            desired = !controller.run;
        } else if (type == InputActionType.Performed && !toggleRunning) {
            desired = true;
        } else if (type == InputActionType.Canceled && !toggleRunning) {
            desired = false;
        } else {
            return;
        }
        controller.AttemptSetRun(desired);
    }

    void SetScroll(Vector2 scroll) {
        playerCamera.AdjustDistance(scroll.normalized);
    }

    void SwitchCamera() {
        playerCamera.SwitchCamera();
    }

    void SwitchModel() {
        if (controller.modelType == MyCreatureController.ModelType.Bird) {
            controller.UpdateGraphics(MyCreatureController.ModelType.Humanoid, GetGraphic("bipedalnverse")); ;
        } else if (controller.modelType == MyCreatureController.ModelType.Humanoid) {
            controller.UpdateGraphics(MyCreatureController.ModelType.Bird, GetGraphic("bird"));
        }
        playerCamera.SetTarget(controller.GetHead());
    }

    void OnCollisionStay() {
        controller.OnCollisionStay();
    }

    private void OnEnable() {
        controls.Enable();
    }

    private void OnDisable() {
        controls.Disable();
    }

    private enum InputActionType {
        Canceled, Performed
    }
}

[System.Serializable]
public class Graphic {
    public string key;
    public Transform model;

    public static Dictionary<string, Transform> ToDictionary(List<Graphic> graphics) {
        Dictionary<string, Transform> dict = new Dictionary<string, Transform>();
        foreach (Graphic graphic in graphics) {
            graphic.model.gameObject.SetActive(false);
            dict.Add(graphic.key, graphic.model);
        }
        return dict;
    }
}