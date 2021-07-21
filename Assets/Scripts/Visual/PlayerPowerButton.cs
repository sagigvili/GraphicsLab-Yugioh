using UnityEngine;
using System.Collections;

public class PlayerPowerButton : MonoBehaviour {

    public AreaPosition owner;


    private bool wasUsed = false;
    public bool WasUsed
    { 
        get
        {
            return wasUsed;
        } 
        set
        {
            wasUsed = value;
        }
    }

    void OnMouseDown()
    {
        if (!WasUsed && TurnManager.Instance.whoseTurn.PArea.owner == owner)
        {
            new PlayerPowerCommand(this).AddToQueue();
        }
    }
}
