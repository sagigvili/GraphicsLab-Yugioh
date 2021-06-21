using UnityEngine;
using System.Collections;

public class DamageAllMonsters : SpellEffect {

    public override void ActivateEffect(int specialAmount = 0, ICharacter target = null)
    {
        MonsterLogic[] MonstersToDamage = TurnManager.Instance.whoseTurn.otherPlayer.table.MonstersOnTable.ToArray();
        foreach (MonsterLogic cl in MonstersToDamage)
        {
            new DealDamageCommand(cl.ID, specialAmount, healthAfter: cl.Health - specialAmount).AddToQueue();
            cl.Health -= specialAmount;
        }
    }
}
