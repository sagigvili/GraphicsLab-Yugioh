using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DragMonsterOnTable : DraggingActions {

    private int savedHandSlot;
    private WhereIsTheCardOrMonster whereIsCard;
    private IDHolder idScript;
    private VisualStates tempState;
    private OneCardManager manager;

    public override bool CanDrag
    {
        get
        {
            // TODO : include full field check
            //return true;
            return base.CanDrag && manager.CanBePlayedNow;
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
        if (DragSuccessful())
        {
            // determine table position
            int tablePos = playerOwner.PArea.tableVisual.getMonstersOnTableCount();
            // play this card
            playerOwner.PlayAMonsterFromHand(GetComponent<IDHolder>().UniqueID, tablePos);
        }
        else
        {
            // Set old sorting order 
            whereIsCard.SetHandSortingOrder();
            whereIsCard.VisualState = tempState;
            // TODO : Currenty returning to wrong place (slot place not like on drawing), fix it
            // Move this card back to its slot position
            Vector3 oldCardPos = transform.parent.localPosition;
            transform.DOLocalMove(oldCardPos, 1f);
        } 
    }

    protected override bool DragSuccessful()
    {
        bool TableNotFull = (playerOwner.table.MonstersOnTable.Count < 8);

        return TableVisual.CursorOverSomeTable && TableNotFull;
    }
}
