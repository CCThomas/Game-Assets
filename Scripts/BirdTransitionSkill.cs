using UnityEngine;
using UnityEditor;
using System;

public class BirdTransitionSkill {

    bool active;

    public void Action(Creature creature) {
        active = !active;
        creature.currentForm = active ? Creature.CreatureType.Bird : creature.type;
    }
}