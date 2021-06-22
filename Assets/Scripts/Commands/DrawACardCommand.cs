using UnityEngine;
using System.Collections;

public class DrawACardCommand : Command {
    // first argument
    // "1" - fast
    // "0" - normal

    private Player p;
    private CardLogic cl;
    private bool fast;
    private bool fromDeck;

    public DrawACardCommand(CardLogic cl, Player p, bool fast, bool fromDeck)
    {
        Debug.Log("1");
        this.cl = cl;
        this.p = p;
        this.fast = fast;
        this.fromDeck = fromDeck;
    }

    public override void StartCommandExecution()
    {
        Debug.Log("2");
        p.PArea.PDeck.CardsInDeck--;
        p.PArea.handVisual.GivePlayerACard(cl.ca, cl.UniqueCardID, fast, fromDeck);
    }
}
