using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DragPlayerPowerActions : DraggingActions {

    public override void OnStartDrag()
    {
       
    }

    public override void OnDraggingInUpdate()
    {

    }

    public override void OnEndDrag()
    {

        // 1) Check if we are holding a card over the table
        if (DragSuccessful())
        {
            
        }
        else
        {
            
        } 
    }

    protected override bool DragSuccessful()
    {
        bool TableNotFull = (TurnManager.Instance.whoseTurn.table.MonstersOnTable.Count < 8);

        return TableVisual.CursorOverSomeTable&& TableNotFull;
    }
}
