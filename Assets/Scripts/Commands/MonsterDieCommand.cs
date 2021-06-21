using UnityEngine;
using System.Collections;

public class MonsterDieCommand : Command 
{
    private Player p;
    private int DeadMonsterID;

    public MonsterDieCommand(int MonsterID, Player p)
    {
        this.p = p;
        this.DeadMonsterID = MonsterID;
    }

    public override void StartCommandExecution()
    {
        p.PArea.tableVisual.RemoveMonsterWithID(DeadMonsterID);
    }
}
