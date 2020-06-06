using UnityEngine;
using UnityEditor;
using System;

public abstract class GraphicsManager {
    public Animator animator;
    public Transform creatureGameObject;
    public CapsuleCollider collider;

    int originalHeight;

    public GraphicsManager(Transform creatureGameObject) {
        //Transform model = (Transform) AssetDatabase.LoadAssetAtPath("Assets/Graphics/Bird_009.blend", typeof(Transform)));
        //creatureGameObject
        //Instantiate(myPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        this.animator = creatureGameObject.GetComponent<Animator>();
        this.creatureGameObject = creatureGameObject;
    }

    public virtual Transform GetBone(string key) {
        return GetBone(key, ArmatureSide.NA);
    }

    public abstract Transform GetBone(string key, ArmatureSide side);

    public bool IsGrounded() {
        // TODO HERE This may not work
        return Physics.Raycast(creatureGameObject.position, -Vector3.up, 0.1f);
    }

    public Vector3 Forward() {
        return creatureGameObject.forward;
    }

    public Vector3 Right() {
        return creatureGameObject.right;
    }

    public Vector3 Up() {
        return creatureGameObject.up;
    }

    public void Rotate(Vector3 eulerAngles) {
        creatureGameObject.eulerAngles = eulerAngles;
    }
    public enum ArmatureSide {
        Left, Right, NA
    }

    internal void Move(Vector3 vector3) {
        creatureGameObject.Translate(vector3 * Time.deltaTime, Space.World);
    }

    internal void ResetCollider() {
        throw new NotImplementedException();
    }

    internal void ModifyCollider(float crouchHeightPercentage) {
        collider.height = 0;
    }
}

internal class BirdGraphics : GraphicsManager {
    public BirdGraphics(Transform player) : base(player) {
    }

    public override Transform GetBone(string key, ArmatureSide side) {
        return null;
    }
}

internal class HumanGraphics : GraphicsManager {
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

    public HumanGraphics(Transform player) : base(player) {
    }

    public override Transform GetBone(string key, ArmatureSide side) {
        string boneName = key;
        if (side != ArmatureSide.NA) {
            if (side == ArmatureSide.Left) {
                key = key.Replace(".SIDE", ".L");
            }
            if (side == ArmatureSide.Right) {
                key = key.Replace(".SIDE", ".R");
            }
        }
        return creatureGameObject.Find(key);
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