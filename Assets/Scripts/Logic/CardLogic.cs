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

    public int ID
    {
        get{ return UniqueCardID; }
    }

    public bool CanBePlayed
    {
        get
        {
            bool ownersTurn = (TurnManager.Instance.whoseTurn == owner);
            bool fieldNotFull = true;
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
