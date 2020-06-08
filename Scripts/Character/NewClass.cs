using System;
using UnityEngine;

public class Attack
{
    public string name;
    public int damage;
    // X: Radius left to right. If Zero or One straight line
    // Y: Radius Up and Down. If Zero or One Straight Line
    // Z: Range or Distance
    public Vector3 radius;

    public Attack()
    {
    }
}
