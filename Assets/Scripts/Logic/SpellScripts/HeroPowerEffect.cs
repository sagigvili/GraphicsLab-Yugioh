using UnityEngine;
using System.Collections;

public class HeroPowerEffect : MonoBehaviour {

    public void ActivateEffect(Player p)
    {
        if (p.PlayerID == 1)
        {
            MonsterLogic[] MonstersToDestory = TurnManager.Instance.whoseTurn.otherPlayer.table.MonstersOnTable.ToArray();
            foreach (MonsterLogic ml in MonstersToDestory)
            {
                new MonsterDieCommand(ml.UniqueMonsterID, p).AddToQueue();
            }
        } else
        {
            SpellTrapLogic[] SpellTrapsToDestory = TurnManager.Instance.whoseTurn.otherPlayer.table.SpellsTrapsOnTable.ToArray();
            foreach (SpellTrapLogic stl in SpellTrapsToDestory)
            {
                new SpellTrapDieCommand(stl.UniqueSpellTrapID, p).AddToQueue();
            }
        }

    }
}
