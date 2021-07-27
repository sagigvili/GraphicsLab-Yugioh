using UnityEngine;
using System.Collections;

public class ReviveACardCommand : Command {

    private Player p;
    private CardLogic cl;

    public ReviveACardCommand(CardLogic cl, Player p)
    {
        this.cl = cl;
        this.p = p;
    }

    public override void StartCommandExecution()
    {
        p.PArea.graveyardVisual.ReviveACard(cl.ca);
        p.graveyard.cards.Remove(cl.ca);
        p.PArea.handVisual.GivePlayerACardFromGraveyard(cl.ca, cl.UniqueCardID);
        
    }
}
