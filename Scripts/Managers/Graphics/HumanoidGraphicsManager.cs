using UnityEngine;
using UnityEditor;
public class HumanoidGraphicsManager : GraphicsManager {
    public static readonly string Hip = "Armature/Hip";
    public static readonly string Stomach = Hip + "/Stomach";
    public static readonly string Chest = Stomach + "/Chest";
    public static readonly string Neck = Chest + "/Neck";
    public static readonly string Head = Neck + "/Head";
    public static readonly string Shoulder = Chest + "/Shoulder.SIDE";
    public static readonly string Bicep = Shoulder + "/Bicep.SIDE";
    public static readonly string Forearm = Bicep + "/Forearm.SIDE";
    public static readonly string Hand = Forearm + "/Hand.SIDE";
    public static readonly string FingerUpper = Hand + "/FingerUpper.SIDE";
    public static readonly string FingerLower = FingerUpper + "/FingerLower.SIDE";
    public static readonly string ThumbUpper = Hand + "/ThumbUpper.SIDE";
    public static readonly string ThumbLower = ThumbUpper + "/ThumbLower.SIDE";
    public static readonly string Tigh = Hip + "/Tigh.SIDE";
    public static readonly string Shin = Tigh + "/Shin.SIDE";
    public static readonly string Foot = Shin + "/Foot.SIDE";
    public static readonly string Toe = Foot + "/Toe.SIDE";


    public HumanoidGraphicsManager(Transform graphics) : base(graphics) { }

    public override Transform GetBone(string name, ArmatureSide side) {
        string boneName = name;
        if (side != ArmatureSide.NA) {
            if (side == ArmatureSide.Left) {
                name = name.Replace(".SIDE", ".L");
            }
            if (side == ArmatureSide.Right) {
                name = name.Replace(".SIDE", ".R");
            }
        }
        return creatureGameObject.Find(name);
    }

    public void SetClimbing(bool climbing) {
        animator.SetBool("climbing", climbing);
        if (!climbing) {
            SetHanging(false);
        }
    }
    public void SetCrouching(bool crouching) {
        animator.SetBool("crouching", crouching);
    }

    public void SetHanging(bool hanging) {
        animator.SetBool("hanging", hanging);
    }

    public void SetSliding(bool sliding) {
        animator.SetBool("sliding", sliding);
    }

    public void SetSpeedForward(float speedForward, float dampTime) {
        animator.SetFloat("speedForward", speedForward, dampTime, Time.deltaTime);
    }

    public void SetSpeedRight(float speedRight, float dampTime) {
        animator.SetFloat("speedRight", speedRight, dampTime, Time.deltaTime);
    }

    public void SetSpeedUp(float speedUp, float dampTime) {
        animator.SetFloat("speedUp", speedUp, dampTime, Time.deltaTime);
    }
}