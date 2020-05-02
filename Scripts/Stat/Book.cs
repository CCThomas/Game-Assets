using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "New Book", menuName = "Inventory/Book")]
public class Book : Item {
    public int skillExperienceToGain;
    public Skill[] skillsToImprove;
    public Spell[] spellsToLearn;

}