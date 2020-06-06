using UnityEngine;
using UnityEditor;

public class HumanoidManager {
    CharacterManager character;
    HumanoidGraphicsManager graphics;
    MovementManager movement;

    public HumanoidManager(CharacterManager characterManager, Transform graphicsTransform) {
        this.character = characterManager;
        graphics = new HumanoidGraphicsManager(graphicsTransform);
        UpdateMovementManager();
    }

    public int Update(float horizontal, float vertical, Vector3 lookingDirection) {
        if (movement.StateChanged()) {
            UpdateMovementManager();
        }
        return movement.Update(horizontal, vertical, lookingDirection);
    }

    public int LateUpdate() {
        return movement.UpdateAnimation();
    }


    public virtual void SetDown(bool down) {
        movement.SetDown(down);
    }

    public virtual void SetQuick(bool quick) {
        movement.SetQuick(quick);
    }

    public virtual void SetUp(bool up) {
        movement.SetUp(up);
    }

    public virtual void ToggleDown() {
        movement.ToggleDown();
    }

    public virtual void ToggleQuick() {
        movement.ToggleQuick();
    }

    public enum CameraTarget {
        Standing, Crouching
    }

    void UpdateMovementManager() {
        if (movement == null) {
            movement = new HumanoidGroundManager(character, graphics);
            return;
        }

        movement.CleanUp();
        if (movement.intendedState == MovementManager.MovementState.Ground) {
            movement = new HumanoidGroundManager(character, graphics);
        } else if (movement.intendedState == MovementManager.MovementState.Climb) {
            movement = new HumanoidClimbManager(character, graphics, ((HumanoidGroundManager) movement).holdRaycastHit);
        }
    }

}