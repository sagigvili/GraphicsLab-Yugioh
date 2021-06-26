 using UnityEngine;
using System.Collections;
using DG.Tweening;


// this class should be attached to the deck
// generates new cards and places them into the hand
public class PlayerDeckVisual : MonoBehaviour {

    public AreaPosition owner;
    public float HeightOfOneCard = 670;
    public GameObject Cube;

    void Start()
    {
        CardsInDeck = GlobalSettings.Instance.Players[owner].deck.cards.Count;
    }

    private int cardsInDeck = 0;
    public int CardsInDeck
    {
        get{ return cardsInDeck; }

        set
        {
            cardsInDeck = value;
            if (value == 3)
                Cube.transform.position = new Vector3(0, 0, -8040);
            else
            {
                if (owner == AreaPosition.Top)
                {
                    Cube.transform.localScale = new Vector3(Cube.transform.localScale.x - HeightOfOneCard, Cube.transform.localScale.y - 5, Cube.transform.localScale.z - HeightOfOneCard);
                    Cube.transform.position = new Vector3(Cube.transform.position.x - 15, Cube.transform.position.y + 15, Cube.transform.position.z);
                } else
                {
                    Cube.transform.localScale = new Vector3(Cube.transform.localScale.x - HeightOfOneCard, Cube.transform.localScale.y - 5, Cube.transform.localScale.z - HeightOfOneCard);
                    Cube.transform.position = new Vector3(Cube.transform.position.x, Cube.transform.position.y - 15, Cube.transform.position.z);
                }

            }
                

        }
    }
   
}
