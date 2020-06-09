[System.Serializable]
public class Ability : Trait
{
    public string traitKey;
    public int level;

    public Ability(string name, string traitKey) : base(name, 1)
    {
        this.traitKey = traitKey;
        level = 0;
    }

    public Ability(string name, string traitKey, float defaulValue) : base(name, defaulValue)
    {
        this.traitKey = traitKey;
        level = 0;
    }
}