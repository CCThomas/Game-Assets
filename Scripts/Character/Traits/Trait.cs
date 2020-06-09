[System.Serializable]
public class Trait
{
    public string name;
    public float value;
    public float defaulValue;

    public Trait(string name, float defaultValue)
    {
        this.name = name;
        this.value = this.defaulValue = defaultValue;
    }
}