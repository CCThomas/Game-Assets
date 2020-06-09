[System.Serializable]
public class TraitModifier
{
    public string name;
    public string key;
    public float modifier;
    public TraitModifierType type;

    public TraitModifier(string name, string key, float modifier, TraitModifierType type)
    {
        this.name = name;
        this.key = key;
        this.modifier = modifier;
        this.type = type;
    }
}

public enum TraitModifierType
{
    Trait, Ability
}

