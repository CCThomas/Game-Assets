using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Test Creature", menuName = "Test Creature")]
public class TestCreature : ScriptableObject {
    [Range(0.0f, 1.0f)]
    public float airControlPercent;
    public float height;
    public float speedClimb;
    public float speedCrouch;
    public float speedFly;
    public float speedLeisure;
    public float speedQuick;
    public float speedSneak;
    public CreatureType type;
    public CreatureType currentForm;
    public CreatureType intendedForm;

    public bool FormChanged() {
        return currentForm != intendedForm;
    }

    public enum CreatureType {
        Bird, Human
    }
}