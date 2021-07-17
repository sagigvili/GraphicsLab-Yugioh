using UnityEngine;
using System.Collections;


public class MonsterAttackCommand : Command 
{
    // position of creature on enemy`s table that will be attacked
    // if enemyindex == -1 , attack an enemy character 
    private int TargetUniqueID;
    private int AttackerUniqueID;
    private int TargetHPAfter;
    private int DamageTakenByAttacker;
    private int DamageTakenByTarget;
    public bool canAttack = true;

    public MonsterAttackCommand(int targetID, int attackerID, int damageTakenByAttacker, int damageTakenByTarget, int targetHPAfter)
    {
        this.TargetUniqueID = targetID;
        this.AttackerUniqueID = attackerID;
        this.TargetHPAfter = targetHPAfter;
        this.DamageTakenByTarget = damageTakenByTarget;
        this.DamageTakenByAttacker = damageTakenByAttacker;
    }

    public override void StartCommandExecution()
    {

        if (canAttack)
        {
            GameObject Attacker = IDHolder.GetGameObjectWithID(AttackerUniqueID);

            Attacker.GetComponent<MonsterAttackVisual>().AttackTarget(TargetUniqueID, DamageTakenByTarget, DamageTakenByAttacker, TargetHPAfter);
        }
        else
        {
            CommandExecutionComplete();
        }

    }
}
