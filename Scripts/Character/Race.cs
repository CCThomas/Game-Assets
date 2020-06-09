using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Race", menuName = "Character/Race")]
public class Race : ScriptableObject {
    public List<TraitModifier> requiredModifiers = new List<TraitModifier>() {
        new TraitModifier("Race Height", "height", 1, TraitModifierType.Trait),
        new TraitModifier("Race Hit Points", "hit_points", 1, TraitModifierType.Trait),
        new TraitModifier("Race Race", "speed", 1, TraitModifierType.Trait),
    };

    public List<TraitModifier> additionalModifiers = new List<TraitModifier>() {
    };


    // Abilities - Drawf
    // Darkvision?
    // Resilience ~ Posion
    // 20% Extra Damange with Axes & hammers

    // Elf
}