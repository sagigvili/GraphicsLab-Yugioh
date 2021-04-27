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
    Machine
}
public class CardAsset : ScriptableObject
{
    // this object will hold the info about the most general card
    [Header("General Info")]
    public CharacterAsset characterAsset;  // if this is null, it`s a neutral card
    [TextArea(2,3)]
    public string Description;  // Description for spell or character
	public Sprite CardImage;

    [Header("Monster Info")]
    public int Attack;
    public int Defence;
    public MonsterAttribute Attribue;
    public MonsterType Type;
    public int rank;

}
