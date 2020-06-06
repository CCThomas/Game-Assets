using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldManager : MonoBehaviour {
    public Direction direction;


    public Vector3 GetDesiredRotation(Vector3 position, Vector3 contactPoint) {
        Vector3 rotation = new Vector3(0, 0, 0);
        if (direction == Direction.X) {
            rotation.y = position.z < contactPoint.z ? 0 : 180;
        } else if (direction == Direction.XZ) {
            if (position.x < contactPoint.x) {
                if (position.z < contactPoint.z) {
                    rotation.y = 45;
                } else {
                    rotation.y = 90 + 45;
                }
            } else {
                if (position.z < contactPoint.z) {
                    rotation.y = 270 + 45;
                } else {
                    rotation.y = 180 + 45;
                }
            }
        } else if (direction == Direction.Z) {
            rotation.y = position.x < contactPoint.x ? 90 : 270;
        }
        return rotation;
    }

    public enum Direction {
        X, Z, XZ
    }
}
