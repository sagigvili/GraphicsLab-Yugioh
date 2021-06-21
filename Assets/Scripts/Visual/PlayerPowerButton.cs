using UnityEngine;
using System.Collections;

public class PlayerPowerButton : MonoBehaviour {

    public AreaPosition owner;


    private bool wasUsed = false;
    public bool WasUsedThisTurn
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
        if (!WasUsedThisTurn)
        {
            GlobalSettings.Instance.Players[owner].UsePlayerPower();
            WasUsedThisTurn= !WasUsedThisTurn;
        }
    }
}
