using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DragSpellTrapOnTable : DraggingActions {

    private int savedHandSlot;
    private WhereIsTheCardOrMonster whereIsCard;
    private IDHolder idScript;
    private VisualStates tempState;
    private OneCardManager manager;

    public override bool CanDrag
    {
        get
        {
            if (playerOwner.PArea.tableVisual.getSpellsTrapsOnTableCount() < 3)
            {
                return base.CanDrag;
            } else
            {
                return false;
            }
        }
    }

    void Awake()
    {
        whereIsCard = GetComponent<WhereIsTheCardOrMonster>();
        manager = GetComponent<OneCardManager>();
    }

    public override void OnStartDrag()
    {
        savedHandSlot = whereIsCard.Slot;
        tempState = whereIsCard.VisualState;
        whereIsCard.VisualState = VisualStates.Dragging;
        whereIsCard.BringToFront();

    }

    public override void OnDraggingInUpdate()
    {

    }

    public override void OnEndDrag()
    {
        // 1) Check if we are holding a card over the table
        if (DragSuccessful() && !playerOwner.table.InAttackPhase)
        {
            SpellTrapEffects effect = GetComponent<OneCardManager>().cardAsset.Effect;
            transform.Find("StatesBalloon").transform.Find("Panel").gameObject.SetActive(true);

            if ((playerOwner.otherPlayer.table.MonstersOnTable.Count == 0) && (effect == SpellTrapEffects.DestoryMonster))
                transform.Find("StatesBalloon").transform.Find("Panel").GetComponent<SelectStateToTable>().summonState.gameObject.SetActive(false);

            if (AreThereNoSetMonstersInField(effect) && (effect == SpellTrapEffects.ChangeToAttack))
                transform.Find("StatesBalloon").transform.Find("Panel").GetComponent<SelectStateToTable>().summonState.gameObject.SetActive(false);

            if (AreThereNoSetMonstersInField(effect) && (effect == SpellTrapEffects.ChangeToDefence))
                transform.Find("StatesBalloon").transform.Find("Panel").GetComponent<SelectStateToTable>().summonState.gameObject.SetActive(false);

            if (playerOwner.otherPlayer.table.SpellsTrapsOnTable.Count == 0 && effect == SpellTrapEffects.DestorySpellTrap)
                transform.Find("StatesBalloon").transform.Find("Panel").GetComponent<SelectStateToTable>().summonState.gameObject.SetActive(false);

            if (GetComponent<OneCardManager>().cardAsset.SpellTrap == SpellOrTrap.Trap)
                transform.Find("StatesBalloon").transform.Find("Panel").GetComponent<SelectStateToTable>().summonState.gameObject.SetActive(false);

            if (effect == SpellTrapEffects.Revive && TurnManager.Instance.whoseTurn.graveyard.cards.Count == 0)
                transform.Find("StatesBalloon").transform.Find("Panel").GetComponent<SelectStateToTable>().summonState.gameObject.SetActive(false);

            manager.toLoop = true;
        }
        else
        {
            // Set old sorting order 
            whereIsCard.SetHandSortingOrder();
            whereIsCard.VisualState = tempState;
            // Move this card back to its slot position
            Vector3 oldCardPos = transform.parent.localPosition;
            transform.DOLocalMove(oldCardPos, 0.3f);
            playerOwner.PArea.handVisual.ReCalculateCards();
        } 
    }


    public void AfterStateSelected()
    {
        transform.Find("StatesBalloon").transform.Find("Panel").gameObject.SetActive(false);
        // determine table position
        int tablePos = playerOwner.PArea.tableVisual.getSpellsTrapsOnTableCount();
        // play this card
        CardLogic.CardsCreatedThisGame[GetComponent<IDHolder>().UniqueID].ca.SpellTrapState = manager.cardAsset.SpellTrapState;
        playerOwner.PlayASpellFromHand(GetComponent<IDHolder>().UniqueID, tablePos);
    }

    public bool AreThereNoSetMonstersInField(SpellTrapEffects effect)
    {
        if (playerOwner.otherPlayer.table.MonstersOnTable.Count == 0)
            return true;
        if (effect == SpellTrapEffects.ChangeToAttack)
        {
            if (TurnManager.Instance.whoseTurn.otherPlayer.table.AnyAttackMonsters())
                return true;
            return false;
        }

        if (effect == SpellTrapEffects.ChangeToDefence)
        {
            if (TurnManager.Instance.whoseTurn.otherPlayer.table.AnyDefenceMonsters())
                return true;
            return false;
        }

/*        foreach (MonsterLogic m in playerOwner.otherPlayer.table.MonstersOnTable)
        {
            if (!(m.monsterPosition == FieldPosition.Set))
                return false;
        }*/
        return true;
    }
    protected override bool DragSuccessful()
    {
        Debug.Log("Cursor over SPELLTRAP " + TableVisual.CursorOverSomeTable.ToString());
        bool TableNotFull = (playerOwner.table.SpellsTrapsOnTable.Count < 3);

        return TableVisual.CursorOverSomeTable && TableNotFull;
    }

}
