using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TestGraphics {

    protected Animator animator;
    public Rigidbody rigidbody;
    public Transform model;
    protected Transform currentHead;
    public float standingHeight;

    public TestGraphics(Rigidbody rigidbody, Transform model) {
        this.rigidbody = rigidbody;
        this.animator = model.GetComponent<Animator>();
        this.model = model;

        standingHeight = model.transform.lossyScale.y;
        currentHead = GetBone(TestBipedalGraphics.Head);
    }

    internal Transform GetBone(string name, ArmatureSide side) {
        if (side != ArmatureSide.NA) {
            if (side == ArmatureSide.Left) {
                name = name.Replace(".SIDE", ".L");
            }
            if (side == ArmatureSide.Right) {
                name = name.Replace(".SIDE", ".R");
            }
        }
        return model.Find(name);
    }

    internal void AddForce(Vector3 force) {
        Debug.Log("Apply Force=" + force);
        rigidbody.AddForce(force);
    }

    internal Transform GetBone(string bone) {
        return GetBone(bone, ArmatureSide.NA);
    }

    internal Transform GetBoneHead() {
        return currentHead;
    }

    public void SetClimbing(bool climbing) {
        animator.SetBool("climbing", climbing);
        if (!climbing) {
            SetHanging(false);
        }
    }

    public void SetCrouching(bool crouching) {
        Debug.Log("crouching=" + crouching);
        animator.SetBool("crouching", crouching);
    }

    public void SetQuick(bool crouching) {
        animator.SetBool("running", crouching);
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

    public enum ArmatureSide {
        Left, Right, NA
    }
}

class TestBipedalGraphics : TestGraphics {
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

    public TestBipedalGraphics(Rigidbody rigidbody, Transform model) : base(rigidbody, model) {

    }
}

class TestBirdGraphics : TestGraphics {
    public TestBirdGraphics(Rigidbody rigidbody, Transform model) : base(rigidbody, model) {

    }
}


