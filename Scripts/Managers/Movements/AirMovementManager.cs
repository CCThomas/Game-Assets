using UnityEngine;
using UnityEditor;

internal class AirMovementManager : MovementManagerV1 {

    float mouseSensitivity = 10f;
    float rotationSmoothTime = 0; //.12f;
    float yaw, pitch;
    float velocitySpeedSmooth, VelocityTurnSmooth;
    float speedSmoothTime = 0.1f;

    Vector3 currentRotation;
    Vector3 rotationSmoothVelocity;

    public AirMovementManager(Collider[] colliders, Creature creature, GraphicsManager graphics) : base(colliders, creature, graphics) {
        SetState(MovementState.Air);
    }

    public override void CleanUp() {
        // Reset Animations
    }

    public override int Update(float vertical, float horizontal, Vector3 lookingDirection) {
        //Debug.Log("horizontal=" + horizontal + ", vertical=" + vertical);
        yaw += lookingDirection.x * mouseSensitivity * Time.deltaTime;
        pitch -= lookingDirection.y * mouseSensitivity * Time.deltaTime;

        // Rotate Player
        currentRotation = Vector3.SmoothDamp(currentRotation, new Vector3(pitch, yaw), ref rotationSmoothVelocity, rotationSmoothTime);
        graphics.creatureGameObject.forward = lookingDirection;
        //graphics.Rotate(new Vector3(pitch, yaw));

        float targetSpeed = 3; // creature.speedLeisure;
        movement.forward = GetModifiedMoveSpeed(vertical, movement.forward, targetSpeed);
        movement.right = GetModifiedMoveSpeed(horizontal, movement.right, targetSpeed);
        //movement.up += Time.deltaTime;
        Vector3 velocity = graphics.Forward() * movement.forward + Vector3.up * movement.up + graphics.Right() * movement.right;

        // TODO Collider Detection
        graphics.Move(velocity);

        return 0;
    }

    public override int UpdateAnimation() {
        // TODO
        return 0;
    }

    float GetModifiedMoveSpeed(float input, float current, float target) {
        if (input == 0) {
            return 0;
        }
        float targetSpeed = input > 0 ? target : target * -1;
        return Mathf.SmoothDamp(current, targetSpeed, ref velocitySpeedSmooth, GetModifiedSmoothTime(speedSmoothTime));
    }

    float GetModifiedSmoothTime(float smoothTime) {
        return smoothTime;
    }
}