using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class CardLogic: IIdentifiable
{
    public Player owner;
    public int UniqueCardID; 

    public CardAsset ca;
    public GameObject VisualRepresentation;

    public SpellEffect effect;

    public int ID
    {
        get{ return UniqueCardID; }
    }

    public bool CanBePlayed
    {
        get
        {
            bool ownersTurn = (TurnManager.Instance.whoseTurn == owner);
            // for spells the amount of characters on the field does not matter
            bool fieldNotFull = true;
            // but if this is a creature, we have to check if there is room on board (table)
            //Debug.Log("Card: " + ca.name + " has params: ownersTurn=" + ownersTurn + "fieldNotFull=" + fieldNotFull + " hasMana=" + (CurrentManaCost <= owner.ManaLeft));
            return ownersTurn && fieldNotFull;
        }
    }

    public CardLogic(CardAsset ca)
    {
        this.ca = ca;
        UniqueCardID = IDFactory.GetUniqueID();
        CardsCreatedThisGame.Add(UniqueCardID, this);
    }

    // STATIC (for managing IDs)
    public static Dictionary<int, CardLogic> CardsCreatedThisGame = new Dictionary<int, CardLogic>();

}
