using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DragMonsterOnTable : DraggingActions
{

    private int savedHandSlot;
    private WhereIsTheCardOrMonster whereIsCard;
    private IDHolder idScript;
    private VisualStates tempState;
    private OneCardManager manager;

    public override bool CanDrag
    {
        get
        {
            if (playerOwner.PArea.tableVisual.getMonstersOnTableCount() < 3)
            {
                return base.CanDrag && manager.CanBePlayedNow;
            }
            else
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
        if (DragSuccessful() && !playerOwner.table.PlayedACard && !playerOwner.table.InAttackPhase)
        {
            transform.Find("StatesBalloon").transform.Find("Panel").gameObject.SetActive(true);
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
        playerOwner.table.PlayedACard = true;
        // determine table position
        int tablePos = playerOwner.PArea.tableVisual.getMonstersOnTableCount();
        // play this card
        CardLogic.CardsCreatedThisGame[GetComponent<IDHolder>().UniqueID].ca.MonsterState = manager.cardAsset.MonsterState;
        playerOwner.PlayAMonsterFromHand(GetComponent<IDHolder>().UniqueID, tablePos);
    }
    protected override bool DragSuccessful()
    {
        bool TableNotFull = (playerOwner.table.MonstersOnTable.Count < 3);

        return TableVisual.CursorOverSomeTable && TableNotFull;
    }

}