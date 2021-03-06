using DG.Tweening;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]

public enum SpellTrapPosition
{
    FaceUp,
    Set
}

public enum SpellOrTrap
{
    Spell,
    Trap
}

public class SpellTrapLogic : ICharacter
{
    // PUBLIC FIELDS
    public Player owner;
    public CardAsset ca;
    public int UniqueSpellTrapID;
    public SpellOrTrap Type;
    public int ID
    {
        get { return UniqueSpellTrapID; }
    }
    public int amount = 0;
    public SpellTrapEffects Effect;

    public bool CanUse
    {
        get
        {
            bool ownersTurn = (TurnManager.Instance.whoseTurn == owner);
            return (ownersTurn && (ca.SpellTrapState == SpellTrapPosition.FaceUp));
        }
    }
    public int Health { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    // CONSTRUCTOR
    public SpellTrapLogic(Player owner, CardAsset ca)
    {
        this.ca = ca;
        this.owner = owner;
        Type = ca.SpellTrap;
        amount = ca.amount;
        Effect = ca.Effect;
        UniqueSpellTrapID = IDFactory.GetUniqueID();
        SpellTrapsCreatedThisGame.Add(UniqueSpellTrapID, this);
    }
    public void Die()
    {
        owner.table.SpellsTrapsOnTable.Remove(this);
        new SpellTrapDieCommand(UniqueSpellTrapID, owner).AddToQueue();
    }



    // STATIC For managing IDs
    public static Dictionary<int, SpellTrapLogic> SpellTrapsCreatedThisGame = new Dictionary<int, SpellTrapLogic>();

}
