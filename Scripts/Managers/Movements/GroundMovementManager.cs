using UnityEngine;
using UnityEditor;

internal class GroundMovementManager : MovementManagerV1 {
    // Trackers
    bool crouching, running, sliding; // TODO Current Crouch for after slide bug. Duplicate: Toggle Crouch off. Run, Crouch to slide. Release mid slide.
    float velocitySpeedSmooth, VelocityTurnSmooth;
    public RaycastHit holdRaycastHit;
    Vector3 distanceStart;

    // Modifiers
    float gravity = -12;
    float jumpHeight = .8f;
    float crouchHeightPercentage = .7f; // 70%
    float slideDistance = 2;
    float speedSmoothTime = 0.1f;
    float turnSmoothTime = 0.1f;
    float airControlPercent = 0.3f;

    public GroundMovementManager(Collider[] colliders, Creature creature, GraphicsManager graphics) : base(colliders, creature, graphics) {
        SetState(MovementState.Ground);
    }

    public override void CleanUp() {
        ((HumanGraphics) graphics).SetCrouching(false);
    }

    public override int Update(float horizontal, float vertical, Vector3 lookingDirection) {
        throw new System.NotImplementedException();
    }

    public override int UpdateAnimation() {
        throw new System.NotImplementedException();
    }

    bool AttemptSetCrouch(bool attemptCrouching) {
        if (!attemptCrouching && CanStandUp()) {
            this.crouching = attemptCrouching;
            graphics.ResetCollider();
            ((HumanGraphics) graphics).SetCrouching(this.crouching);
        } else if (attemptCrouching) {
            this.crouching = attemptCrouching;
            graphics.ModifyCollider(crouchHeightPercentage);
            ((HumanGraphics) graphics).SetCrouching(this.crouching);
            if (running) {
                AttemptSetSliding(true);
            }
        }

        return crouching == attemptCrouching;
    }

    bool AttemptSetRunning(bool attemptSetRunning) {
        if (attemptSetRunning && crouching) {
            // Running will be determined by if the character can stand up
            running = AttemptSetCrouch(false);
        } else {
            running = attemptSetRunning;
        }

        return running == attemptSetRunning;
    }

    bool AttemptSetSliding(bool attempSliding) {
        sliding = attempSliding;
        ((HumanGraphics) graphics).SetSliding(attempSliding);
        if (sliding) {
            AttemptSetRunning(false);
            distanceStart = graphics.creatureGameObject.position;
        }
        return true;
    }

    /**
     * Can Stand up takes the position of the Character Controller, and checks if the character controller would collide with something if was extended to full height.
     * <p>
     * The Y Coord is decided by placing it at the center of the Character Controller. That way it will not take feet collision into account. 
     * (There was a bug where placing the Y Coord at Zero would have the RayCast always Collide with something)
     */
    bool CanStandUp() {
        Vector3 origin = graphics.creatureGameObject.position;
        Vector3 direction = Vector3.up.normalized;
        RaycastHit hit = Raycast(origin, direction, creature.height, ~0, Color.blue);
        return hit.collider == null;
    }

    float GetModifiedMoveSpeed(float input, float current, float target) {
        if (input == 0) {
            return 0;
        }
        float targetSpeed = input > 0 ? target : target * -1;
        return Mathf.SmoothDamp(current, targetSpeed, ref velocitySpeedSmooth, GetModifiedSmoothTime(speedSmoothTime));
    }

    float GetModifiedSmoothTime(float smoothTime) {
        if (graphics.IsGrounded()) {
            return smoothTime;
        }

        if (airControlPercent == 0) {
            return float.MaxValue;
        }
        return smoothTime / airControlPercent;
    }
    int GetReturnInt() {
        return crouching ? (int) HumanoidController.CameraTarget.Crouching : (int) HumanoidController.CameraTarget.Standing;
    }
}