using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour, ICharacter
{
    public int PlayerID;
    public CharacterAsset charAsset;
    public PlayerArea PArea;
    public SpellEffect PlayerPowerEffect;

    public Deck deck;
    public Hand hand;
    public Table table;

    //private int bonusManaThisTurn = 0;
    public bool usedPlayerPowerThisTurn = false;

    public int ID
    {
        get{ return PlayerID; }
    }

    public Player otherPlayer
    {
        get
        {
            if (Players[0] == this)
                return Players[1];
            else
                return Players[0];
        }
    }

    private int health;
    public int Health
    {
        get { return health;}
        set
        {
            health = value;
            if (value <= 0)
                Die(); 
        }
    }

    public delegate void VoidWithNoArguments();
    //public event VoidWithNoArguments MonsterPlayedEvent;
    //public event VoidWithNoArguments SpellPlayedEvent;
    //public event VoidWithNoArguments StartTurnEvent;
    public event VoidWithNoArguments EndTurnEvent;

    public static Player[] Players;

    void Awake()
    {
        Players = GameObject.FindObjectsOfType<Player>();
        PlayerID = IDFactory.GetUniqueID();
    }

    public virtual void OnTurnStart()
    {
        //Debug.Log("In ONTURNSTART for "+ gameObject.name);
        usedPlayerPowerThisTurn = false;
        PArea.HeroPower.WasUsedThisTurn = false;
    }

    public void AddAttackToAllMonstersOnTable()
    {
        foreach (MonsterLogic cl in table.MonstersOnTable)
            cl.OnTurnStart();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            DrawACard();
        }

    }


    public void OnTurnEnd()
    {
        if(EndTurnEvent != null)
            EndTurnEvent.Invoke();
        GetComponent<TurnMaker>().StopAllCoroutines();
    }

    public void DrawACard(bool fast = false)
    {
        if (deck.cards.Count > 0)
        {
            if (hand.CardsInHand.Count < PArea.handVisual.slots.Children.Length)
            {
                // 1) logic: add card to hand
                CardLogic newCard = new CardLogic(deck.cards[0]);
                newCard.owner = this;
                hand.CardsInHand.Add(newCard);
                // 2) logic: remove the card from the deck
                deck.cards.RemoveAt(0);
                // 3) create a command
                new DrawACardCommand(hand.CardsInHand[hand.CardsInHand.Count - 1], this, fast, fromDeck: true).AddToQueue(); 
            }
        }
        else
        {
            // there are no cards in the deck, take fatigue damage.
        }
       
    }

    public void PlayASpellFromHand(int SpellCardUniqueID, int TargetUniqueID)
    {
        // TODO: !!!
        // if TargetUnique ID < 0 , for example = -1, there is no target.
        if (TargetUniqueID < 0)
            PlayASpellFromHand(CardLogic.CardsCreatedThisGame[SpellCardUniqueID], null);
        else if (TargetUniqueID == ID)
        {
            PlayASpellFromHand(CardLogic.CardsCreatedThisGame[SpellCardUniqueID], this);
        }
        else if (TargetUniqueID == otherPlayer.ID)
        {
            PlayASpellFromHand(CardLogic.CardsCreatedThisGame[SpellCardUniqueID], this.otherPlayer);
        }
        else
        {
            // target is a monster
            PlayASpellFromHand(CardLogic.CardsCreatedThisGame[SpellCardUniqueID], MonsterLogic.MonstersCreatedThisGame[TargetUniqueID]);
        }
          
    }

    public void PlayASpellFromHand(CardLogic playedCard, ICharacter target)
    {
        // no matter what happens, move this card to PlayACardSpot
        new PlayASpellCardCommand(this, playedCard).AddToQueue();
        // remove this card from hand
        hand.CardsInHand.Remove(playedCard);
        // check if this is a monster or a spell
    }

    public void PlayAMonsterFromHand(int UniqueID, int tablePos)
    {
        PlayAMonsterFromHand(CardLogic.CardsCreatedThisGame[UniqueID], tablePos);
    }

    public void PlayAMonsterFromHand(CardLogic playedCard, int tablePos)
    {
        // create a new monster object and add it to Table
        MonsterLogic newMonster = new MonsterLogic(this, playedCard.ca);
        table.MonstersOnTable.Insert(tablePos, newMonster);
        // no matter what happens, move this card to PlayACardSpot
        new PlayAMonsterCommand(playedCard, this, tablePos, newMonster.UniqueMonsterID).AddToQueue();
        // remove this card from hand
        hand.CardsInHand.Remove(playedCard);
        HighlightPlayableCards();
    }

    public void Die()
    {
        // game over
        // block both players from taking new moves 
        PArea.ControlsON = false;
        otherPlayer.PArea.ControlsON = false;
        if (PlayerID == 0)
        {
            GlobalSettings.Instance.WhoWins.text = "Player 2 Wins";
        } else
        {
            GlobalSettings.Instance.WhoWins.text = "Player 1 Wins";
        }
        
        new GameOverCommand(this).AddToQueue();


    }

    // METHODS TO SHOW GLOW HIGHLIGHTS
    public void HighlightPlayableCards()
    {
        foreach (CardLogic cl in hand.CardsInHand)
        {
            GameObject g = IDHolder.GetGameObjectWithID(cl.UniqueCardID);
            if (g!=null)
                g.GetComponent<OneCardManager>().CanBePlayedNow = true;
        }

        foreach (MonsterLogic crl in table.MonstersOnTable)
        {
            GameObject g = IDHolder.GetGameObjectWithID(crl.UniqueMonsterID);
            if(g!= null)
            {
                g.GetComponent<OneMonsterManager>().CanAttackNow = (crl.AttacksLeftThisTurn > 0);
            }
                                
        }
    }

    // START GAME METHODS
    public void LoadCharacterInfoFromAsset()
    {
        Health = charAsset.MaxHealth;
        // change the visuals for portrait, hero power, etc...
        PArea.Portrait.charAsset = charAsset;
        PArea.Portrait.ApplyLookFromAsset();
        // TODO: insert the code to attach hero power script here. 
        /*
        if (charAsset.CharPowerName != null && charAsset.CharPowerName != "")
        {
            HeroPowerEffect = System.Activator.CreateInstance(System.Type.GetType(charAsset.CharPowerName)) as SpellEffect;
        }
        else
        {
            Debug.LogWarning("Check hero powr name for character " + charAsset.Name);
        }*/
    }

    public void TransmitInfoAboutPlayerToVisual()
    {
        PArea.Portrait.gameObject.AddComponent<IDHolder>().UniqueID = PlayerID;
        if (GetComponent<TurnMaker>() is AITurnMaker)
        {
            // turn off turn making for this character
            PArea.AllowedToControlThisPlayer = false;
        }
        else
        {
            // allow turn making for this character
            PArea.AllowedToControlThisPlayer = true;
        }
    }

    public void UsePlayerPower()
    {
        //ManaLeft -= 2;
        //usedHeroPowerThisTurn = true;
        PlayerPowerEffect.ActivateEffect();
    }
}
