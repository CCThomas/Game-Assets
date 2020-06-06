using UnityEngine;
using UnityEditor;

internal class HumanoidClimbManager : MovementManager {
    // Key Codes
    public KeyCode keyCodeDrop = KeyCode.LeftControl;
    public KeyCode keyCodeJump = KeyCode.Space;

    // Hold
    RaycastHit holdRaycastHit;

    // Trackers
    bool hanging;
    float velocitySpeedSmooth, VelocityTurnSmooth;

    // Modifiers
    float gravity = -12;
    float jumpHeight = .8f;
    float speedSmoothTime = 0.1f;
    float turnSmoothTime = 0.1f;
    float airControlPercent = 0.3f;
    float hangingHeightPercentage = 1.235f;
    float climbHiehgtPercentage = 0.647f;

    public HumanoidClimbManager(CharacterManager characterManager, GraphicsManager graphics, RaycastHit holdRaycastHit) : base(characterManager, graphics) {
        this.holdRaycastHit = holdRaycastHit;

        SetState(MovementState.Climb);
        ((HumanoidGraphicsManager) graphics).SetClimbing(true);
        ((HumanoidGraphicsManager) graphics).SetHanging(false);
        character.ModifyController(1, hanging ? hangingHeightPercentage : climbHiehgtPercentage);
        character.SetRadius(0);
    }
    public override void CleanUp() {
        ((HumanoidGraphicsManager) graphics).SetClimbing(false);
        ((HumanoidGraphicsManager) graphics).SetHanging(false);
    }

    public override int Update(float horizontal, float vertical, Vector3 lookingDirection) {
        //HoldScript.Direction direction = holdTransform.GetComponent<HoldScript>().direction;
        HoldManager holdManager = holdRaycastHit.transform.GetComponent<HoldManager>();
        Vector3 desiredRotation = holdManager.GetDesiredRotation(character.GetPosition(), holdRaycastHit.point);

        // Looking Direction?
        float targetRotation = Mathf.Atan2(desiredRotation.z, desiredRotation.x) * Mathf.Rad2Deg + desiredRotation.y;
        graphics.Rotate(Vector3.up * Mathf.SmoothDampAngle(graphics.creatureGameObject.eulerAngles.y, targetRotation, ref VelocityTurnSmooth, GetModifiedSmoothTime(turnSmoothTime)));

        // Move to point
        Transform leftHand = graphics.GetBone(HumanoidGraphicsManager.Hand, GraphicsManager.ArmatureSide.Left);
        Transform rightHand = graphics.GetBone(HumanoidGraphicsManager.Hand, GraphicsManager.ArmatureSide.Right);
        Vector3 centerHands = Vector3.Lerp(leftHand.position, rightHand.position, .5f);
        if (centerHands != holdRaycastHit.point) {
            //Raycast(leftHand.position, holdRaycastHit.point, Vector3.Distance(leftHand.position, holdRaycastHit.point), ~0);
            //leftHand.position = holdRaycastHit.point; // Vector3.Lerp(leftHand.position, holdRaycastHit.point, .1f);
            //float distance = Vector3.Distance(centerHands, character.GetPosition());
            character.Lerp(holdRaycastHit.point, .03f);
        }

        CheckHanging();

        // TODO Actually Move


        return GetReturnInt();
    }

    public override int UpdateAnimation() {
        // Do Nothing
        return 0;
    }

    public override void SetDown(bool down) {
        this.down = down;
        if (this.down) {
            intendedState = MovementState.Ground;
        }
    }

    public override void SetUp(bool up) {
        this.up = up;
        if (this.up) {
            intendedState = MovementState.Ground;
        }
    }

    bool CanMove() {
        return false;
    }

    void CheckHanging() {
        Transform chest = graphics.GetBone(HumanoidGraphicsManager.Chest);
        Vector3 origin = chest.position - chest.forward * 1;
        Vector3 direction = chest.forward;
        RaycastHit hit = Raycast(origin, direction, character.GetHeight() / 2, ~0, Color.blue);
        bool shouldHang = hit.collider != null;
        if (shouldHang != hanging) {
            hanging = shouldHang;
            ((HumanoidGraphicsManager) graphics).SetHanging(hanging);
        }
    }

    float GetModifiedMoveSpeed(float input, float current, float target) {
        if (input == 0) {
            return 0;
        }
        float targetSpeed = input > 0 ? target : target * -1;
        return Mathf.SmoothDamp(current, targetSpeed, ref velocitySpeedSmooth, GetModifiedSmoothTime(speedSmoothTime));
    }

    float GetModifiedSmoothTime(float smoothTime) {
        if (character.isGrounded()) {
            return smoothTime;
        }

        if (airControlPercent == 0) {
            return float.MaxValue;
        }
        return smoothTime / airControlPercent;
    }

    int GetReturnInt() {
        return hanging ? (int) HumanoidController.CameraTarget.Hanging : (int) HumanoidController.CameraTarget.Climbing;
    }
}