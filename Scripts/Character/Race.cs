using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Race", menuName = "Character/Race")]
public class Race : ScriptableObject
{
    public List<TraitModifier> modifiers;
}