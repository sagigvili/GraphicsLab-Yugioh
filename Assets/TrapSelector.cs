using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapSelector : MonoBehaviour
{
    public List<TargetSlot> Traps = new List<TargetSlot>();
    public Transform panel;
    public SpellTrapEffects CurrentEffect;
    private bool isFinished = false;

    public void Start()
    {
        getTraps();
    }

    public void getTraps()
    {
        TableVisual tabVis = TurnManager.Instance.whoseTurn.otherPlayer.PArea.tableVisual;
        Table table = TurnManager.Instance.whoseTurn.otherPlayer.table;
        
        for (int i = 0; i < tabVis.getSpellsTrapsOnTableCount(); i++)
        {
            if(table.SpellsTrapsOnTable[i].Type == SpellOrTrap.Trap)
            {
                Traps[i].getTargetDetails(tabVis.getSpellTrapOnTable(i));
                Traps[i].gameObject.SetActive(true);
                // TODO: EFFECT BY THE TYPE OF THE TRAP
            }
            
        }
        
 
    }

    public void ActivateEffect(TargetSlot t)
    {
        Transform trap = t.target;
        int trapID = trap.gameObject.GetComponent<IDHolder>().UniqueID;
        switch (t.getEffect())
        {
/*            case Effects.DestoryMonster:
                MonsterLogic targetedMonster = MonsterLogic.MonstersCreatedThisGame[targetID];
                targetedMonster.Die();
                GameObject newMonsterOppField = GameObject.Instantiate(GlobalSettings.Instance.MonsterFieldPrefab, target.parent.transform.position, Quaternion.identity) as GameObject;
                newMonsterOppField.transform.SetParent(target.parent.transform);
                break;
            case Effects.DestorySpellTrap:
                SpellTrapLogic targetedSpellTrap = SpellTrapLogic.SpellTrapsCreatedThisGame[targetID];
                targetedSpellTrap.Die();
                GameObject newSpellOppTrapField = GameObject.Instantiate(GlobalSettings.Instance.SpellTrapFieldPrefab, target.parent.transform.position, Quaternion.identity) as GameObject;
                newSpellOppTrapField.transform.SetParent(target.parent.transform);
                break;*/
            case SpellTrapEffects.Negate:
                ((MonsterAttackCommand)Command.CommandQueue.Peek()).canAttack = false;
                Command.ClearQueue();
                break;
        }
        panel.gameObject.SetActive(false);
        isFinished = true;
        SpellTrapLogic.SpellTrapsCreatedThisGame[trap.gameObject.GetComponent<IDHolder>().UniqueID].Die();
    }

    public bool getFinished()
    {
        return isFinished;
    }

    public void setFinished(bool val)
    {
        isFinished = val;
    }

}
