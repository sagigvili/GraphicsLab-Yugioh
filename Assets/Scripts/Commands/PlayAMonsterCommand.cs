using UnityEngine;
using System.Collections;

public class PlayAMonsterCommand : Command
{
    private CardLogic cl;
    private int tablePos;
    private Player p;
    private int monsterID;

    public PlayAMonsterCommand(CardLogic cl, Player p, int tablePos, int creatureID)
    {
        this.p = p;
        this.cl = cl;
        this.tablePos = tablePos;
        this.monsterID = creatureID;
    }

    public override void StartCommandExecution()
    {
        // remove and destroy the card in hand 
        HandVisual PlayerHand = p.PArea.handVisual;
        GameObject card = IDHolder.GetGameObjectWithID(cl.UniqueCardID);
        PlayerHand.RemoveCard(card);
        GameObject.Destroy(card);
        // enable Hover Previews Back
        HoverPreview.PreviewsAllowed = true;
        // move this card to the spot 
        p.PArea.tableVisual.AddMonsterAtIndex(cl.ca, monsterID, tablePos);
    }
}
