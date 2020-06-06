using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Creature", menuName = "Creature")]
public class Creature : ScriptableObject {
    [Range(0.0f, 1.0f)]
    public float airControlPercent;
    public float height;
    public float speedClimb;
    public float speedFly;
    public float speedLeisure;
    public float speedQuick;
    public float speedSneak;
    public CreatureType type;
    public CreatureType currentForm;

    public GameObject birdPrefab;
    public GameObject humanPrefab;

    public enum CreatureType {
        Bird, Human
    }
}