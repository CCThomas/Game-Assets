using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Inventory/Spell")]
public class Spell : Item {

    public Effect effect;
    public int levelOfDifficulty;
    public float range;
}
