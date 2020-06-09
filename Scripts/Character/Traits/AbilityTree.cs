using System.Collections.Generic;

public class CharacterTree
{
    public string name;
    public Block root;

    public CharacterTree(string name, Block root)
    {
        this.name = name;
        this.root = root;
    }

    // Ideas on other Character Trees not listed below.
    // Forgery - Weapon and Armor creation
    // Cooking - Cooking
    // Alchemy - Poitions

    // The Fitness character tree contains modifiers for basic traits like hit points, stealth, and climbing/swimming abilities
    public static CharacterTree Fitness()
    {
        CharacterTree character = new CharacterTree("Character",
            new TraitModifierBlock("10% Health Increase", 1,
            new TraitModifier("10% Health Increase", "hit_points", .1f, TraitModifierType.Trait)));
        return character;
    }

    // The Hunter character tree contains the ranged and stealth skills
    public static CharacterTree Hunter()
    {
        throw new System.NotImplementedException("Hunter Character Tree not implemented");
    }

    // The Magician character tree contains the magic based skills/spells
    public static CharacterTree Magician()
    {
        throw new System.NotImplementedException("Warrior Character Tree not implemented");
    }

    // The Warrior character tree contains the melee based skills
    public static CharacterTree Warrior()
    {
        throw new System.NotImplementedException("Warrior Character Tree not implemented");
    }

    // The Wild character tree contains nature base skills including animal transformation
    public static CharacterTree Wild()
    {
        throw new System.NotImplementedException("Wild Character Tree not implemented");
    }
}

public abstract class Block
{
    public string name;
    public int cost;
    public List<Block> children;
    public bool locked = true;
    public Block(string name, int cost)
    {
        this.name = name;
        this.cost = cost;
    }
}

public class AbilityBlock : Block
{
    public Ability ability;
    public AbilityBlock(string name, int cost, Ability ability) : base (name, cost)
    {
        this.ability = ability;
    }
}

public class TraitModifierBlock : Block
{
    public TraitModifier traitModifier;
    public TraitModifierBlock(string name, int cost, TraitModifier traitModifier) : base(name, cost)
    {
        this.traitModifier = traitModifier;
    }
}