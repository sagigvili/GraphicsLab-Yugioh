using UnityEngine;
using System.Collections;

public class PlayerTurnMaker : TurnMaker 
{
    public override void OnTurnStart()
    {
        base.OnTurnStart();
        // dispay a message that it is player`s turn
        if (p.PlayerID == 2)
        {
            new ShowMessageCommand(false, 2.0f).AddToQueue();
        } else
        {
            new ShowMessageCommand(true, 2.0f).AddToQueue();
        }
        
        p.DrawACard();
    }
}
