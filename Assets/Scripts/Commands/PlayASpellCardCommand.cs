using UnityEngine;
using System.Collections;
using DG.Tweening;

public class PlayASpellCardCommand: Command
{
    private CardLogic cl;
    private int tablePos;
    private Player p;
    private int spellTrapID;

    public PlayASpellCardCommand(CardLogic cl, Player p, int tablePos, int spellTrapID)
    {
        this.p = p;
        this.cl = cl;
        this.tablePos = tablePos;
        this.spellTrapID = spellTrapID;
    }

    public override void StartCommandExecution()
    {
        // remove and destroy the card in hand 
        HandVisual PlayerHand = p.PArea.handVisual;
        GameObject card = IDHolder.GetGameObjectWithID(cl.UniqueCardID);
        if (p.PArea.owner == AreaPosition.Top)
        {
            Sequence s = DOTween.Sequence();
            s.Insert(0f, card.transform.DOMove(p.PArea.tableVisual.SpellsTrapsSlots.Children[tablePos].transform.position, 1f));
            s.OnComplete(() =>
            {
                PlayerHand.RemoveCard(card);
                GameObject.Destroy(card);
                // enable Hover Previews Back
                HoverPreview.PreviewsAllowed = true;
                // move this card to the spot 
                p.PArea.tableVisual.AddSpellTrapAtIndex(cl.ca, spellTrapID, tablePos);
            });
        }
        else
        {
            PlayerHand.RemoveCard(card);
            GameObject.Destroy(card);
            // enable Hover Previews Back
            HoverPreview.PreviewsAllowed = true;
            // move this card to the spot 
            p.PArea.tableVisual.AddSpellTrapAtIndex(cl.ca, spellTrapID, tablePos);
        }
    }
}
