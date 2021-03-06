using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum MonsterAttribute
{
    Fire,
    Water,
    Wind,
    Earth,
    Light,
    Dark
}

public enum MonsterType
{
    Dragon,
    Fiend,
    Spellcaster,
    Fairy,
    Machine,
    Warrior,
    Beast,
    Rock,
    Zombie,
    Winged_Beast,
    Insect,
    Beast_Warrior,
    Aqua
}

public enum SpellTrapEffects
{
    DestoryMonster,
    DestorySpellTrap,
    ChangeToAttack,
    ChangeToDefence,
    Heal,
    DirectAttack,
    Revive,
    Negate,
    Draw
}
public class CardAsset : ScriptableObject
{
    // this object will hold the info about the most general card
    [Header("General Info")]
    public string Name;
    public CharacterAsset characterAsset;  // if this is null, it`s a neutral card
    [TextArea(2,3)]
    public string Description;  // Description for spell or character
	public Sprite CardImage;
    public GameObject Model;
    public GameObject LoadEffect;
    public GameObject ShotAttack;
    public GameObject AttackEffect;
    public int xTransOffset;
    public int yTransOffset;
    public int zTransOffset;

    [Header("Monster Info")]
    public int Attack; // if -1 that is spell
    public int Defence;
    public MonsterAttribute Attribute;
    public int AttacksForOneTurn = 1;
    public MonsterType Type;
    public int Rank;
    private FieldPosition monsterState;
    public FieldPosition MonsterState
    {
        get {
            return monsterState; 
        }
        set {
            monsterState = value;
        }
    }

    [Header("Spell and Trap Info")]
    public SpellOrTrap SpellTrap;
    public SpellTrapEffects Effect;
    public int amount = 0;
    private SpellTrapPosition spellTrapState;
    public SpellTrapPosition SpellTrapState
    {
        get { return spellTrapState; }
        set { spellTrapState = value; }
    }

}
