using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

// this class will take care of switching turns and counting down time until the turn expires
public class TurnManager : MonoBehaviour {

    // for Singleton Pattern
    public static TurnManager Instance;

    private Player _whoseTurn;
    public Player whoseTurn
    {
        get
        {
            return _whoseTurn;
        }

        set
        {
            _whoseTurn = value;
            foreach (Transform t in value.PArea.tableVisual.MonstersSlots.Children)
            {
                t.GetComponentInChildren<StatesChanger>().panel.canChangeState = true;
            }
            foreach (Transform t in value.PArea.tableVisual.SpellsTrapsSlots.Children)
            {
                t.GetComponentInChildren<StatesChanger>().panel.canChangeState = true;
            }
            GlobalSettings.Instance.EnableEndTurnButtonOnStart(_whoseTurn);
            TurnMaker tm = whoseTurn.GetComponent<TurnMaker>();
            // player`s method OnTurnStart() will be called in tm.OnTurnStart();
            tm.OnTurnStart();
                
        }
    }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        OnGameStart();
    }

    public void OnGameStart()
    {
        //Debug.Log("In TurnManager.OnGameStart()");

        CardLogic.CardsCreatedThisGame.Clear();
        MonsterLogic.MonstersCreatedThisGame.Clear();
        SpellTrapLogic.SpellTrapsCreatedThisGame.Clear();
        // TODO: later add SpellLogic and TrapLogic

        foreach (Player p in Player.Players)
        {
            // TODO update each player's HP to 4000
            p.LoadCharacterInfoFromAsset();
            p.TransmitInfoAboutPlayerToVisual();
            p.PArea.PDeck.CardsInDeck = p.deck.cards.Count;
        }

        Sequence s = DOTween.Sequence();
        s.OnComplete(() =>
            {
                // determine who starts the game.
                // TODO: flipping coin of choosing which player starts
                int rnd = Random.Range(0,2);  // 2 is exclusive boundary
                //Debug.Log(Player.Players.Length);
                //Debug.Log("Who is first - " + rnd);
                Player whoGoesFirst = Player.Players[rnd];
                // Debug.Log(whoGoesFirst);
                Player whoGoesSecond = whoGoesFirst.otherPlayer;
                // Debug.Log(whoGoesSecond);
         
                // draw 4 cards for first player and 5 for second player
                int initDraw = 4;
                for (int i = 0; i < initDraw; i++)
                {            
                    // second player draws a card
                    whoGoesSecond.DrawACard(true);
                    // first player draws a card
                    whoGoesFirst.DrawACard(true);
                }
                new StartATurnCommand(whoGoesFirst).AddToQueue();
            });
    }

    public void EndTurn()
    {
        // send all commands in the end of current player`s turn
        whoseTurn.OnTurnEnd();

        new StartATurnCommand(whoseTurn.otherPlayer).AddToQueue();
    }

}

