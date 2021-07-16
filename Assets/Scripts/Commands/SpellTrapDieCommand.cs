using UnityEngine;
using System.Collections;

public class SpellTrapDieCommand : Command 
{
    private Player p;
    private int DestroiedSpellTrapID;

    public SpellTrapDieCommand(int SpellTrapID, Player p)
    {
        this.p = p;
        this.DestroiedSpellTrapID = SpellTrapID;
    }

    public override void StartCommandExecution()
    {
        p.PArea.tableVisual.RemoveSpellTrapWithID(DestroiedSpellTrapID);
    }
}
