using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item {

    public EquipmentSlot equipmentSlot;
    public Effect[] effects;
}

public enum EquipmentSlot {
    Head, //
    Chest, //
    ShoulderLeft, ShoulderRight, //
    ArmLeft, ArmRight, //
    HandLeft, HandRight, //
    Waist, //
    LegLeft, LegRight, //
    FootLeft, FootRight
}
